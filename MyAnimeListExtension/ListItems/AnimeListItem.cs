using System.Linq;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Commands;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.ListItems;

public sealed partial class AnimeListItem : ListItem
{
    private const int _maxNumberOfTags = 4;

    public AnimeListItem(Anime anime) : base(new LinkCommand(anime))
    {
        Title = anime.Title;
        Subtitle = anime.EnglishTitle;
        Tags = anime.Genres.Select(genre => new Tag
        {
            Text = genre,
        }).Take(_maxNumberOfTags).ToArray();
    }
}
