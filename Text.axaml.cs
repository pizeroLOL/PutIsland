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
    private bool _registed = true;

    public Text() {
        InitializeComponent();
    }

    public new void Loaded(object? sender, RoutedEventArgs e) {
        // TODO: 使用设置
        var token = "123";
        if (token == "") {
            InnerText.Content = "Token 为空";
            return;
        }

        if (Depot.Instance.RegisterToken(token)) {
            InnerText.Content = $"<{token}> 冲突";
            Depot.Instance.UnregisterToken(token);
            _registed = false;
            return;
        }

        InnerText.Content = $"<{token}>";
        var channel =
            Depot.Instance.MessageChannel.Reader.ReadAllAsync(
                _tokenSource.Token);
        Task.Factory.StartNew(async () => {
            await foreach (var message in channel) {
                Console.WriteLine(message);
                if (message.token != token) continue;

                await Dispatcher.UIThread.InvokeAsync(() => {
                    // 再次检查冲突（可能动态注册）
                    InnerText.Content =
                        Depot.Instance.TokenListeners.GetValueOrDefault(token) >
                        1
                            ? $"<{token}>！冲突"
                            : message.reqText;
                });
            }
        });
    }

    public new void Unloaded(object? sender, RoutedEventArgs e) {
        _tokenSource.Cancel();
        var token = "123";
        if (_registed && token == "") Depot.Instance.UnregisterToken(token);
    }
}