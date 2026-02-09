using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PutIsland;

[PluginEntrance]
public class Plugin : PluginBase {
    public override void Initialize(HostBuilderContext context,
        IServiceCollection services) {
        services.AddComponent<Text, TextSettings>();
        _ = Depot.Instance;
        // services.is
        AppBase.Current.AppStarted += (_, _) => {
            Console.WriteLine("PutIsland initialized");
        };
        AppBase.Current.AppStopping += (_, _) => {
            Depot.Instance.TokenSource.Cancel();
        };
    }
}