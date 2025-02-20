using System;

namespace PerpetuaNet.Models
{
    public class ContentItem
    {
        public string MainCategory { get; set; } = string.Empty;  // Exemplos: "Jogos", "Filmes", "Series"
        public string SubCategory { get; set; } = string.Empty;     // Exemplos: "Ação", "Drama", etc.
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TorrentLink { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
