using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PutIsland;

[PluginEntrance]
public class Plugin : PluginBase {
    private readonly ILogger<Plugin> _logger =
        Utils.Logger().CreateLogger<Plugin>();

    public override void Initialize(HostBuilderContext context,
        IServiceCollection services) {
        _ = Depot.Instance;
        services.AddComponent<Text, TextSettings>();
        services.AddNotificationProvider<Notification, NotificationSettings>();
        services.AddSettingsPage<PluginSettings>();
        // services.is
        AppBase.Current.AppStarted += (_, _) => {
            _logger.LogInformation("PutIsland is started");
            var settings =
                ConfigureFileHelper.LoadConfig<PluginSettingsModel>(
                    Path.Combine(PluginConfigFolder, "settings.json"));
            Depot.Instance.RestartServer(settings.Host, settings.Port);
        };
        AppBase.Current.AppStopping += (_, _) => {
            Depot.Instance.PluginToken.Cancel();
            Depot.Instance.PluginToken.Dispose();
            _logger.LogInformation("PutIsland is stoped");
        };
    }
}