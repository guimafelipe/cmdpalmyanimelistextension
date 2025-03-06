// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Authentication;
using MyAnimeListExtension.Commands;
using MyAnimeListExtension.Pages;

namespace MyAnimeListExtension;

public partial class MyAnimeListExtensionCommandsProvider : CommandProvider
{
    private readonly TokenService _tokenService;
    private readonly InvokableCommand _signInCommand;
    private readonly InvokableCommand _signOutCommand;
    private readonly ListPage _topAnimePage;
    private readonly ListPage _seasonalAnimePage;
    private readonly ListPage _suggestedAnimePage;
    private readonly ListPage _userListsPage;

    public MyAnimeListExtensionCommandsProvider(
        TokenService tokenService,
        ListPage topAnimePage,
        ListPage seasonalAnimePage,
        ListPage suggestedAnimePage,
        ListPage userListsPage,
        InvokableCommand signInCommand,
        InvokableCommand signOutCommand)
    {
        _tokenService = tokenService;
        _tokenService.LoginStateChanged += OnSignInStateChanged;
        _topAnimePage = topAnimePage;
        _seasonalAnimePage = seasonalAnimePage;
        _suggestedAnimePage = suggestedAnimePage;
        _userListsPage = userListsPage;
        _signInCommand = signInCommand;
        _signOutCommand = signOutCommand;

        DisplayName = "My Anime List";
        Icon = IconHelpers.FromRelativePath("Assets\\MALLogo.png");
    }

    public override ICommandItem[] TopLevelCommands()
    {
        var commands = new List<ICommandItem>
        {
            new CommandItem(_topAnimePage)
            {
                Title = "Top anime",
                Subtitle = "Top 50 anime on My Anime List",
            },
            new CommandItem(_seasonalAnimePage)
            {
                Title = "Animes of the Season",
                Subtitle = "Seasonal anime on My Anime List for the current season",
            },
        };

        if (_tokenService.IsLoggedIn())
        {
            commands.Add(new CommandItem(_suggestedAnimePage)
            {
                Title = "Suggested anime",
                Subtitle = "Anime suggestions based on your personal list",
            });
            commands.Add(new CommandItem(_userListsPage)
            {
                Title = "My Anime Lists",
                Subtitle = "Your personal anime lists",
            });
            commands.Add(new CommandItem(_signOutCommand)
            {
                Title = "Sign out of My Anime List",
                Subtitle = "Sign out of My Anime List Extension",
            });
        }
        else
        {
            commands.Add(new CommandItem(_signInCommand)
            {
                Title = "Sign in to My Anime List",
                Subtitle = "Sign in to My Anime List Extension",
            });
        }

        return commands.ToArray();
    }

    private void OnSignInStateChanged(object? source, bool loginState)
    {
        RaiseItemsChanged(0);
    }
}
