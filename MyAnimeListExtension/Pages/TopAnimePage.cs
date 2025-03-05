using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.ListItems;

namespace MyAnimeListExtension.Pages;

internal sealed partial class TopAnimePage : ListPage
{
    private readonly DataProvider _dataProvider;
    public TopAnimePage(DataProvider dataProvider)
    {
        _dataProvider = dataProvider;
        Icon = IconHelpers.FromRelativePath("Assets\\MALLogo.jpg");
        Title = "Top 100 anime on My Anime List";
    }

    public override IListItem[] GetItems()
    {
        var res = _dataProvider.GetAnimeRankingAsync().GetAwaiter().GetResult();

        return res.Select(item => new AnimeListItem(item)).ToArray();
    }
}
