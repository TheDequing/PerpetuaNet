using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PerpetuaNet.Views
{
    public partial class InicioView : UserControl
    {
        public InicioView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
