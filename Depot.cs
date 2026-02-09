using System.Collections.Concurrent;
using System.Threading.Channels;

namespace PutIsland;

public class Depot {
    private static readonly Lazy<Depot> Self = new(() => new Depot());

    // Channel 替代 EventHandler，支持多消费者
    public readonly Channel<(string token, string reqText)> MessageChannel
        = Channel.CreateUnbounded<(string, string)>();

    // 追踪哪些 token 正在被监听（用于冲突检测）
    public readonly ConcurrentDictionary<string, int> TokenListeners = new();

    public readonly CancellationTokenSource TokenSource = new();

    private Depot() {
        _ = new UseHttp(MessageChannel.Writer, TokenSource.Token);
    }

    public static Depot Instance => Self.Value;

    // 注册/注销监听，返回是否冲突
    public bool RegisterToken(string token) {
        var count = TokenListeners.AddOrUpdate(token, 1, (_, c) => c + 1);
        return count > 1; // true = 冲突
    }

    public void UnregisterToken(string token) {
        TokenListeners.AddOrUpdate(token, 0, (_, c) => c - 1);
        TokenListeners.TryRemove(token, out _); // 清理0值
    }
}