using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Diagnostics;

namespace PerpetuaNet.Views
{
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Botão Teste clicado!");
        }
    }
}
