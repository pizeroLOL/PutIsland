using System.Threading.Channels;

namespace PutIsland;

public class Depot {
    private static readonly Lazy<Depot> Self = new(() => new Depot());

    // Channel 替代 EventHandler，支持多消费者
    // public readonly Channel<(string token, string reqText)> MessageChannel
    //     = Channel.CreateUnbounded<(string, string)>();

    // 追踪哪些 token 正在被监听（用于冲突检测）
    // public readonly ConcurrentDictionary<string, int> TokenListeners = new();
    public readonly GroupChannel GroupChannel;

    public readonly CancellationTokenSource TokenSource = new();

    private Depot() {
        var sender = Channel.CreateUnbounded<(string, string)>();
        GroupChannel = new GroupChannel(sender.Reader, TokenSource.Token);
        _ = new UseHttp(sender.Writer, TokenSource.Token);
    }

    public static Depot Instance => Self.Value;
}