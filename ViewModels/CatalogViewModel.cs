using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonoTorrent;
using MonoTorrent.Client;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        Debug.WriteLine("CatalogViewModel inicializado com sucesso");
    }

    [RelayCommand]
    private async Task DownloadMagnet(string link)
    {
        try
        {
            Debug.WriteLine($"Iniciando download do link: {link}");
            var magnet = MagnetLink.Parse(link);
            var manager = await _engine.AddAsync(magnet, "C:\\Downloads");
            await manager.StartAsync();
            Debug.WriteLine($"Download iniciado para: {link}");
        }
        catch (Exception ex)
        {
            MagnetLinks.Add(new MagnetItem { Name = "Erro", Link = $"Falha: {ex.Message}" });
            Debug.WriteLine($"Erro ao baixar {link}: {ex.Message}");
        }
    }
}

public class MagnetItem
{
    public string? Name { get; set; }
    public string? Link { get; set; }
}