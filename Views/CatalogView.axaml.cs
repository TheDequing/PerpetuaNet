using Avalonia.Controls;

namespace PerpetuaNet.Views;

public partial class CatalogView : UserControl
{
    public CatalogView()
    {
        InitializeComponent();
        DataContext = new PerpetuaNet.ViewModels.CatalogViewModel();
    }
}