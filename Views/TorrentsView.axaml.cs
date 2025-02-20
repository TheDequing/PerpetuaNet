using Avalonia.Controls;

namespace PerpetuaNet.Views;
public partial class TorrentsView : UserControl
{
    public TorrentsView()
    {
        InitializeComponent();
        DataContext = new PerpetuaNet.ViewModels.TorrentsViewModel();
    }
}