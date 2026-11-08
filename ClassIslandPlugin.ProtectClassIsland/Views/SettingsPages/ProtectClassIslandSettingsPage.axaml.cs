using Avalonia.Controls;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;
using ClassIslandPlugin.ProtectClassIsland.Configuration;

namespace ClassIslandPlugin.ProtectClassIsland.Views.SettingsPages;

[SettingsPageInfo("classislandplugin.protectclassisland.settings", "ProtectClassIsland 窗口保护", "\ue816", "\ue816")]
public partial class ProtectClassIslandSettingsPage : SettingsPageBase
{
    private readonly ProtectClassIslandConfigHandler _configHandler;

    public ProtectClassIslandSettingsPage()
    {
        // 页面由宿主在运行时创建，此时 Host 已构建，按宿主视图约定用 IAppHost 解析服务。
        _configHandler = IAppHost.GetService<ProtectClassIslandConfigHandler>();
        InitializeComponent();
        GreetingSentencesBox.Text = string.Join("\n", _configHandler.Data.GreetingSentences);
        DataContext = this;
    }

    /// <summary>插件配置对象；属性变更经配置处理器自动保存。</summary>
    public ProtectClassIslandConfig Settings => _configHandler.Data;

    /// <summary>
    ///     保存自定义句子（一行一句）：失焦时才写回，避免每次按键都写盘；
    ///     整体替换列表以触发配置自动保存，并让引擎在下一轮刷新已打开窗口的标题。
    /// </summary>
    private void GreetingSentencesBox_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var sentences = GreetingSentencesBox.Text?
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList() ?? [];

        if (!sentences.SequenceEqual(Settings.GreetingSentences, StringComparer.Ordinal))
            Settings.GreetingSentences = sentences;
    }
}
