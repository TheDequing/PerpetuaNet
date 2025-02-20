using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonoTorrent.Client;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PerpetuaNet.ViewModels;

public partial class TorrentsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<object> _torrents = new();

    [ObservableProperty]
    private string _torrentLink;

    [RelayCommand]
    private async Task AddTorrent()
    {
        if (!string.IsNullOrEmpty(TorrentLink))
        {
            var engine = new ClientEngine();
            var torrent = await Torrent.LoadAsync(TorrentLink);
            var manager = await engine.AddAsync(torrent, "C:\\Downloads");
            await manager.StartAsync();
            Torrents.Add(new { Link = TorrentLink, Progress = $"{manager.Progress}%" });
        }
    }
}