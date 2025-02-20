using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PerpetuaNet.ViewModels;

namespace PerpetuaNet.Views
{
    public partial class CatalogView : UserControl
    {
        public CatalogView()
        {
            InitializeComponent();
            DataContext = new CatalogViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
