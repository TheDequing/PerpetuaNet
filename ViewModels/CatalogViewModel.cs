using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PerpetuaNet.ViewModels
{
    public class CatalogViewModel : INotifyPropertyChanged
    {
        private string _title = "Catálogo de Conteúdos";
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
