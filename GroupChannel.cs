using System.Threading.Channels;

namespace PutIsland;

public class GroupChannel {
    private readonly List<Channel<(string key, string value)>>
        _listeners = new();

    public GroupChannel(ChannelReader<(string key, string value)> sender,
        CancellationToken token) {
        Task.Factory.StartNew(async () => {
            await foreach (var msg in sender.ReadAllAsync()) {
                Channel<(string key, string value)>[] snapshot;
                lock (_listeners) {
                    snapshot = _listeners.ToArray();
                }

                foreach (var ch in snapshot)
                    await ch.Writer.WriteAsync(msg, token);
            }
        });
    }

    public Channel<(string key, string value)> Register() {
        var channel = Channel.CreateUnbounded<(string key, string value)>();
        lock (_listeners) {
            _listeners.Add(channel);
        }

        return channel;
    }

    public void Unregister(Channel<(string key, string value)> channel) {
        lock (_listeners) {
            _listeners.Remove(channel);
        }
    }
}