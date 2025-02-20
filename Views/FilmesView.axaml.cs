using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PerpetuaNet.Views
{
    public partial class FilmesView : UserControl
    {
        public FilmesView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
