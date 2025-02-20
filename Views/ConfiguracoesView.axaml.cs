using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PerpetuaNet.Views
{
    public partial class ConfiguracoesView : UserControl
    {
        public ConfiguracoesView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
