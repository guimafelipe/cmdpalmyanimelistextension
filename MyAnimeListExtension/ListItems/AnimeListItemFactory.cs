using System;
using System.Collections.Generic;
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
        var moreCommands = new List<CommandContextItem>()
        {
            new(new AnimeContentPage(anime)),
        };

        if (_tokenService.IsLoggedIn())
        {
            moreCommands.Add(new(_commandFactory.CreateUpdateAnimeStatusCommand(anime, AnimeStatusType.PlanToWatch)));

            var deleteCommand = _commandFactory.CreateDeleteAnimeCommand(anime);
            deleteCommand.AnimeDeleted += animeListItem.OnAnimeDeleted;

            moreCommands.Add(new(deleteCommand));
        }

        animeListItem.MoreCommands = moreCommands.ToArray();
        return animeListItem;
    }
}
