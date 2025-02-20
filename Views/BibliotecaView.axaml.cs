using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PerpetuaNet.Views
{
    public partial class BibliotecaView : UserControl
    {
        public BibliotecaView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
