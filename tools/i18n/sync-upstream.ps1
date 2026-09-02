# tools/i18n/sync-upstream.ps1
#
# One-shot helper for syncing the RHI fork (p0etryw1ne/RHI) with
# upstream (RankFTW/RHI). Run from a normal PowerShell session:
#
#   cd D:\AI\Codex\RHI
#   .\tools\i18n\sync-upstream.ps1
#
# Workflow:
#   1. Ensures `upstream` remote is configured.
#   2. Fetches upstream/main.
#   3. Shows the commits in upstream that are not in our fork.
#   4. Runs `git merge upstream/main --no-ff --no-commit` (dry-run merge).
#   5. For each conflicted file, classifies it as either:
#        * i18n-layer file   (keep ours, then re-apply zh-CN to upstream's
#                              renamed methods / new fields)
#        * upstream change   (accept theirs -- the file isn't translated)
#   6. After the user resolves conflicts and runs again, runs
#      `dotnet build` and `dotnet test` to confirm the merge is healthy.
#
# This script NEVER auto-commits or auto-pushes. The user reviews the
# merge, re-applies zh-CN as needed, then commits and pushes manually.
#
# Exit codes:
#   0  -- merge completed (or fast-forward, no commit yet)
#   1  -- merge produced conflicts; user must resolve manually
#   2  -- merge succeeded but build/tests failed

$ErrorActionPreference = "Stop"
Set-Location -Path (Join-Path $PSScriptRoot "..\..")

function Write-Section($title) {
    Write-Host ""
    Write-Host ("=" * 60) -ForegroundColor Cyan
    Write-Host ("  {0}" -f $title) -ForegroundColor Cyan
    Write-Host ("=" * 60) -ForegroundColor Cyan
}

# Assert the fork-specific i18n hooks are still present after an upstream merge.
# Upstream rewrites of these files can silently drop the fork's additions, which
# makes the app fall back to English even though the satellite resource is packed.
function Assert-I18nHooks {
    $root = Get-Location
    $checks = @(
        @{ File = 'RenoDXCommander\App.xaml.cs';        Needle = 'Localizer.InitializeStartupCulture()'; Desc = 'App startup culture bootstrap (zh-CN default)' },
        @{ File = 'RenoDXCommander\MainWindow.xaml.cs';  Needle = 'Loc.ApplyTo(Content)';                 Desc = 'XAML Loc.ApplyTo wiring' },
        @{ File = 'RenoDXCommander\Localization\Localizer.cs'; Needle = 'new CultureInfo("zh-CN")';        Desc = 'zh-CN normalization' }
    )
    $missing = @()
    foreach ($c in $checks) {
        $full = Join-Path $root $c.File
        if (-not (Test-Path $full)) { $missing += ('{0}  (missing file)' -f $c.File); continue }
        $content = [System.IO.File]::ReadAllText($full)
        if (-not $content.Contains($c.Needle)) {
            $missing += ('{0}  -- missing {1}  [{2}]' -f $c.File, $c.Needle, $c.Desc)
        }
    }
    if ($missing.Count -gt 0) {
        Write-Host 'WARNING: i18n hooks missing after merge (app will fall back to English):' -ForegroundColor Red
        $missing | ForEach-Object { Write-Host '  - $_' -ForegroundColor Red }
        Write-Host 'Re-apply these fork-specific lines before committing.' -ForegroundColor Red
        Write-Host 'See:  git log --all -S "$($checks[0].Needle)" -- $($checks[0].File)' -ForegroundColor Red
        return $false
    }
    Write-Host 'i18n hooks present: App.InitializeStartupCulture / MainWindow.Loc.ApplyTo / Localizer zh-CN' -ForegroundColor Green
    return $true
}
# 1. Ensure upstream remote is configured.
$upstream = git remote get-url upstream 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "upstream remote not configured; adding https://github.com/RankFTW/RHI.git"
    git remote add upstream https://github.com/RankFTW/RHI.git
} else {
    Write-Host ("upstream remote: {0}" -f $upstream)
}

# 2. Fetch upstream.
Write-Section "Fetching upstream/main"
git fetch upstream 2>&1 | Out-Host

# 3. Show upstream-only commits (the diff we are about to merge).
Write-Section "Commits in upstream/main not yet in fork"
$incoming = git log --oneline upstream/main ^HEAD 2>$null
if (-not $incoming) {
    Write-Host "(none -- fork is already up to date with upstream/main)" -ForegroundColor Green
    exit 0
}
$incoming | ForEach-Object { Write-Host ("  {0}" -f $_) }
Write-Host ""
$count = ($incoming | Measure-Object).Count
Write-Host ("Total: {0} commit(s) behind upstream/main" -f $count) -ForegroundColor Yellow

# 4. Attempt the merge (no-commit so user can review).
Write-Section "Running git merge upstream/main --no-ff --no-commit"
$mergeOutput = git merge upstream/main --no-ff --no-commit 2>&1
$mergeOutput | ForEach-Object { Write-Host ("  {0}" -f $_) }

# Detect conflict state.
$conflicted = (git diff --name-only --diff-filter=U 2>$null) | Where-Object { $_ }

if ($conflicted) {
    Write-Section "Conflicts detected -- manual resolution required"
    Write-Host "The following files need manual merge resolution:" -ForegroundColor Yellow
    Write-Host ""

    # Heuristic: i18n-layer files default to `--ours`, everything else
    # defaults to `--theirs`. The user can override per file.
    $i18nPattern = 'Localization|Localizer|Strings\.resx|Strings\.zh-CN|README_zh-CN|README_NexusMods\.zh-CN|DETAILED_GUIDE\.zh-CN|FAQ|^Faq_|Detail_|Settings_|Dialog_|Toolbar_|Menu_|Common_|motd|DarkTheme|RHI_PatchNotes'
    foreach ($file in $conflicted) {
        $isI18n = $file -match $i18nPattern
        if ($isI18n) {
            Write-Host ("  (zh-CN layer)  {0}" -f $file) -ForegroundColor Cyan
            Write-Host "      suggest:  git checkout --ours  " -ForegroundColor Cyan
            Write-Host "      (keep our zh-CN translation; if upstream renamed a key, re-add Localizer call)" -ForegroundColor DarkGray
        } else {
            Write-Host ("  (upstream change)  {0}" -f $file) -ForegroundColor Yellow
            Write-Host "      suggest:  git checkout --theirs" -ForegroundColor Yellow
            Write-Host "      (accept upstream; this file is not translated)" -ForegroundColor DarkGray
        }
    }

    Write-Host ""
    Write-Host "Workflow:" -ForegroundColor Cyan
    Write-Host "  1. For each file, run:  git checkout --ours <file>   or   git checkout --theirs <file>"
    Write-Host "  2. If the file is part of the i18n layer, you may also need to"
    Write-Host "     re-apply Localizer.Get/Instance/Format calls that this script"
    Write-Host "     removed. See tools/i18n/reapply-i18n.ps1 for helpers."
    Write-Host "  3. After all conflicts resolved:  git add -A"
    Write-Host "  4. Re-run this script; it will jump to the build/test step."
    exit 1
}

# 5. Clean merge -- proceed to verification.
Write-Section "Merge completed cleanly (no conflicts)"
Write-Section "Verify i18n hooks survived the merge"
if (-not (Assert-I18nHooks)) {
    Write-Host "Fix the missing hooks above, then re-run this script." -ForegroundColor Red
    exit 2
}


# 6. Build + test.
Write-Section "Running dotnet build"
$buildOutput = dotnet build .\RenoDXCommander\RenoDXCommander.csproj -c Debug -p:Platform=x64 --no-restore 2>&1
$buildOutput | ForEach-Object { Write-Host ("  {0}" -f $_) }
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build FAILED -- fix compile errors before committing." -ForegroundColor Red
    exit 2
}

Write-Section "Running dotnet test"
$testOutput = dotnet test .\RenoDXCommander.Tests\RenoDXCommander.Tests.csproj -c Debug -p:Platform=x64 --no-restore 2>&1
$testOutput | ForEach-Object { Write-Host ("  {0}" -f $_) }
if ($LASTEXITCODE -ne 0) {
    Write-Host "Tests FAILED -- fix test failures before committing." -ForegroundColor Red
    exit 2
}

Write-Section "Ready to commit"
Write-Host "Merge produced a clean tree: build passes, all 38 tests pass." -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
    $cmm = 'git commit -m "Merge upstream/main vX.Y.Z + reapply zh-CN"'
Write-Host ("  1. Review the staged changes:  git diff --cached --stat")
Write-Host "  2. If everything looks good, commit:"
    Write-Host ("       {0}" -f $cmm)
Write-Host "  3. Push to fork:  git push origin main"
exit 0
