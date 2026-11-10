# ClassIslandPlugin.ProtectClassIsland

ProtectClassIsland 窗口保护 —— 一个 [ClassIsland](https://github.com/ClassIsland/ClassIsland) 插件，移植自 [SecRandom](https://github.com/SECTL/SecRandom) 的 `SecRandom.Plugin.HideSecRandom` 插件（未包含其中的“防止窗口被捕获”功能），用于对抗弹窗拦截程序对 ClassIsland 窗口的探测与强制关闭。

## 功能

- **随机窗口名称**：为窗口设置随机名称。无边框弹出窗口（如 ClassIsland 主窗口）始终随机化（仅系统 API 可见）；开启「扩展到所有窗口」后，带标题栏窗口的可见标题栏文字也会随机化。标题内容来源可在「窗口问候与嘲讽」中自定义。
- **窗口问候与嘲讽**：将随机窗口标题的内容来源替换为自定义句子（用于向弹窗拦截程序显示问候或嘲讽信息），并可叠加随机前缀、随机后缀与随机插入字母，避免句子被精确匹配。
- **随机窗口大小**：每个窗口打开时随机增大尺寸（默认关闭）：原本长宽不等的窗口，长、宽各随机增大 1-4px；原本长宽相等的窗口，长、宽同步随机增大相同的 1-4px。
- **拦截 WM_CLOSE 关闭请求**：丢弃外部发送的 WM_CLOSE 消息，使弹窗拦截程序无法借此关闭窗口；通过界面按钮或 Alt+F4 的正常关闭不受影响。
- **拦截强制关窗动作**：针对弹窗拦截程序的强制关窗动作链——拒绝其隐藏标志（SWP_HIDEWINDOW）、拒绝压底降级（HWND_BOTTOM）、拒绝屏幕内的位置与尺寸改动，仅允许“移动到屏幕外”这一步，以避免拦截程序触发后备的进程结束逻辑（TerminateProcess）。同时吞掉外部投递的 WM_SYSCOMMAND（SC_CLOSE）与诱导自隐藏的 WM_SHOWWINDOW。
- **层级与显示保护**：监听窗口层级与状态变化：弹窗被其他窗口覆盖时立即移回顶层；Topmost 置顶层带被剥夺时立即恢复；窗口被外部隐藏、最小化、禁用或移出屏幕时延迟恢复（以软件自身的显示意图为准，不影响正常的最小化与后台驻留）。
- **弹窗工具窗口伪装**：将无边框弹窗标记为 WS_EX_TOOLWINDOW 工具窗口：弹窗拦截程序不会对工具窗口显示“确认拦截”提示条，弹窗同时从 Alt+Tab 中隐藏。
- **原生弹窗保护**（独立开关）：下拉框（ComboBox）、Flyout、右键菜单等弹出层宿主在 Avalonia Window 集合之外的本进程 WS_POPUP 顶层窗口（如 PhonyRootWindow）中，本插件同样对其应用 WM_CLOSE 拦截、强制关窗对抗与工具窗口伪装。事件驱动实现：弹窗创建/显示时即时挂钩、销毁时释放，仅在开启时做一次种子扫描，无周期扫描开销。

> 注：原插件（SecRandom.Plugin.HideSecRandom）中的「防止窗口被捕获」（SetWindowDisplayAffinity）功能**未**移植到本插件。

所有改动在关闭对应开关或插件停用后自动还原。插件自身的显示、隐藏与布局操作不受任何影响。

## 配置

配置保存在宿主为插件创建的配置目录（`PluginConfigFolder`）下的 `ProtectClassIsland.json`，JSON 风格与 ClassIsland 主程序一致。

## 构建

```powershell
dotnet build -c Release
```

构建产物在 `bin/Release/net8.0/`，并会在 `cipx/` 下生成可直接安装的 `.cipx` 插件包（需要 PowerShell 7 / `pwsh` 生成 MD5 摘要）。

## 鸣谢

- [SecRandom.Plugin.HideSecRandom](https://github.com/SECTL/SecRandom)（SecRandom 的窗口保护插件）—— 本插件的窗口特性引擎移植自该项目。
- [ClassIsland](https://github.com/ClassIsland/ClassIsland) 及其插件 SDK。
