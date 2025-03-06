using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Commands;
using MyAnimeListExtension.Models;
using MyAnimeListExtension.Pages;

namespace MyAnimeListExtension.ListItems;

public sealed class AnimeListItemFactory
{
    private readonly CommandFactory _commandFactory;

    public AnimeListItemFactory(CommandFactory commandFactory)
    {
        _commandFactory = commandFactory;
    }

    public AnimeListItem Create(Anime anime)
    {
        var animeListItem = new AnimeListItem(anime);
        animeListItem.MoreCommands = new CommandContextItem[]
        {
            new(new AnimeContentPage(anime)),
            new(_commandFactory.CreateUpdateAnimeStatusCommand(anime, AnimeStatusType.PlanToWatch)),
        };
        return animeListItem;
    }
}
