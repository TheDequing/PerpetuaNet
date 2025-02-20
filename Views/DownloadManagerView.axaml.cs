using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PerpetuaNet.Views
{
    public partial class DownloadManagerView : UserControl
    {
        public DownloadManagerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
