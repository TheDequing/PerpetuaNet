using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonoTorrent.Client;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PerpetuaNet.ViewModels;

public partial class CatalogViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<MagnetItem> _magnetLinks;

    private readonly ClientEngine _engine;

    public CatalogViewModel()
    {
        _engine = new ClientEngine();
        MagnetLinks = new ObservableCollection<MagnetItem>
        {
            new MagnetItem { Name = "Arquivo 1", Link = "magnet:?xt=urn:btih:example1..." },
            new MagnetItem { Name = "Arquivo 2", Link = "magnet:?xt=urn:btih:example2..." }
        }; // Adicione seus links magnéticos aqui
    }

    [RelayCommand]
    private async Task DownloadMagnet(string link)
    {
        if (Torrent.TryLoad(link, out var torrent))
        {
            var manager = await _engine.AddAsync(torrent, "C:\\Downloads");
            await manager.StartAsync();
            // Aqui você pode notificar a seção Downloads, se desejar
        }
    }
}

public class MagnetItem
{
    public string Name { get; set; }
    public string Link { get; set; }
}