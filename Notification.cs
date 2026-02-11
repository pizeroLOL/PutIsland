using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Services.NotificationProviders;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Models.Notification;

namespace PutIsland;

[NotificationProviderInfo(
    "9DD294B2-2151-410C-96D4-DE32F0ABD17D",
    "网络通知",
    "\uF414",
    "允许外部应用通过 post http://localhost:36000/<token> 发送通知"
)]
public class
    Notification : NotificationProviderBase<NotificationSettingsModel> {
    public Notification() {
        var link = Depot.Instance.GroupChannel.Register().Reader
            .ReadAllAsync(Depot.Instance.TokenSource.Token);
        Task.Factory.StartNew(async () => {
            await foreach (var msg in link) {
                if (msg.key != Settings.Token) {
                    continue;
                }
                await Dispatcher.UIThread.InvokeAsync(() => {
                    ShowNotification(new NotificationRequest() {
                        MaskContent = NotificationContent.CreateTwoIconsMask(msg.value)
                    });
                });
            }
        }, Depot.Instance.TokenSource.Token);
    }
}