using System;
using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Data;
using MyAnimeListExtension.ListItems;

namespace MyAnimeListExtension.Pages;

internal sealed partial class UserAnimeListPage : ListPage
{
    private readonly DataProvider _dataProvider;
    private readonly UserAnimePageType _type;

    private static string GetTitleForType(UserAnimePageType type)
    {
        return type switch
        {
            UserAnimePageType.Watching => "Watching",
            UserAnimePageType.Completed => "Completed",
            UserAnimePageType.OnHold => "On Hold",
            UserAnimePageType.Dropped => "Dropped",
            UserAnimePageType.PlanToWatch => "Plan to Watch",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public UserAnimeListPage(UserAnimePageType type, DataProvider dataProvider)
    {
        _dataProvider = dataProvider;
        _type = type;
        Icon = IconHelpers.FromRelativePath("Assets\\MALLogo.jpg");
        Title = $"{GetTitleForType(type)}";
    }

    public override IListItem[] GetItems()
    {
        var res = _dataProvider.GetUserAnimeListAsync(_type).GetAwaiter().GetResult();
        return res.Select(item => new AnimeListItem(item)).ToArray();
    }
}
