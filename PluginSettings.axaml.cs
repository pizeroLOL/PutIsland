using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared.Helpers;
using Microsoft.Extensions.Logging;

namespace PutIsland;

[SettingsPageInfo(
    "PutIsland.SettingsPage",
    "PutIsland",
    "\uF414",
    "\uF414"
)]
public partial class PluginSettings : SettingsPageBase {
    private readonly ILogger<PluginSettings> _logger =
        Utils.Logger().CreateLogger<PluginSettings>();

    private readonly string _path;

    public PluginSettings(Plugin plugin) {
        InitializeComponent();
        _path = Path.Combine(plugin.PluginConfigFolder, "settings.json");
        var settings =
            ConfigureFileHelper.LoadConfig<PluginSettingsModel>(_path);
        settings.PropertyChanged += (_, _) => {
            ConfigureFileHelper.SaveConfig(_path, DataContext);
            Depot.Instance.RestartServer(settings.Host, settings.Port);
            _logger.LogInformation($"设置 -> {settings.Host}:{settings.Port}");
        };
        DataContext = settings;
    }


    private void Control_OnUnloaded(object? sender, RoutedEventArgs e) {
        ConfigureFileHelper.SaveConfig(_path, DataContext);
    }
}