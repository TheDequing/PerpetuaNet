using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerpetuaNet.Services
{
    public class TorrentManager
    {
        // Lista de torrents gerenciados
        public List<TorrentInfo> Torrents { get; private set; } = new List<TorrentInfo>();

        public TorrentManager()
        {
            // Aqui você pode carregar torrents salvos do banco de dados, se necessário.
        }

        /// <summary>
        /// Adiciona um torrent e simula seu download.
        /// </summary>
        public async Task<bool> AddTorrentAsync(string torrentLink)
        {
            TorrentInfo info = new TorrentInfo
            {
                Link = torrentLink,
                Status = "Download iniciado",
                Progress = 0
            };

            Torrents.Add(info);
            Console.WriteLine($"[TORRENT] Torrent adicionado: {torrentLink}");

            // Simula o progresso do download
            await Task.Delay(1000); // Simula tempo para iniciar o download
            info.Progress = 50;
            info.Status = "Em andamento";
            Console.WriteLine($"[TORRENT] Torrent em andamento: {torrentLink}");

            await Task.Delay(1000); // Simula o tempo para concluir
            info.Progress = 100;
            info.Status = "Concluído";
            Console.WriteLine($"[TORRENT] Torrent concluído: {torrentLink}");

            return true;
        }

        // Métodos para pausar, retomar ou cancelar downloads podem ser implementados posteriormente.
    }

    public class TorrentInfo
    {
        public string Link { get; set; } = "";
        public string Status { get; set; } = "";
        public int Progress { get; set; } = 0;
    }
}
