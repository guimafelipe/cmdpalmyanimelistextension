using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Data;
using MyAnimeListExtension.ListItems;

namespace MyAnimeListExtension.Pages;

internal sealed partial class SuggestedAnimePage : ListPage
{
    private readonly DataProvider _dataProvider;

    public SuggestedAnimePage(DataProvider dataProvider)
    {
        _dataProvider = dataProvider;
        Icon = IconHelpers.FromRelativePath("Assets\\MALLogo.jpg");
        Title = "Anime suggestions";
    }

    public override IListItem[] GetItems()
    {
        var res = _dataProvider.GetSuggestedAnimeAsync().GetAwaiter().GetResult();

        return res.Select(item => new AnimeListItem(item)).ToArray();
    }
}
