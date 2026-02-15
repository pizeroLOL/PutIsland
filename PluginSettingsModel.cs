using CommunityToolkit.Mvvm.ComponentModel;

namespace PutIsland;

public partial class PluginSettingsModel : ObservableObject {
    [ObservableProperty] private string _host = "localhost";
    [ObservableProperty] private ushort _port = 36000;
}