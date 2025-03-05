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
    public List<string> Studios { get; set; } = new List<string>();
    public string MediaType { get; set; } = string.Empty;
    public string StartSeason { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int Rank { get; set; }
    public double Mean { get; set; }
    public int NumListUsers { get; set; }
}
