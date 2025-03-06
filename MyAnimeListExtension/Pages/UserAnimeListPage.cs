using System;
using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Data;
using MyAnimeListExtension.ListItems;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.Pages;

internal sealed partial class UserAnimeListPage : ListPage
{
    private readonly DataProvider _dataProvider;
    private readonly AnimeListItemFactory _animeListItemFactory;
    private readonly AnimeStatusType _type;

    private static string GetTitleForType(AnimeStatusType type)
    {
        return type switch
        {
            AnimeStatusType.Watching => "Watching",
            AnimeStatusType.Completed => "Completed",
            AnimeStatusType.OnHold => "On Hold",
            AnimeStatusType.Dropped => "Dropped",
            AnimeStatusType.PlanToWatch => "Plan to Watch",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public UserAnimeListPage(AnimeStatusType type, DataProvider dataProvider, AnimeListItemFactory animeListItemFactory)
    {
        _dataProvider = dataProvider;
        _animeListItemFactory = animeListItemFactory;
        _type = type;
        Icon = IconHelpers.FromRelativePath("Assets\\MALLogo.jpg");
        Title = $"{GetTitleForType(type)}";
    }

    public override IListItem[] GetItems()
    {
        var res = _dataProvider.GetUserAnimeListAsync(_type).GetAwaiter().GetResult();
        return res.Select(item => _animeListItemFactory.Create(item)).ToArray();
    }
}
