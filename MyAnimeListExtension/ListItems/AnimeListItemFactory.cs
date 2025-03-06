using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Authentication;
using MyAnimeListExtension.Commands;
using MyAnimeListExtension.Models;
using MyAnimeListExtension.Pages;

namespace MyAnimeListExtension.ListItems;

public sealed class AnimeListItemFactory
{
    private readonly CommandFactory _commandFactory;
    private readonly TokenService _tokenService;

    public AnimeListItemFactory(CommandFactory commandFactory, TokenService tokenService)
    {
        _commandFactory = commandFactory;
        _tokenService = tokenService;
    }

    public AnimeListItem Create(Anime anime)
    {
        var animeListItem = new AnimeListItem(anime);
        var contentPage = new AnimeContentPage(anime);

        var pageCommands = new List<CommandContextItem>()
        {
            new(new LinkCommand(anime))
            {
                Title = "Open in browser",
                Subtitle = "Open the anime in the browser",
            },
        };

        var moreCommands = new List<CommandContextItem>()
        {
            new(contentPage)
        };

        if (_tokenService.IsLoggedIn())
        {
            var updateAnimeStatusCommand = _commandFactory.CreateUpdateAnimeStatusCommand(anime, AnimeStatusType.PlanToWatch);
            moreCommands.Add(new(updateAnimeStatusCommand));
            pageCommands.Add(new(updateAnimeStatusCommand));

            var deleteCommand = _commandFactory.CreateDeleteAnimeCommand(anime);
            deleteCommand.AnimeDeleted += animeListItem.OnAnimeDeleted;

            moreCommands.Add(new(deleteCommand));
            pageCommands.Add(new(deleteCommand));
        }

        animeListItem.MoreCommands = moreCommands.ToArray();
        contentPage.Commands = pageCommands.ToArray();
        return animeListItem;
    }
}
