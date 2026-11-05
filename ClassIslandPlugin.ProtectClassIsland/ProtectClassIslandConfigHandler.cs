using System.ComponentModel;
using ClassIsland.Shared.Helpers;
using ClassIslandPlugin.ProtectClassIsland.Configuration;
using Microsoft.Extensions.Logging;

namespace ClassIslandPlugin.ProtectClassIsland;

/// <summary>
///     插件配置处理器：配置对象在构造时经宿主 <see cref="ConfigureFileHelper"/>
///     从插件配置目录加载（自动备份/回退），属性变更时自动写盘，
///     行为与主程序 <c>SettingsService</c> 的配置保存保持同构。
/// </summary>
public sealed class ProtectClassIslandConfigHandler : IDisposable
{
    private readonly ILogger<ProtectClassIslandConfigHandler> _logger;

    public ProtectClassIslandConfigHandler(
        ILogger<ProtectClassIslandConfigHandler> logger, string configFilePath)
    {
        _logger = logger;
        ConfigFilePath = configFilePath;
        Data = ConfigureFileHelper.LoadConfig<ProtectClassIslandConfig>(configFilePath);
        Data.PropertyChanged += Data_OnPropertyChanged;
    }

    /// <summary>配置文件完整路径（插件配置目录下的 ProtectClassIsland.json）。</summary>
    public string ConfigFilePath { get; }

    /// <summary>插件配置对象；属性变更经 <see cref="ConfigureFileHelper"/> 自动保存。</summary>
    public ProtectClassIslandConfig Data { get; }

    private void Data_OnPropertyChanged(object? sender, PropertyChangedEventArgs e) => Save();

    /// <summary>立即把当前配置写入磁盘（原子安全写）。</summary>
    public void Save()
    {
        try
        {
            ConfigureFileHelper.SaveConfig(ConfigFilePath, Data);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "保存 ProtectClassIsland 配置失败（{}）。", ConfigFilePath);
        }
    }

    public void Dispose() => Data.PropertyChanged -= Data_OnPropertyChanged;
}
