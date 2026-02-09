using ReactiveUI;

namespace PutIsland;

// ReSharper disable once ClassNeverInstantiated.Global
public class TextSettingsModel : ReactiveObject {
    private string _token = string.Empty;

    public string Token {
        get => _token;
        set => this.RaiseAndSetIfChanged(ref _token, value);
    }
}