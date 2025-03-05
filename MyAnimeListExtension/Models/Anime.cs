using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAnimeListExtension.Models;

public class Anime
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Episodes { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Synopsis { get; set; } = string.Empty;
    public string EnglishTitle { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new List<string>();
}
