using System.Text;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.Data;

public class UpdateAnimeStatusArgs
{
    public int? Score { get; set; }

    // completed, watching, on_hold, dropped or plan_to_watch
    public string? Status { get; set; }

    public int? NumEpisodesWatched { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("status: ");
        sb.Append(Status);
        sb.Append(", score: ");
        sb.Append(Score);
        sb.Append(", num watched episodes: ");
        sb.Append(NumEpisodesWatched);
        return sb.ToString();
    }
}
