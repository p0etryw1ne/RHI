using System.Globalization;
using RenoDXCommander.Localization;
using Xunit;

namespace RenoDXCommander.Tests;

[Collection("Localizer")]
public class ZhResolveTests
{
    [Fact]
    public void Zh_Resolves_Recently_Added_Keys()
    {
        var orig = Localizer.Instance.Culture;
        try
        {
            Localizer.Instance.Culture = new CultureInfo("zh-CN");
            Assert.Equal("组件", Localizer.Get("DP_Components"));
            Assert.Equal("就绪", Localizer.Get("Status_Ready"));
            Assert.Equal("配置", Localizer.Get("DP_Config"));
            Assert.Equal("更新包含", Localizer.Get("Settings_UpdateInclusion"));
        }
        finally { Localizer.Instance.Culture = orig; }
    }
}