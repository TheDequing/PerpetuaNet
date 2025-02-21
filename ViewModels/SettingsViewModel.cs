using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PerpetuaNet.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private string _downloadPath = "C:\\Downloads";

    [RelayCommand]
    private void SaveSettings()
    {
        // Salvar caminho em um arquivo de configuração ou usar diretamente
        System.IO.File.WriteAllText("settings.txt", DownloadPath);
    }
}