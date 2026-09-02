using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace RenoDXCommander.Localization;

/// <summary>
/// Attached property set that marks a XAML element for localization.
/// Use in XAML as <c>loc:Loc.Key="SomeKey"</c> on any element that exposes
/// a string <c>Text</c>-style property, plus optional <c>loc:Loc.ToolTipKey</c>
/// for tooltips.
///
/// <para>
/// Why attached properties instead of <c>{Binding [Key]}</c>?
/// </para>
/// <para>
/// RHI targets WinUI 3 (Microsoft.UI.Xaml), whose XAML compiler does not
/// accept the <c>{Binding ...}</c> markup extension at compile time and
/// has a notoriously narrow set of supported expression shapes. Static
/// resource / x:Bind paths through cross-assembly types also fail. An
/// attached property is the lowest-friction alternative: zero markup
/// extension involved, no compile-time expression to validate, and the
/// wire-up happens once at runtime via <see cref="ApplyTo"/>.
/// </para>
///
/// <para>
/// After elements are tagged in XAML, call
/// <c>Loc.ApplyTo(rootElement)</c> from the owning window\'s constructor.
/// The walker records each tagged element + key, applies the current
/// localized string, and subscribes to <see cref="Localizer.PropertyChanged"/>
/// so a culture switch refreshes every tagged element in one pass.
/// </para>
/// </summary>
public static class Loc
{
    /// <summary>
    /// Attached property holding the resource key for an element\'s primary
    /// string content (TextBlock.Text, Button.Content, Run.Text, etc.).
    /// </summary>
    public static readonly DependencyProperty KeyProperty =
        DependencyProperty.RegisterAttached(
            "Key",
            typeof(string),
            typeof(Loc),
            new PropertyMetadata(null, OnKeyChanged));

    /// <summary>
    /// Optional attached property for the element\'s tooltip string.
    /// Used when the element exposes a <c>ToolTipService.ToolTip</c> string.
    /// </summary>
    public static readonly DependencyProperty ToolTipKeyProperty =
        DependencyProperty.RegisterAttached(
            "ToolTipKey",
            typeof(string),
            typeof(Loc),
            new PropertyMetadata(null, OnToolTipKeyChanged));

    public static string? GetKey(DependencyObject obj) => (string?)obj.GetValue(KeyProperty);
    public static void SetKey(DependencyObject obj, string? value) => obj.SetValue(KeyProperty, value);

    public static string? GetToolTipKey(DependencyObject obj) => (string?)obj.GetValue(ToolTipKeyProperty);
    public static void SetToolTipKey(DependencyObject obj, string? value) => obj.SetValue(ToolTipKeyProperty, value);

    // ── Walker state ────────────────────────────────────────────────────
    //
    // Holds every (element, key, original-content) tuple so that the
    // refresh path can reapply the localized string without walking the
    // visual tree again. Cleared by ApplyTo() each time it runs.
    private static readonly List<Entry> _entries = new();

    private readonly struct Entry
    {
        public Entry(DependencyObject element, string key, ContentKind kind)
        {
            Element = element;
            Key = key;
            Kind = kind;
        }
        public DependencyObject Element { get; }
        public string Key { get; }
        public ContentKind Kind { get; }
    }

    private enum ContentKind { Text, Content, ToolTip, RunText, Placeholder, MenuText }

    /// <summary>
    /// Walk the visual subtree starting at <paramref name="root"/>, collect
    /// every element tagged with <c>Loc.Key</c> or <c>Loc.ToolTipKey</c>,
    /// apply the current localized text, and subscribe to
    /// <see cref="Localizer.PropertyChanged"/> so a culture switch refreshes
    /// all of them in one pass.
    ///
    /// Safe to call multiple times — previous entries are dropped before
    /// the new walker runs.
    /// </summary>
    public static void ApplyTo(DependencyObject root)
    {
        if (root is null) return;

        DetachFromLocalizer();
        _entries.Clear();

        Collect(root);
        AttachToLocalizer();
        RefreshAll();
    }

    private static void Collect(DependencyObject node)
    {
        var key = GetKey(node);
        if (!string.IsNullOrEmpty(key))
        {
            var kind = node switch
            {
                TextBlock => ContentKind.Text,
                Button => ContentKind.Content,
                ComboBoxItem => ContentKind.Content,
                RadioButton => ContentKind.Content,
                Microsoft.UI.Xaml.Controls.Primitives.ToggleButton => ContentKind.Content,
                Run => ContentKind.RunText,
                MenuFlyoutItem => ContentKind.MenuText,
                TextBox => ContentKind.Placeholder,
                _ => ContentKind.Text, // falls through to runtime branch
            };
            _entries.Add(new Entry(node, key!, kind));
        }

        var ttKey = GetToolTipKey(node);
        if (!string.IsNullOrEmpty(ttKey))
            _entries.Add(new Entry(node, ttKey!, ContentKind.ToolTip));

        // Recurse into children. Works for both FrameworkElement descendants
        // and Microsoft.UI.Xaml.FrameworkElement (which has .Children access
        // through VisualTreeHelper).
        // WinUI 3 VisualTreeHelper is unreliable for panels that were created
        // collapsed: it reports 0 children even once the panel becomes visible.
        // Walk the logical-ish tree instead (Panel.Children / ContentControl /
        // ScrollViewer / ItemsControl), falling back to VisualTreeHelper.
        switch (node)
        {
            case Panel panel:
                foreach (var child in panel.Children)
                    Collect(child);
                break;
            case ContentControl cc:
                if (cc.Content is DependencyObject c1) Collect(c1);
                break;
            case ItemsControl items:
                foreach (var item in items.Items)
                    if (item is DependencyObject i1) Collect(i1);
                break;
            case TextBlock textBlock:
                foreach (var inline in textBlock.Inlines)
                    if (inline is DependencyObject di) Collect(di);
                break;
            case MenuFlyout menuFlyout:
                foreach (var item in menuFlyout.Items)
                    if (item is DependencyObject i2) Collect(i2);
                break;
            case MenuFlyoutSubItem sub:
                foreach (var item in sub.Items)
                    if (item is DependencyObject i3) Collect(i3);
                break;
            case Flyout flyout:
                if (flyout.Content is DependencyObject dc) Collect(dc);
                break;
            case UIElement:
                var count = VisualTreeHelper.GetChildrenCount(node);
                for (int i = 0; i < count; i++)
                    Collect(VisualTreeHelper.GetChild(node, i));
                break;
            default:
                // Run and other non-visual DependencyObjects do not implement
                // the visual-tree COM interface. Their own localization key
                // has already been collected above, so there is nothing else
                // to traverse here.
                break;
        }

        // Popups / flyouts are NOT part of the visual tree until opened.
        // Walk attached popup roots so their (collapsed) content still gets
        // localized when the flyout is first shown.
        if (node is FrameworkElement fe)
        {
            var attached = FlyoutBase.GetAttachedFlyout(fe);
            if (attached is DependencyObject ad && ad != node) Collect(ad);
        }
        if (node is Button btn)
        {
            var fly = btn.Flyout;
            if (fly is DependencyObject fd && fd != node) Collect(fd);
        }
    }

    private static void AttachToLocalizer()
    {
        if (_entries.Count > 0)
            Localizer.Instance.PropertyChanged += OnLocalizerChanged;
    }

    private static void DetachFromLocalizer()
    {
        if (_entries.Count > 0)
            Localizer.Instance.PropertyChanged -= OnLocalizerChanged;
    }

    private static void OnLocalizerChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Indexer bindings fire "Item[]"; Culture setter fires both.
        if (e.PropertyName == "Item[]" || e.PropertyName == nameof(Localizer.Culture))
            RefreshAll();
    }

    private static void RefreshAll()
    {
        foreach (var entry in _entries)
        {
            try
            {
                ApplyToElement(entry);
            }
            catch { /* one bad element must not break the whole tree */ }
        }
    }

    private static void ApplyToElement(Entry entry)
    {
        var text = Localizer.Instance[entry.Key];
        switch (entry.Kind)
        {
            case ContentKind.Text:
                if (entry.Element is TextBlock tb) tb.Text = text;
                break;
            case ContentKind.Content:
                if (entry.Element is Button btn) btn.Content = text;
                else if (entry.Element is ContentControl cc) cc.Content = text;
                break;
            case ContentKind.RunText:
                if (entry.Element is Run run) run.Text = text;
                break;
            case ContentKind.ToolTip:
                ToolTipService.SetToolTip(entry.Element, text);
                break;
            case ContentKind.Placeholder:
                if (entry.Element is TextBox tb3) tb3.PlaceholderText = text;
                break;
            case ContentKind.MenuText:
                if (entry.Element is MenuFlyoutItem mfi) mfi.Text = text;
                else if (entry.Element is ContentControl cc2) cc2.Content = text;
                break;
        }
    }

    // Property change handlers — currently no-op. Kept so designers see the
    // attached properties update in the property panel when editing XAML.
    private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
    private static void OnToolTipKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
}
