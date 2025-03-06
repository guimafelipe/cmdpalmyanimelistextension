using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Data;
using MyAnimeListExtension.ListItems;

namespace MyAnimeListExtension.Pages;

internal sealed partial class TopAnimePage : ListPage
{
    private readonly DataProvider _dataProvider;
    private readonly AnimeListItemFactory _animeListItemFactory;
    public TopAnimePage(DataProvider dataProvider, AnimeListItemFactory animeListItemFactory)
    {
        _dataProvider = dataProvider;
        Icon = IconHelpers.FromRelativePath("Assets\\MALLogo.jpg");
        Title = "Top 100 anime on My Anime List";
        _animeListItemFactory = animeListItemFactory;
    }

    public override IListItem[] GetItems()
    {
        var res = _dataProvider.GetAnimeRankingAsync().GetAwaiter().GetResult();

        return res.Select(item => _animeListItemFactory.Create(item)).ToArray();
    }
}
