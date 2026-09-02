# RHI — 详细指南（中文）

本指南为 RHI 完整功能参考。为了便于中文用户阅读，前 5 节已翻译为简体中文；其余章节（DLSS 管理、ReShade 通道、Per-Game Overrides、Shaders 等等）请参考 [英文完整版](DETAILED_GUIDE.md)。

如果英文版有更新但中文版未同步，可以参考 [GitHub 上的 RHI 仓库](https://github.com/RankFTW/RHI) 查看最新英文内容。

---

## 目录

- [布局与视图](#布局与视图)
- [设置页](#设置页)
- [游戏检测](#游戏检测)
- [图形 API 检测](#图形-api-检测)
- [组件](#组件)

> 以下章节为完整翻译。再往下会标注"（英文）"的章节请直接查看英文版。

---

## 布局与视图

RHI 提供两种主视图，可通过工具栏的 **Views** 按钮或左下角齿轮切换。

### 详情视图

左侧是游戏列表侧边栏，右侧是多区段的详情面板。选择一个游戏后会显示四个区段：

1. **组件** — 游戏信息徽章，每个组件的安装/更新/卸载按钮
2. **游戏覆盖** — 每游戏设置（DLL 命名、着色器、附加组件、DXVK、RS 通道、位数、API）
3. **Nvidia 配置覆盖** — DLSS/Streamline 管理和驱动配置（VSync、延迟、Smooth Motion、电源、ReBAR）
4. **管理** — 更改安装目录、重置覆盖、复制诊断报告

### 紧凑视图

分页布局，与详情视图内容相同，分为三页：
- **第 1 页** — 组件（游戏信息、安装按钮）
- **第 2 页** — 游戏覆盖
- **第 3 页** — Nvidia 配置覆盖 + 管理

使用两侧的箭头按钮在页面间循环。紧凑视图下窗口锁定为固定紧凑尺寸，切换回详情视图时恢复原尺寸。

### 工具栏

| 按钮 | 作用 |
|--------|-------------|
| Refresh | 重新扫描你的游戏库，并从所有源获取最新模组信息。 |
| Shaders/Addons | 下拉：Global Shaders（选择着色器包）和 ReShade Addons（管理附加组件开关）。 |
| Update All | 跨所有有资格的游戏更新 ReShade、RenoDX、ReLimiter、Display Commander、OptiScaler 和 RE Framework。有可用更新时按钮变紫。 |
| Links | 下拉快速链接：RenoDX Wiki、Luma Wiki、RHI GitHub、ReLimiter GitHub、Display Commander GitHub。 |
| Help | 下拉：Discord 支持频道、本指南、Kofi、About 页面。 |
| Views | 切换按钮：在详情和紧凑视图之间切换。 |
| Settings | 打开设置页。 |

### 侧边栏（详情视图）

- **搜索框** — 实时过滤游戏。匹配游戏名、商店、引擎、图形 API、位数、模组名、模组作者等。输入 "UW Fix" 或 "Ultra+" 即可过滤出有这些链接的游戏。
- **筛选标签** — All Games、Favourites、Installed、Unreal、Unity、Other、RenoDX、Luma、Hidden。你选择的筛选会在下次打开时保留。
- **自定义筛选标签** — 点击搜索框旁的 "+" 按钮可将任意搜索词保存为命名标签。自定义标签使用青绿色配色方案。右键删除。
- **游戏/已安装计数** — 显示可见游戏数以及已安装模组的数量。
- **游戏条目** — 每行显示平台图标、游戏名，有可用更新时显示绿色圆点。

### 详情面板

选择一个游戏后，详情面板显示：

- **头部** — 游戏名、Launch 按钮（绿色 ▶）、Luma 开关、模组作者徽章（如有可链接到 Ko-fi）。
- **信息卡** — 操作按钮（Nexus Mods、PCGW、UW Fix、Ultra+ 在左侧；Hide、Favourite 在右侧）以及平台、引擎、wiki 状态、图形 API、UE-Extended/原生 HDR 和位数的徽章。
- **安装路径** — 等宽字体显示解析后的游戏目录。
- **组件区段** — 每个组件的安装/更新/卸载按钮，带每个附加组件的 Info 按钮。
- **游戏覆盖区段** — 所有每游戏设置都内联显示。
- **Nvidia 配置覆盖区段** — DLSS/Streamline 行和驱动设置行。
- **管理区段** — 更改安装目录、重置目录、重置覆盖、复制报告。

### 状态栏

底部栏在左侧显示游戏数和当前操作，中心显示单机警告，右侧显示应用版本号和 Patch Notes 链接。

---

## 设置页

点击工具栏的 **Settings**。点击 **Back to Games** 返回。该页由 9 个带标签的卡片组成。

| 卡片 | 内容 |
|------|-------------|
| Game Library | Add Game（手动检测）、Check For Updates（绕过 4 小时冷却）。 |
| ReShade & Display | 左：截图路径 + 子文件夹组合 + 快捷键 + Apply to All。右：峰值亮度（Auto + nits）+ HDR 自动切换（Off/On）+ Apply to All。 |
| DLSS / Streamline Settings | Batch Deploy、Configure Defaults（版本/预设/渲染缩放）、On-Screen Indicator（Enabled/Disabled）、Auto-Update DLSS（Off/On）、Auto-Update Streamline（Off/On）。 |
| Global NVIDIA Driver Settings | Shader Cache Size、Shader Pre-Compile、G-Sync Mode、Preferred Refresh Rate、VSync、ReBAR + Size Limit、Export/Import/Reset Profiles、Clear Shader Cache。 |
| Component Settings | 左：ReLimiter OSD 快捷键 + Shared Presets + DLSS Hooks + Apply。右：OptiScaler GPU 类型 + DLSS inputs + 快捷键 + Apply。 |
| Shaders & Addons | Custom Shaders 开关、Cache All Shaders 开关、Addon Watch Folder + Browse + Reset。 |
| Update & Deployment | Global Update Inclusion（每组件开关）、Mass INI Deployment（reshade.ini、relimiter.ini、DC.ini、OptiScaler.ini、Mass Preset Install）。 |
| System & Maintenance | Full Refresh、Purge Cache、Admin Mode（Off/On）、Drop Helper（Off/On — 禁用 Discord 拖放覆盖层，需重启）。 |
| Data & Folders | AppData Folder、Downloads Folder、Custom Folder、Logs Folder、Copy Logs。 |

---

## 游戏检测

RHI 在每次启动时扫描所有支持的商店，并将新安装的游戏合并到其缓存的游戏库中。位于未连接驱动器上的游戏会在缓存中保留，直到重新连接。每个商店的检测失败是隔离的 — 一个商店失败不会阻塞其他商店。

### 支持的商店

| 商店 | RHI 如何查找游戏 |
|-------|-------------------|
| Steam | 读取所有库文件夹下的 `libraryfolders.vdf` 和 `appmanifest_*.acf` 文件。 |
| GOG | `HKLM\SOFTWARE\GOG.com\Games` 下的注册表项。 |
| Epic Games | `ProgramData\Epic\EpicGamesLauncher\Data\Manifests` 下的清单 `.item` 文件。 |
| EA App | `installerdata.xml` 清单、注册表项、默认 EA Games 文件夹、EA Desktop 配置。 |
| Ubisoft Connect | `HKLM\SOFTWARE\Ubisoft\Launcher\Installs` 下的注册表项、`settings.yml`、默认文件夹。 |
| Xbox / Game Pass | Windows `PackageManager` API，结合 `MicrosoftGame.config` 检测。回退到 `.GamingRoot`、注册表和文件夹扫描。 |
| Battle.net | 卸载注册表项、`Battle.net.config`、默认文件夹扫描。 |
| Rockstar Games | 卸载注册表项、启动器 `titles.dat`、默认文件夹扫描。 |

### 引擎检测

| 引擎 | 检测方式 |
|--------|------------------|
| Unreal Engine | Unreal 特有文件和目录结构。版本在可用时从 CrashReportClient 或 Build.version 检测。 |
| Unreal (Legacy) | 通过遗留目录布局识别 Unreal Engine 3 游戏。 |
| Unity | `UnityPlayer.dll`、`Mono`、`MonoBleedingEdge`、`il2cpp`、`GameAssembly.dll`。 |
| RE Engine | 游戏目录中的 `re_chunk_000.pak`。 |
| Custom | 来自远程清单的引擎名称（如 "Silk Engine"、"Frostbite"）。 |

### 手动添加游戏

- **Add Game**（设置页）— 点击按钮，选择游戏的 exe，然后命名。
- **拖放** — 将游戏的 `.exe` 拖到 RHI 窗口。引擎和游戏根目录会自动检测。

### 多游戏分割

一个文件夹中包含多个游戏的游戏（如 Mass Effect Legendary Edition）会通过远程清单分割为独立条目。每个子游戏获得独立的模组管理。

---

## 图形 API 检测

RHI 使用 PE 头导入表分析扫描游戏可执行文件。结果会缓存到磁盘。

| API | 徽章 | RHI 检查什么 |
|-----|-------|--------------------|
| DirectX 8 | DX8 | `d3d8.dll` 导入 |
| DirectX 9 | DX9 | `d3d9.dll` 导入 |
| DirectX 10 | DX10 | `d3d10.dll` / `d3d10_1.dll` 导入 |
| DirectX 11/12 | DX11/12 | `d3d11.dll` / `d3d12.dll` 导入 |
| Vulkan | VLK | `vulkan-1.dll` 导入 |
| OpenGL | OGL | `opengl32.dll` 导入 |

检测到的 API 驱动自动 ReShade DLL 命名：
- DX9 → `d3d9.dll`
- OpenGL → `opengl32.dll`
- 默认 → `dxgi.dll`

双 API 游戏同时显示两个 API（如 `DX11/12 / VLK`）。对于 PE 检测失败的游戏，可使用每游戏 API 覆盖和清单覆盖。

---

## 组件

详情面板的组件区段为每个受管模组显示一行：

| 组件 | 描述 |
|-----------|-------------|
| RE Framework | RE Engine 游戏所需。仅对这些游戏显示。 |
| ReShade | 核心注入框架。安装/重新安装/更新、Copy INI、卸载。 |
| RenoDX | HDR 模组附加组件。安装/更新、Info、卸载。 |
| Luma | Luma Framework。仅在 Luma 模式下显示。 |
| ReLimiter | 帧率调节附加组件。安装、Info、Copy INI、卸载。 |
| Display Commander | 替代帧率限制器。安装、Info、Copy INI、卸载。 |
| OptiScaler | 上采样重定向。安装、Info、Copy INI、卸载。 |
| DXVK | DirectX-to-Vulkan。通过 Overrides 中的 DXVK 下拉管理。 |

### 每附加组件 Info 按钮

每个组件都有一个 **Info** 按钮，显示游戏特定上下文：

1. **清单注释** — 来自远程清单的游戏特定注释
2. **Wiki 内容** — 来自相关 wiki 的兼容性数据
3. **通用描述** — 附加组件的一般功能

Info 按钮在有内容时以**蓝色**高亮。ReLimiter 和 Display Commander 还会显示更新日志。

### 版本显示

每个组件显示其安装的版本号。紫色文字表示有可用更新。

### 依赖强制

- **需要 ReShade** — RenoDX、ReLimiter、Display Commander 需要先安装 ReShade
- **需要 RE Framework** — RE Engine 游戏需要 RE Framework 然后才能装 ReShade

---

## 后续章节（英文）

以下章节请参考 [完整英文版](DETAILED_GUIDE.md)：

- [ReShade](DETAILED_GUIDE.md#reshade) — 详细功能、通道、INI、Keep ReShade.ini Updated
- [RenoDX](DETAILED_GUIDE.md#renodx) — 命名模组、UE-Extended、Unity、RTX HDR
- [RE Framework](DETAILED_GUIDE.md#re-framework) — RE Engine 模组
- [Luma Framework](DETAILED_GUIDE.md#luma-framework) — Luma 模式、Generic Luma
- [UE-Extended Auto-Configuration](DETAILED_GUIDE.md#ue-extended-auto-configuration) — Engine.ini 调整
- [Ryubing Emulator Support](DETAILED_GUIDE.md#ryubing-emulator-support) — Switch 模拟器
- [Frame Rate Limiters](DETAILED_GUIDE.md#frame-rate-limiters) — ReLimiter / Display Commander 详细
- [DLSS & Streamline Manager](DETAILED_GUIDE.md#dlss--streamline-manager) — 版本、预设、备份/恢复
- [DLSS & Streamline Defaults](DETAILED_GUIDE.md#dlss--streamline-defaults) — 默认配置
- [Nvidia Profile Overrides](DETAILED_GUIDE.md#nvidia-profile-overrides) — 驱动配置详情
- [Multi Frame Generation](DETAILED_GUIDE.md#multi-frame-generation) — MFG 设置
- [Global Nvidia Settings](DETAILED_GUIDE.md#global-nvidia-settings) — 全局驱动设置
- [Digital Vibrance](DETAILED_GUIDE.md#digital-vibrance) — 颜色鲜艳度
- [Admin Mode](DETAILED_GUIDE.md#admin-mode) — 任务计划程序提权
- [Profile Export and Import](DETAILED_GUIDE.md#profile-export-and-import) — 备份/恢复
- [OptiScaler](DETAILED_GUIDE.md#optiscaler) — 上采样器和帧生成
- [Shader Packs](DETAILED_GUIDE.md#shader-packs) — 着色器包管理
- [ReShade Addon Management](DETAILED_GUIDE.md#reshade-addon-management) — 附加组件
- [Game Launch](DETAILED_GUIDE.md#game-launch) — Steam/Epic/自定义启动
- [System Tray & Jump List](DETAILED_GUIDE.md#system-tray--jump-list) — 系统托盘
- [Per-Game Overrides](DETAILED_GUIDE.md#per-game-overrides) — DLL 命名、DXVK 变体
- [ReShade Presets](DETAILED_GUIDE.md#reshade-presets) — 预设管理
- [Nexus Mods and PCGamingWiki Links](DETAILED_GUIDE.md#nexus-mods-and-pcgamingwiki-links) — 信息链接
- [UW Fix and Ultra+ Links](DETAILED_GUIDE.md#uw-fix-and-ultra-links) — 超宽修复
- [Vulkan ReShade Support](DETAILED_GUIDE.md#vulkan-reshade-support) — Vulkan 全局层
- [DXVK](DETAILED_GUIDE.md#dxvk) — DirectX-to-Vulkan
- [Foreign DLL Protection](DETAILED_GUIDE.md#foreign-dll-protection) — 签名扫描
- [Drag-and-Drop](DETAILED_GUIDE.md#drag-and-drop) — 拖放功能
- [Addon Auto-Detection](DETAILED_GUIDE.md#addon-auto-detection) — 自动检测
- [Update All](DETAILED_GUIDE.md#update-all) — 批量更新
- [Auto-Update](DETAILED_GUIDE.md#auto-update) — 后台自动更新
- [Remote Manifest](DETAILED_GUIDE.md#remote-manifest) — 远程清单
- [Message of the Day](DETAILED_GUIDE.md#message-of-the-day) — 每日提示
- [Performance](DETAILED_GUIDE.md#performance) — 性能
- [Data Storage](DETAILED_GUIDE.md#data-storage) — 数据存储
- [Troubleshooting](DETAILED_GUIDE.md#troubleshooting) — 故障排查
- [Third-Party Components](DETAILED_GUIDE.md#third-party-components) — 第三方组件

如需翻译其中某几节，可在 [GitHub 仓库](https://github.com/RankFTW/RHI) 提 Issue 或自行 PR。