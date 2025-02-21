using Avalonia.Controls;

namespace PerpetuaNet.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new PerpetuaNet.ViewModels.MainWindowViewModel();
    }
}