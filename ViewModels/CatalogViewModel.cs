using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonoTorrent;
using MonoTorrent.Client;
using System;
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
        };
    }

    [RelayCommand]
    private async Task DownloadMagnet(string link)
    {
        try
        {
            var magnet = MagnetLink.Parse(link); // Usa MagnetLink.Parse para links magnéticos
            var manager = await _engine.AddAsync(magnet, "C:\\Downloads"); // Adiciona diretamente o MagnetLink
            await manager.StartAsync();
            // Aqui você pode notificar a seção Downloads, se desejar
        }
        catch (Exception ex)
        {
            // Tratar erro (ex.: link inválido)
            MagnetLinks.Add(new MagnetItem { Name = "Erro", Link = $"Falha: {ex.Message}" });
        }
    }
}

public class MagnetItem
{
    public string? Name { get; set; }
    public string? Link { get; set; }
}