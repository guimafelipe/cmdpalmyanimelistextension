using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Commands;

namespace MyAnimeListExtension.Pages;

internal sealed partial class SeasonalAnimePage : ListPage
{
    private readonly DataProvider _dataProvider;

    public SeasonalAnimePage(DataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public override IListItem[] GetItems()
    {
        var res = _dataProvider.GetSeasonAnimeAsync().GetAwaiter().GetResult();

        return res.Select(item => new ListItem(new LinkCommand(item) { })
        {
            Title = item.Title,
        }).ToArray();
    }
}
