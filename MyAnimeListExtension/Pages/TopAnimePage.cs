using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace MyAnimeListExtension.Pages;

internal partial class TopAnimePage : ListPage
{
    public override IListItem[] GetItems()
    {
        var res = DataProvider.GetAnimeRankingAsync().GetAwaiter().GetResult();

        return [];
    }
}
