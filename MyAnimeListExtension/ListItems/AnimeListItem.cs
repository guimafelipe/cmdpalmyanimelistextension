using System;
using System.Linq;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Commands;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.ListItems;

public sealed partial class AnimeListItem : ListItem
{
    private const int _maxNumberOfTags = 4;

    public event EventHandler? AnimeDeleted;

    public void OnAnimeDeleted(object? source, EventArgs e)
    {
        AnimeDeleted?.Invoke(this, EventArgs.Empty);
    }

    public AnimeListItem(Anime anime) : base(new LinkCommand(anime) { Name = "Open Anime in browser" })
    {
        Title = anime.Title;
        Subtitle = anime.EnglishTitle;
        Tags = anime.Genres.Select(genre => new Tag
        {
            Text = genre,
        }).Take(_maxNumberOfTags).ToArray();
    }
}
