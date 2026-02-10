using System.Threading.Channels;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;

namespace PutIsland;

[ComponentInfo(
    "f8412005-f577-4095-8431-194c8b4a02ea", // 这里需要替换成你生成的 GUID
    "网络文本"
)]
// ReSharper disable once ClassNeverInstantiated.Global
public partial class Text : ComponentBase<TextSettingsModel> {
    private readonly CancellationTokenSource _tokenSource = new();
    private Channel<(string key, string value)>? _channel;

    public Text() {
        InitializeComponent();
    }

    public new void Loaded(object? sender, RoutedEventArgs e) {
        _channel = Depot.Instance.GroupChannel.Register();
        var reader = _channel.Reader
            .ReadAllAsync(
                _tokenSource.Token);

        Task.Factory.StartNew(async () => {
            await foreach (var message in reader) {
                if (message.key != Settings.Token.Trim()) continue;

                await Dispatcher.UIThread.InvokeAsync(() => {
                    InnerText.Content = message.value;
                });
            }
        });
    }

    public new void Unloaded(object? sender, RoutedEventArgs e) {
        _tokenSource.Cancel();
        var token = Settings.Token.Trim();
        if (_channel != null)
            Depot.Instance.GroupChannel.Unregister(_channel);
    }
}