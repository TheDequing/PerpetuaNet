using Avalonia.Controls;

namespace PerpetuaNet.Views;
public partial class SyncView : UserControl
{
    public SyncView()
    {
        InitializeComponent();
        DataContext = new PerpetuaNet.ViewModels.SyncViewModel();
    }
}