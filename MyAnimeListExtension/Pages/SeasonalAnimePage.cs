using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Data;
using MyAnimeListExtension.ListItems;

namespace MyAnimeListExtension.Pages;

internal sealed partial class SeasonalAnimePage : ListPage
{
    private readonly DataProvider _dataProvider;
    private readonly AnimeListItemFactory _animeListItemFactory;

    public SeasonalAnimePage(DataProvider dataProvider, AnimeListItemFactory animeListItemFactory)
    {
        _dataProvider = dataProvider;
        _animeListItemFactory = animeListItemFactory;
        Icon = IconHelpers.FromRelativePath("Assets\\MALLogo.jpg");
        Title = "Animes of the Season";
    }

    public override IListItem[] GetItems()
    {
        var res = _dataProvider.GetSeasonAnimeAsync().GetAwaiter().GetResult();

        return res.Select(item => _animeListItemFactory.Create(item)).ToArray();
    }
}
