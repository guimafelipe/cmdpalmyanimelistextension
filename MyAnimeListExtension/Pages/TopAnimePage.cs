using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Commands;

namespace MyAnimeListExtension.Pages;

internal sealed partial class TopAnimePage : ListPage
{
    public override IListItem[] GetItems()
    {
        var res = DataProvider.GetAnimeRankingAsync().GetAwaiter().GetResult();

        return res.Select(item => new ListItem(new LinkCommand(item) { })
        {
            Title = item.Title,
        }).ToArray();
    }
}
