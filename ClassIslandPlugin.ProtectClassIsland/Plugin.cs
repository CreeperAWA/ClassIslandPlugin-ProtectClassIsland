using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared;
using ClassIslandPlugin.ProtectClassIsland.Services;
using ClassIslandPlugin.ProtectClassIsland.Views.SettingsPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClassIslandPlugin.ProtectClassIsland;

/// <summary>
///     ProtectClassIsland 插件入口：注册配置处理器、窗口特性引擎与插件设置页。
///     窗口特性的运行时逻辑在宿主 <see cref="AppBase.AppStarted"/> 之后启动
///     （此时 Host 与窗口均已就绪），并在 <see cref="AppBase.AppStopping"/> 时停止。
/// </summary>
[PluginEntrance]
public class Plugin : PluginBase
{
    private WindowFeatureService? _features;
    private EventHandler? _onAppStarted;
    private EventHandler? _onAppStopping;

    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        // 按官方开发文档建议，配置文件存放在 data/config/plugins/<插件Id> 目录下。
        var configFolder = Path.Combine(
            CommonDirectories.AppRootFolderPath, "data", "config", "plugins", Info.Manifest.Id);
        Directory.CreateDirectory(configFolder);
        // 处理器用工厂注册，保证在 Host 构建完成后（ConfigureFileHelper 日志可用时）才加载。
        services.AddSingleton(sp => new ProtectClassIslandConfigHandler(
            sp.GetRequiredService<ILogger<ProtectClassIslandConfigHandler>>(),
            Path.Combine(configFolder, "ProtectClassIsland.json")));
        services.AddSingleton<WindowFeatureService>();
        services.AddSettingsPage<ProtectClassIslandSettingsPage>();

        // ClassIsland 的 PluginBase 没有 SecRandom SDK 那样的 OnAppStarted/OnAppStopping
        // 生命周期钩子，改为订阅宿主 AppBase 的应用生命周期事件。
        _onAppStarted = (_, _) => OnAppStarted();
        _onAppStopping = (_, _) => OnAppStopping();
        AppBase.Current.AppStarted += _onAppStarted;
        AppBase.Current.AppStopping += _onAppStopping;
    }

    private void OnAppStarted()
    {
        if (!OperatingSystem.IsWindows())
            return; // 插件特性依赖 Win32 API，仅 Windows 端启用。

        _features = IAppHost.GetService<WindowFeatureService>();
        _features.Start();
    }

    private void OnAppStopping()
    {
        // 退出前放行一切关闭请求并移除钩子。
        _features?.Stop();
        if (AppBase.Current is not null)
        {
            AppBase.Current.AppStarted -= _onAppStarted;
            AppBase.Current.AppStopping -= _onAppStopping;
            _onAppStarted = null;
            _onAppStopping = null;
        }
    }
}
