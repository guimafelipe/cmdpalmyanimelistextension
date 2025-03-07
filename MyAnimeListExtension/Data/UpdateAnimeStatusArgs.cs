using System.Text;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.Data;

public class UpdateAnimeStatusArgs
{
    public int Score { get; set; }
    public AnimeStatusType Status { get; set; }
    public int NumEpisodesWatched { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("status: ");
        sb.Append(DataHelper.GetStringForUserAnimePageType(Status));
        sb.Append(", score: ");
        sb.Append(Score);
        sb.Append(", num watched episodes: ");
        sb.Append(NumEpisodesWatched);
        return sb.ToString();
    }
}
