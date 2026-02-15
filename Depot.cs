using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace PutIsland;

public class Depot {
    private static readonly Lazy<Depot> Self = new(() => new Depot());

    private readonly ILogger<Depot> _logger =
        Utils.Logger().CreateLogger<Depot>();

    private readonly Channel<(string, string)> _msgBus =
        Channel.CreateUnbounded<(string, string)>();

    public readonly GroupChannel GroupChannel;
    public readonly CancellationTokenSource PluginToken = new();

    private CancellationTokenSource _serverToken = new();

    private Depot() {
        GroupChannel = new GroupChannel(_msgBus.Reader, PluginToken.Token);
        PluginToken.Token.Register(() => {
            _serverToken.Cancel();
            _serverToken.Dispose();
        });
    }

    public static Depot Instance => Self.Value;

    public void RestartServer(string host, ushort port) {
        _logger.LogInformation($"开始重启服务器 -> {host}:{port}");
        _serverToken.Cancel();
        _serverToken.Dispose();
        _serverToken = new CancellationTokenSource();
        UseHttp.StartServer(_msgBus.Writer, _logger, _serverToken.Token, host,
            port);
        _logger.LogInformation($"结束重启服务器 -> {host}:{port}");
    }
}