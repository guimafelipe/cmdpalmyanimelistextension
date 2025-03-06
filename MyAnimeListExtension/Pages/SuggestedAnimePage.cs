using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Data;
using MyAnimeListExtension.ListItems;

namespace MyAnimeListExtension.Pages;

internal sealed partial class SuggestedAnimePage : ListPage
{
    private readonly DataProvider _dataProvider;
    private readonly AnimeListItemFactory _animeListItemFactory;

    public SuggestedAnimePage(DataProvider dataProvider, AnimeListItemFactory animeListItemFactory  )
    {
        _dataProvider = dataProvider;
        Icon = IconHelpers.FromRelativePath("Assets\\MALLogo.jpg");
        Title = "Anime suggestions";
        _animeListItemFactory = animeListItemFactory;
    }

    public override IListItem[] GetItems()
    {
        var res = _dataProvider.GetSuggestedAnimeAsync().GetAwaiter().GetResult();

        return res.Select(item => _animeListItemFactory.Create(item)).ToArray();
    }
}
