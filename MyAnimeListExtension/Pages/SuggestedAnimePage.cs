﻿using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Authentication;
using MyAnimeListExtension.Commands;

namespace MyAnimeListExtension.Pages;

internal sealed partial class SuggestedAnimePage : ListPage
{
    private readonly DataProvider _dataProvider;

    public SuggestedAnimePage(DataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public override IListItem[] GetItems()
    {
        var res = _dataProvider.GetSuggestedAnimeAsync().GetAwaiter().GetResult();

        return res.Select(item => new ListItem(new LinkCommand(item) { })
        {
            Title = item.Title,
        }).ToArray();
    }
}
