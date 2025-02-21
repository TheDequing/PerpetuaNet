using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonoTorrent;
using MonoTorrent.Client;
using System.Collections.ObjectModel;
using System.Timers;

namespace PerpetuaNet.ViewModels;

public partial class TorrentsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<object> _torrents = new();

    [ObservableProperty]
    private string? _torrentLink;

    private ClientEngine _engine;
    private TorrentManager? _manager;

    public TorrentsViewModel()
    {
        _engine = new ClientEngine();
    }

    [RelayCommand]
    private async Task AddTorrent()
    {
        try
        {
            if (!string.IsNullOrEmpty(TorrentLink))
            {
                if (Torrent.TryLoad(TorrentLink, out var torrent)) // Usa TryLoad
                {
                    _manager = await _engine.AddAsync(torrent, "C:\\Downloads");
                    await _manager.StartAsync();
                    Torrents.Add(new { Link = TorrentLink, Progress = $"{_manager.Progress}%" });

                    var timer = new System.Timers.Timer(1000);
                    timer.Elapsed += (s, e) => UpdateProgress();
                    timer.Start();
                }
                else
                {
                    Torrents.Add(new { Link = TorrentLink, Progress = "Erro: Torrent inválido" });
                }
            }
        }
        catch (Exception ex)
        {
            Torrents.Add(new { Link = TorrentLink, Progress = $"Erro: {ex.Message}" });
        }
    }

    private void UpdateProgress()
    {
        if (_manager != null && Torrents.Count > 0)
        {
            var lastTorrent = Torrents[Torrents.Count - 1];
            Torrents[Torrents.Count - 1] = new { Link = lastTorrent.GetType().GetProperty("Link")?.GetValue(lastTorrent), Progress = $"{_manager.Progress}%" };
        }
    }
}