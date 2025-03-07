using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Authentication;
using MyAnimeListExtension.Commands;
using MyAnimeListExtension.Data;
using MyAnimeListExtension.Models;
using MyAnimeListExtension.Pages;
using MyAnimeListExtension.Pages.Forms;

namespace MyAnimeListExtension.ListItems;

public sealed class AnimeListItemFactory
{
    private readonly CommandFactory _commandFactory;
    private readonly TokenService _tokenService;
    private readonly DataUpdater _dataUpdater;

    public AnimeListItemFactory(CommandFactory commandFactory, TokenService tokenService, DataUpdater dataUpdater)
    {
        _commandFactory = commandFactory;
        _tokenService = tokenService;
        _dataUpdater = dataUpdater;
    }

    public AnimeListItem Create(Anime anime)
    {
        var linkCommand = new LinkCommand(anime);
        var animeListItem = new AnimeListItem(anime, linkCommand);

        var form = new AnimeUpdateForm(anime, _dataUpdater);
        var contentPage = new AnimeContentPage(anime, form);

        var pageCommands = new List<CommandContextItem>()
        {
            new(linkCommand),
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
            deleteCommand.AnimeDeleted += contentPage.OnAnimeDeleted;

            moreCommands.Add(new(deleteCommand));
            pageCommands.Add(new(deleteCommand));
        }

        animeListItem.MoreCommands = moreCommands.ToArray();
        contentPage.Commands = pageCommands.ToArray();
        return animeListItem;
    }
}
