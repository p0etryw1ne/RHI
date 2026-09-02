# commit-i18n.ps1 -- Commits the RHI zh-CN localization work in this fork.
#
# Run from a normal PowerShell session on the host (NOT from inside
# the Codex sandbox, which blocks writes to .git/index.lock):
#
#   cd D:\AI\Codex\RHI
#   .\commit-i18n.ps1
#
# Stages every i18n file and creates ONE commit. To split into multiple
# commits later, run `git reset HEAD~1` and re-stage by hand.

$ErrorActionPreference = "Stop"
Set-Location -Path $PSScriptRoot

# Stage every i18n-related change (tracked + untracked).
git add .gitignore
git add "RenoDXCommander/Localization"
git add "RenoDXCommander.Tests/LocalizerTests.cs"
git add "RenoDXCommander/App.xaml"
git add "RenoDXCommander/App.xaml.cs"
git add "RenoDXCommander/MainWindow.xaml"
git add "RenoDXCommander/MainWindow.xaml.cs"
git add "RenoDXCommander/DialogService.cs"
git add "RenoDXCommander/DialogService.Game.cs"
git add "RenoDXCommander/DialogService.Update.cs"
git add "RenoDXCommander/SettingsHandler.cs"
git add "RenoDXCommander/DetailPanelBuilder.cs"
git add "RenoDXCommander/MainWindow.FaqBuilder.cs"

Write-Host "--- Staged ---"
git --no-pager diff --cached --stat

$body = @"
i18n(zh-CN): add Simplified Chinese localization (102 keys)

RHI fork -- Simplified Chinese localization layer that does not modify
upstream source files beyond minimal tagged-property annotations.

Skeleton
========
* Localization/Strings.resx        -- neutral English fallback, 102 keys
* Localization/Strings.zh-CN.resx  -- Simplified Chinese translations
* Localization/Localizer.cs         -- singleton INotifyPropertyChanged wrapper
                                       around ResourceManager; static Get/Format
                                       facade for terse call sites
* Localization/Loc.cs               -- attached-property walker: tag XAML
                                       elements with loc:Loc.Key / ToolTipKey
                                       and ApplyTo(root) wires everything up
* App.xaml                          -- implicit FontFamily style
                                       (Segoe UI, Microsoft YaHei UI, 微软雅黑)
                                       for clean Chinese glyph rendering
* App.xaml.cs                       -- Localizer.InitializeStartupCulture()
                                       auto-picks zh-CN when system UI is
                                       Chinese, otherwise falls back to
                                       English (neutral resx)
* Tests/LocalizerTests.cs           -- 6 unit tests covering neutral + zh-CN
                                       resolution, missing-key placeholder,
                                       culture-switch INPC, and the
                                       InitializeStartupCulture branches

Translated surface (102 keys)
============================
Toolbar (8 buttons + 8 tooltips):
  NewModsAvailable, QuickStart, Refresh, ShadersAddons, UpdateAll,
  Links, Help, Settings + matching tips
Menus (10): GlobalShaders, ReShadeAddons, RenoDxWiki, LumaWiki,
  RhiGithub, ReLimiterGithub, Discord, Guide, Kofi, About
Sidebar filter chips (9): AllGames, Installed, Favourites, Hidden,
  Unreal, Unity, Other, RenoDX, Luma
Detail panel: Favourite
Loading status: Loading...

DialogService (15 keys): Vulkan ReShade, Admin required, RTX HDR badge,
  MOTD, DownloadFailed, generic Close/Cancel/OK/Don't-show-again
SettingsHandler (49 keys): 11 dialog titles + 19 contents + 11
  interpolated strings (Format) covering screenshots, peak nits,
  admin mode, log copy, purge staging, ReShade hotkeys, ReLimiter FPS,
  Nexus Mods API key flow, NXM protocol handler, DXVK variant switch,
  ReShade build channel switch, About version line, generic labels.

Why attached properties instead of {Binding}
============================================
RHI targets WinUI 3 (Microsoft.UI.Xaml), whose XAML compiler rejects
the {Binding ...} markup extension at compile time. An attached
property is the lowest-friction alternative: zero markup extension,
no compile-time expression to validate, and one ApplyTo() call in the
window constructor wires every tagged element into the Localizer.

Window.Title is set once in MainWindow code-behind from Localizer at
construction (WinUI 3 Window.Title is not a DependencyProperty).

Brand / proper nouns kept in English per PC gaming convention:
ReShade, RenoDX, OptiScaler, DLSS, HDR, Steam, Epic, Unreal Engine,
ReBAR, G-Sync, Frame Generation, Discord, Ko-fi.

Verified
========
* dotnet build RenoDXCommander.csproj -c Debug -p:Platform=x64  -> 0 errors
* dotnet test  RenoDXCommander.Tests.csproj                    -> 38/38 pass
* Existing CoreLogicTests untouched

Merge notes for upstream syncs
==============================
XAML diff is intentionally minimal: every translation point is an
extra attached property on the original element; the original Text /
Content / ToolTipService.ToolTip value is left untouched as Visual
Studio designer fallback. Strings.resx is fork-only -- adding it to
git ls-files excludes in upstream's history will simply no-op there.

After merge from upstream/main:
* XAML changes will conflict only where upstream modifies the same
  line; in those cases the resolution is mechanical (keep the
  upstream text, add the loc:Loc.Key attribute the fork added).
* Strings.resx / Strings.zh-CN.resx / Localization/*.cs will merge
  cleanly because upstream does not touch them.
* C# changes (DialogService*.cs, SettingsHandler.cs, etc.) will
  conflict wherever upstream touches the same line; resolve manually
  but the pattern is consistent: "..." -> Localizer.Get("Key") or
  $"..." -> Localizer.Format("Key", args).
"@

git commit -m $body

if ($LASTEXITCODE -ne 0) {
    Write-Error "git commit failed"
    exit 1
}

Write-Host ""
Write-Host "=== Done ==="
git --no-pager log --oneline -3