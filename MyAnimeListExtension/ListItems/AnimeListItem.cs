using System;
using System.Linq;
using Microsoft.CommandPalette.Extensions;
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

    public AnimeListItem(Anime anime, ICommand command) : base(command)
    {
        Title = anime.Title;
        Subtitle = anime.EnglishTitle;
        Icon = new IconInfo(anime.ImageUrl);
        Tags = anime.Genres.Select(genre => new Tag
        {
            Text = genre,
        }).Take(_maxNumberOfTags).ToArray();
    }
}
