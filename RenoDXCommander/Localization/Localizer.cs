using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace RenoDXCommander.Localization;

/// <summary>
/// Single-source-of-truth localizer for RHI.
///
/// Resolution order for any key:
///   1. <c>Strings.&lt;Culture&gt;.resx</c> matching the active <see cref="Culture"/>
///   2. Fallback up the culture hierarchy (zh-CN -> zh -> neutral)
///   3. The neutral <c>Strings.resx</c> (English copy)
///
/// XAML binding pattern (supports runtime language switching):
///   <code>
///   &lt;TextBlock Text="{Binding [AppTitle], Source={x:Static local:Localizer.Instance}}" /&gt;
///   </code>
///
/// C# usage:
///   <code>
///   var text = Localizer.Instance["SomeKey"];
///   </code>
///
/// When <see cref="Culture"/> is changed, the localizer raises
/// <c>PropertyChanged("Item[]")</c> so every WPF binding to an indexed
/// accessor refreshes in one pass.
/// </summary>
public sealed class Localizer : INotifyPropertyChanged
{
    // ResourceManager is keyed by namespace + base file name (without .resx).
    // The .NET host automatically discovers satellite assemblies for any
    // culture-specific *.resources.dll embedded next to Strings.resources.
    private static readonly ResourceManager _rm = new(
        "RenoDXCommander.Localization.Strings",
        typeof(Localizer).Assembly);

    /// <summary>Sole instance — XAML binds to this with <c>x:Static</c>.</summary>
    public static Localizer Instance { get; } = new();

    private CultureInfo _culture = DetectInitialCulture();

    /// <summary>
    /// Active UI culture. Setting it updates <see cref="CultureInfo.CurrentUICulture"/>
    /// (affects any subsequent ResourceManager lookups) and notifies
    /// every <c>[Key]</c> binding to refresh.
    /// </summary>
    public CultureInfo Culture
    {
        get => _culture;
        set
        {
            if (value is null) return;
            if (_culture.Name == value.Name) return;
            _culture = value;
            CultureInfo.CurrentUICulture = value;
            // WPF indexer bindings watch for "Item[]" property changes.
            OnIndexerChanged();
            OnPropertyChanged(nameof(Culture));
        }
    }

    /// <summary>
    /// Returns the localized string for <paramref name="key"/>, or the
    /// bracketed key (e.g. <c>#SomeKey#</c>) if no resource is found —
    /// makes missing translations obvious during development.
    /// </summary>
    public string this[string key]
    {
        get
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            var value = _rm.GetString(key, _culture);
            return value ?? "#" + key + "#";
        }
    }

    /// <summary>
    /// Static facade for <see cref="Localizer.Instance"/>[key]. Use from
    /// C# code where a single line looks cleaner than chaining through
    /// <c>Localizer.Instance[...]</c>.
    /// </summary>
    public static string Get(string key) => Instance[key];

    /// <summary>
    /// Format helper: pulls a format string from resources and substitutes
    /// <paramref name="args"/>. Use for any string that contains <c>{0}</c>
    /// placeholders.
    /// </summary>
    public static string Format(string key, params object[] args) =>
        Instance.FormatCore(key, args);

    /// <summary>
    /// Instance implementation of <see cref="Format"/>. Exposed as a regular
    /// method so subclasses (or test doubles) can override behavior while
    /// keeping the static call site stable.
    /// </summary>
    public string FormatCore(string key, params object[] args)
    {
        var fmt = this[key];
        return args is null || args.Length == 0
            ? fmt
            : string.Format(CultureInfo.CurrentCulture, fmt, args);
    }

    /// <summary>
    /// Called once from <c>App()</c> before any XAML or service starts
    /// touching the resource manager. Sets <see cref="CultureInfo.CurrentUICulture"/>
    /// (and the active <see cref="Culture"/>) so satellite resources resolve
    /// correctly on first bind.
    ///
    /// Detection rule (kept intentionally simple):
    ///   * System UI culture starts with <c>zh</c> → use the exact system
    ///     culture so a future <c>Strings.zh-HK.resx</c> would still pick up
    ///     its own file via fallback.
    ///   * Anything else → leave CurrentUICulture on Invariant, which makes
    ///     the fallback chain resolve to the neutral English <c>Strings.resx</c>.
    /// </summary>
    public static void InitializeStartupCulture()
    {
        // RHI zh-CN fork: always normalize to zh-CN. The satellite resource is
        // only shipped as zh-CN (Strings.zh-CN.resx). Systems whose display
        // language is zh-Hans-CN / zh-TW / zh-HK / zh-SG would otherwise fall
        // through to the neutral English resource because ResourceManager cannot
        // find a satellite for those exact tags. Normalizing to zh-CN guarantees
        // the translated UI regardless of the OS language.
        var target = new CultureInfo("zh-CN");

        CultureInfo.CurrentUICulture = target;
        Instance._culture = target;
        // No PropertyChanged raise needed — UI hasn't bound yet.
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnIndexerChanged() =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private static CultureInfo DetectInitialCulture() => CultureInfo.InvariantCulture;

    /// <summary>
    /// Available culture codes for the language switcher. Add an entry
    /// here whenever a new satellite file is shipped.
    /// </summary>
    public static IReadOnlyList<CultureInfo> SupportedCultures { get; } = new[]
    {
        new CultureInfo("en-US"), // neutral English copy
        new CultureInfo("zh-CN"), // Simplified Chinese
    };

    /// <summary>Human-readable label for the language switcher.</summary>
    public static string DisplayNameFor(CultureInfo culture) => culture.Name switch
    {
        "zh-CN" => "简体中文",
        "en-US" => "English",
        _ => culture.NativeName,
    };
}
