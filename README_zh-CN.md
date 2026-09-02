# RHI — 简化的 PC 游戏 HDR 管理

一个应用管理你的整个 PC 游戏库的 HDR 模组。RHI 自动识别来自 8 个商店的游戏，自动安装和更新 ReShade、RenoDX、帧率限制器、DLSS/Streamline、OptiScaler 等 — 全部按游戏粒度控制，零手动配置。

![RHI](screenshots/game_view.png)

> **⚠ 仅限单机使用。** RHI 安装带附加组件支持的 ReShade，可能在联机游戏中触发反外挂。请在进入多人游戏前卸载。

---

## 为什么选 RHI？

- **8 商店自动识别** — Steam、GOG、Epic、EA App、Ubisoft Connect、Xbox/Game Pass、Battle.net、Rockstar。无需手动设置。
- **10 个可管理组件** — ReShade、RenoDX、RenoDX Upgrade、ReLimiter、Display Commander、OptiScaler、RE Framework、Luma Framework、DXVK、DOF Fix。每个都支持一键安装、更新、卸载。
- **46 个着色器包** — RHI 内置、自定义、或关闭。全局或每游戏独立。按文件粒度勾选你想要的着色器，保存并分享命名的着色器配置 — 导出为 zip 分享到 Discord。
- **所有 DX11 UE 游戏的通用 Luma** — 库中每个 DX11 Unreal Engine 游戏都会出现 Luma 行。安装时自动应用游戏特定的 Engine.ini 调整和启动参数。
- **DLSS 与 Streamline 管理** — 独立切换 SR、光线重建与帧生成的任意版本。作为一个集合更新或回退 Streamline。无需 NVIDIA Profile Inspector 即可设置每游戏的 DLSS 预设。
- **Nvidia 驱动配置覆盖** — VSync、低延迟、Smooth Motion、电源模式、ReBAR、多帧生成、DLSS 渲染缩放比例（33–100%）。全部按游戏粒度，直接写入 NVIDIA 驱动配置。
- **批量部署** — 一次性更新多个游戏的 DLSS/Streamline 版本和预设。
- **DLSS/Streamline 默认值** — 配置首选版本、预设和渲染缩放比例。一键快速应用到任何游戏。
- **配置导出/导入** — 备份所有每游戏的 NVIDIA 配置到 JSON，驱动更新后可恢复。
- **拖放** — 拖入 exe、附加组件、预设、Luma 压缩包或 URL，RHI 自动识别。
- **两种视图模式** — 详情和紧凑。一键切换。
- **启动游戏** — Steam 使用 `-applaunch`（含覆盖层和游戏时长统计），Epic 使用协议，其他直接启动。每游戏可配置自定义 exe 和参数。
- **HDR 自动切换** — 启动游戏时自动启用 Windows HDR，退出时关闭。全局或每游戏设置。再也不用在 Windows 设置里手动切换。
- **运行中游戏指示** — 侧边栏在 RHI 启动的游戏运行时显示绿色高亮。
- **系统托盘与跳转列表** — 关闭时最小化到系统托盘。右键点击托盘或任务栏图标可快速启动最近游戏。
- **后台自动更新** — 运行期间每 4 小时重新检查所有模组和应用更新。
- **自定义附加组件** — 将 `.addon64`/`.addon32` 文件放入 Custom 文件夹，会出现在附加组件管理器中可开关切换。
- **数字鲜艳度** — 启动时自动恢复按显示器的颜色鲜艳度（0–100）。
- **DLSS/Streamline 自动更新** — 游戏已是最新版本时，新版本发布后自动替换。手动选择的老版本保留不动。
- **峰值亮度** — 设置一次显示器的峰值亮度（nit），每次部署时自动写入所有 reshade.ini。
- **Luma + RenoDX 共存** — 兼容的游戏可同时运行两个框架。
- **Ryubing 模拟器支持** — 来自 Souperman9 的 9 个 Switch 游戏附加组件，一键下载部署。附加组件自动识别当前运行的游戏。
- **RTX HDR 按游戏切换** — 为任何游戏启用 NVIDIA 驱动层 HDR 注入。可配置峰值亮度、对比度、鲜艳度、中间灰、去环。需要 NVIDIA App 启用覆盖层和游戏滤镜。
- **自定义 ReShade 自动重部署** — 更新 Custom 文件夹中的自定义 ReShade DLL 后，会自动重新部署到所有使用它的游戏。Refresh 和每 4 小时检查。
- **随 Windows 启动** — 启动时最小化到系统托盘。
- **UE-Extended 自动配置** — reshade.ini 的 `[renodx]` 段和 Engine.ini HDR 设置自动写入。
- **Nexus Mods 更新提醒** — GraphQL API，无需 API 密钥。
- **远程清单** — 无需应用更新即可推送游戏特定修复。
- **外来 DLL 保护** — 二进制签名扫描防止意外覆盖 DXVK、Special K、ENB 等。
- **附加组件文件监控** — 自动检测下载文件夹中的新附加组件并提示安装。
- **每日提示** — 有重要变化时启动后提示。

---
## 可管理的组件

| 组件 | 功能 |
|-----------|-------------|
| [ReShade](https://reshade.me) | 后处理注入框架。通道：Stable、Nightly、Custom（用户 DLL）、Legacy（锁定 6.0.0+）、No Addons。 |
| [RenoDX](https://github.com/clshortfuse/renodx) | HDR 模组框架。从 RenoDX Wiki 匹配游戏特定模组，含通用 Unreal/Unity/UE-Extended 回退。 |
| [ReLimiter](https://github.com/RankFTW/ReLimiter) | 帧率调节附加组件，含可配置的 OSD 快捷键和共享预设。 |
| [Display Commander](https://github.com/pmnoxx/display-commander) | 另一个帧率限制器。与 ReLimiter 互斥。 |
| [OptiScaler](https://github.com/optiscaler/OptiScaler) | 上采样重定向（DLSS ↔ FSR ↔ XeSS）和任意 GPU 上的帧生成。Stable 和 Nightly 通道。通过 ⚙ 齿轮提供每游戏 FG 设置、预设和 DLSS SR/RR/Render Scale 控件。 |
| [RE Framework](https://github.com/praydog/REFramework-nightly) | RE Engine 游戏上 ReShade 所需（Monster Hunter Wilds、Resident Evil、DMC5、SF6 等）。 |
| [Luma Framework](https://github.com/Filoppi/Luma-Framework) | 面向 DX11 游戏的 HDR 模组框架。支持命名模组和所有 DX11 Unreal Engine 游戏（通用）。在所有 Luma 游戏上由 RHI 管理 ReShade 和 DLSS。 |
| [DXVK](https://github.com/doitsujin/dxvk) | DX8–DX10 游戏的 DirectX-to-Vulkan 转换。变体：Development、Stable、Lilium HDR（scRGB 输出）。按游戏选择。 |
| [DOF Fix](https://github.com/RankFTW/rhi-repo/releases) | 修复 Unreal Engine 5.0–5.6 游戏的景深阶梯状色块和平铺色问题。一键安装，参与全部更新。 |
| [RenoDX Upgrade](https://github.com/OopyDoopy/renodx) | DX9+ 游戏的逆向 tone mapping 和资源升级。与 RenoFX 着色器配合使用以获得完整 HDR 转换。使用 RenoDX/Luma 模组时不需要。 |

---
## 主要特性

### DLSS 与 Streamline

- 独立切换 DLSS SR、光线重建与帧生成的任意版本
- 作为一个集合更新或回退 Streamline
- 每游戏 DLSS 预设 — SR：Default/J/K/L/M · RR：Default/D/E · FG：Default/A/B
- DLSS 渲染缩放比例覆盖：每游戏 SR 和 RR 都可设 33–100%
- 多帧生成（RTX 50 系列）：模式（Default/Fixed/Dynamic）、帧数（2x–6x）、目标 FPS
- 快速应用一键将你的默认配置部署到任何游戏
- 批量部署同时更新多个游戏的版本 + 预设
- 每游戏备份/恢复（`.original` 文件）

### Nvidia 驱动配置覆盖

全部按游戏独立，通过 NVIDIA 驱动配置。需要管理员权限（可用任务计划程序实现持久提权）。

- **VSync** — 模式 + 撕裂控制
- **低延迟** — Off / On / Ultra
- **Smooth Motion** — 启用 + API + 翻转节奏
- **电源模式** — 自适应 / 优先最高性能 / 最优
- **ReBAR** — 启用 / 模式 / 大小限制
- **FPS 限制** — VRR 优化预设或自定义值。在安装 ReLimiter/DC 时按游戏自动禁用。
- **G-Sync** — 按游戏禁用开关
- **配置导出/导入** — 备份所有设置到 JSON，驱动更新后恢复

### 全局 Nvidia 设置（设置页）

- Shader Cache 大小
- Shader 预编译
- G-Sync 模式
- 首选刷新率
- 全局 ReBAR（开/关 + 大小）
- DLSS 屏上指示
- FPS 限制（Frame Rate Limiter V3）
- G-Sync 启用
- G-Sync 屏上指示
- 数字鲜艳度（按显示器）
- DMFG 默认值（帧数 + 目标 FPS）
### 管理员模式

基于任务计划程序的持久提权。在设置中打开/关闭。启用后，RHI 会在启动时静默重新以管理员权限启动 — 无需每次操作都提示 UAC。ReBAR、低延迟（ULL）、Smooth Motion 写入需要此权限。管理员模式下 Drop Helper 可提供 Discord 拖放。

### 每游戏覆盖

DLL 命名 · 着色器模式（全局/自定义/选择/关闭）· 附加组件模式（全局/选择/关闭）· 位数 · 图形 API · ReShade 通道（Stable/Nightly/Custom/Legacy/No Addons）· DXVK 变体 · 启动参数 · 更新包含开关 · Wiki 名称映射 · HDR 自动切换 · G-Sync 禁用 · 保持 reshade.ini 更新（每游戏锁定，防止自动修改）

### DOF Fix

面向 Unreal Engine 5.0–5.6 发生景深阶梯状色块或平铺色问题的一键安装。参与全部更新。通过引擎版本自动检测资格，或通过清单强制启用。

### OptiScaler

上采样重定向和帧生成，Stable 和 Nightly 通道。⚙ 齿轮提供每游戏设置：FG Input/Output/Nvngx Override、HUD Fix、DLSS SR/RR 预设、渲染缩放、Streamline/DLSS Enabler 部署以及 4 个用户可配置的预设（Nightly）。Unreal Engine 游戏的 Engine.ini 调整。两个上采样下拉让你可以针对不同 API（DX11、DX12、Vulkan）独立选择上采样器，无需手动编辑 INI。

### HDR Gaming Database

RenoDX Info 按钮可直接跳转到支持游戏的 HDR Gaming Database 页面。

### 超宽修复与 Ultra+ 链接

在游戏卡片上可以看到超宽修复（Lyall、RoseTheFlower、p1xel8ted）和 Ultra+ 模组的快捷链接。

---
## 快速入门

1. **下载并运行 RHI** — 你的游戏库会自动出现。
2. **选择一个游戏** — 在侧边栏中选择。你可以搜索或使用筛选标签筛选。
3. **点击安装** — 选择你要的组件（ReShade、RenoDX、帧率限制器）。
4. **启动游戏** — 启动后按 **Home** 打开 ReShade，进入 **Add-ons** 配置 RenoDX。
5. **选择着色器** — 点击工具栏的 **着色器/附加组件** 按钮，选择全局着色器。展开包选择单个着色器文件，保存为命名配置，或导出以分享。

---

## 下载

从 [GitHub Releases 页面](https://github.com/RankFTW/RHI/releases) 获取最新版本。

**系统要求：**
- Windows 10/11 (x64)
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
- 推荐使用 NVIDIA GPU 运行 DLSS/Streamline 和驱动配置覆盖功能（AMD/Intel GPU 可用于其他全部功能）

---

## 故障排查

| 问题 | 解决 |
|---------|-----|
| 游戏未被识别 | **设置** 中 **Add Game** — 选择游戏的 exe 并命名 |
| Xbox 游戏缺失 | 点击 **Refresh** — Game Pass 检测可能需要片刻 |
| ReShade 未加载 | 通过 📁 检查安装路径 — DLL 必须放在游戏 exe 同目录 |
| 黑屏（Unreal） | ReShade → Add-ons → RenoDX → 将 `R10G10B10A2_UNORM` 设为 `output size` |
| UE-Extended 不工作 | 先在游戏的显示设置中启用 HDR |
| 下载失败 | 点击 **Refresh**，或在 设置 → 打开下载缓存 中清除缓存 |
| DLSS 预设未应用 | 在设置中启用管理员模式，或以管理员身份运行 RHI |
| 一切不同步 | 设置 → **Full Refresh** 清除所有缓存并重新扫描 |

完整的功能参考请查看 [详细指南](docs/DETAILED_GUIDE.md)。

---
## 第三方组件

| 组件 | 作者 | 许可 |
|-----------|--------|---------|
| [ReShade](https://reshade.me) | Crosire | [BSD 3-Clause](https://github.com/crosire/reshade/blob/main/LICENSE.md) |
| [RenoDX](https://github.com/clshortfuse/renodx) | clshortfuse & 贡献者 | [MIT](https://github.com/clshortfuse/renodx/blob/main/LICENSE) |
| [ReLimiter](https://github.com/RankFTW/ReLimiter) | RankFTW | Source-available |
| [Display Commander](https://github.com/pmnoxx/display-commander) | pmnoxx | [GPL-3](https://github.com/pmnoxx/display-commander/blob/main/LICENSE) |
| [RE Framework](https://github.com/praydog/REFramework-nightly) | praydog | [MIT](https://github.com/praydog/REFramework/blob/master/LICENSE) |
| [Luma Framework](https://github.com/Filoppi/Luma-Framework) | Pumbo (Filoppi) | Source-available |
| [OptiScaler](https://github.com/optiscaler/OptiScaler) | OptiScaler 贡献者 | Source-available |
| [DXVK](https://github.com/doitsujin/dxvk) | doitsujin & 贡献者 | [Zlib](https://github.com/doitsujin/dxvk/blob/master/LICENSE) |
| [DXVK HDR-mod](https://github.com/EndlesslyFlowering/dxvk) | EndlesslyFlowering (Lilium) | [Zlib](https://github.com/EndlesslyFlowering/dxvk/blob/HDR-mod/LICENSE) |
| [DOF Fix](https://github.com/RankFTW/rhi-repo/releases) | Lazorr | Source-available |
| [7-Zip](https://www.7-zip.org/) | Igor Pavlov | [LGPL-2.1 / BSD-3-Clause](https://www.7-zip.org/license.txt) |

> RHI 是非官方第三方工具，与 RenoDX 项目、Crosire 或 Luma Framework 无关或未经其认可。所有模组文件在运行时从其官方源下载，不重新分发。

---

## 致谢

没有整个 RenoDX 团队和 ReShade 的创造者 [Crosire](https://reshade.me) 的辛勤工作，RHI 不可能存在。感谢每一位推动 PC HDR 前进的模组作者、贡献者和测试者。

---

## 链接

[RenoDX](https://github.com/clshortfuse/renodx) · [RenoDX Wiki](https://github.com/clshortfuse/renodx/wiki/Mods) · [ReShade](https://reshade.me) · [Luma Framework](https://github.com/Filoppi/Luma-Framework) · [Luma Mods List](https://github.com/Filoppi/Luma-Framework/wiki/Mods-List) · [ReLimiter](https://github.com/RankFTW/ReLimiter) · [HDR Guides](https://www.hdrmods.com)

[RenoDX Discord](https://discord.gg/gF4GRJWZ2A) · [HDR Den Discord](https://discord.gg/k3cDruEQ) · [RHI 支持](https://discordapp.com/channels/1296187754979528747/1475173660686815374) · [Ultra+ Discord](https://discord.gg/pQtPYcdE)

[在 Ko-Fi 上支持 RHI ☕](https://ko-fi.com/rankftw)