using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Commands;

namespace MyAnimeListExtension.Pages;

internal sealed partial class TopAnimePage : ListPage
{
    private readonly DataProvider _dataProvider;
    public TopAnimePage(DataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public override IListItem[] GetItems()
    {
        var res = _dataProvider.GetAnimeRankingAsync().GetAwaiter().GetResult();

        return res.Select(item => new ListItem(new LinkCommand(item) { })
        {
            Title = item.Title,
        }).ToArray();
    }
}
