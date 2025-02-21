using Avalonia.Controls;

namespace PerpetuaNet.Views;
public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        DataContext = new PerpetuaNet.ViewModels.SettingsViewModel();
    }
}