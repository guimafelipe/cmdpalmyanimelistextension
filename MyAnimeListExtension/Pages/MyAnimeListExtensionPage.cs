// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Pages;

namespace MyAnimeListExtension;

internal sealed partial class MyAnimeListExtensionPage : ListPage
{
    public MyAnimeListExtensionPage()
    {
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = "My Anime List";
        Name = "Open";
    }

    public override IListItem[] GetItems()
    {
        return [
        ];
    }
}
