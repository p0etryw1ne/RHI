using System.Globalization;
using RenoDXCommander.Localization;
using Xunit;

namespace RenoDXCommander.Tests;

/// <summary>
/// Smoke tests for the RHI fork localization primitives. The tests do not
/// touch the visual tree; they only confirm the <see cref="Localizer"/>
/// resolves keys against the embedded <c>Strings.resx</c> and the
/// <c>Strings.zh-CN.resx</c> satellite, and that <see cref="Localizer.Culture"/>
/// switches the active language on demand.
/// </summary>
public class LocalizerTests
{
    [Fact]
    public void Neutral_Resolves_English_Key()
    {
        // Snapshot whatever culture the test runner has so we can restore it.
        var original = Localizer.Instance.Culture;
        try
        {
            Localizer.Instance.Culture = CultureInfo.InvariantCulture;
            Assert.Equal("RHI", Localizer.Instance["AppTitle"]);
            Assert.Equal("Simplified PC Gaming", Localizer.Instance["AppSubtitle"]);
            Assert.Equal("Refresh", Localizer.Instance["Toolbar_Refresh"]);
        }
        finally
        {
            Localizer.Instance.Culture = original;
        }
    }

    [Fact]
    public void ZhCn_Resolves_Translated_Key()
    {
        var original = Localizer.Instance.Culture;
        try
        {
            Localizer.Instance.Culture = new CultureInfo("zh-CN");
            Assert.Equal("RHI", Localizer.Instance["AppTitle"]);
            Assert.Equal("\u7b80\u5316\u7684 PC \u6e38\u620f HDR \u7ba1\u7406", Localizer.Instance["AppSubtitle"]);
            Assert.Equal("\u5237\u65b0", Localizer.Instance["Toolbar_Refresh"]);
        }
        finally
        {
            Localizer.Instance.Culture = original;
        }
    }

    [Fact]
    public void Missing_Key_Returns_Bracketed_Placeholder()
    {
        // Makes typos and unfinished translations obvious during development
        // rather than silently rendering empty strings.
        var original = Localizer.Instance.Culture;
        try
        {
            Localizer.Instance.Culture = CultureInfo.InvariantCulture;
            Assert.Equal("#NoSuchKey#", Localizer.Instance["NoSuchKey"]);
        }
        finally
        {
            Localizer.Instance.Culture = original;
        }
    }

    [Fact]
    public void Culture_Switch_Raises_PropertyChanged()
    {
        var original = Localizer.Instance.Culture;
        try
        {
            var raised = new List<string>();
            Localizer.Instance.PropertyChanged += (s, e) => raised.Add(e.PropertyName ?? "");

            Localizer.Instance.Culture = new CultureInfo("zh-CN");

            // WPF indexer bindings watch for "Item[]"; explicit name change
            // notifies the Culture property itself.
            Assert.Contains("Item[]", raised);
            Assert.Contains(nameof(Localizer.Culture), raised);
        }
        finally
        {
            Localizer.Instance.Culture = original;
        }
    }

    [Fact]
    public void InitializeStartupCulture_Picks_Zh_For_Chinese_Systems()
    {
        var saved = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");
            Localizer.InitializeStartupCulture();
            Assert.Equal("zh-CN", Localizer.Instance.Culture.Name);
        }
        finally
        {
            CultureInfo.CurrentUICulture = saved;
        }
    }

    [Fact]
    public void InitializeStartupCulture_Picks_Invariant_For_Non_Chinese_Systems()
    {
        var saved = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            Localizer.InitializeStartupCulture();
            Assert.Equal(CultureInfo.InvariantCulture, Localizer.Instance.Culture);
        }
        finally
        {
            CultureInfo.CurrentUICulture = saved;
        }
    }
}