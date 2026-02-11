using CommunityToolkit.Mvvm.ComponentModel;

namespace PutIsland;

public partial class NotificationSettingsModel : ObservableObject {
    [ObservableProperty] private string _token = "";
}