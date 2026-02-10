using CommunityToolkit.Mvvm.ComponentModel;

namespace PutIsland;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class TextSettingsModel : ObservableObject {
    [ObservableProperty] private string _token = string.Empty;
}