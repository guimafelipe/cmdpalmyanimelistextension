// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Commands;
using MyAnimeListExtension.Pages;

namespace MyAnimeListExtension;

public partial class MyAnimeListExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public MyAnimeListExtensionCommandsProvider()
    {
        DisplayName = "My Anime List";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        _commands = [
            new CommandItem(new TopAnimePage())
            {
                Title = "Top anime",
            },
            new CommandItem(new SuggestedAnimePage())
            {
                Title = "Suggested anime",
            },
            new CommandItem(new SignInCommand()){
                Title = "My Anime List Extension",
                Subtitle = "Sign in to My Anime List",
            }
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }

}
