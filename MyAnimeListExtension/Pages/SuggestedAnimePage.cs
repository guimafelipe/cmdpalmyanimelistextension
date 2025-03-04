using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Authentication;
using MyAnimeListExtension.Commands;

namespace MyAnimeListExtension.Pages;

internal sealed partial class SuggestedAnimePage : ListPage
{
    public override IListItem[] GetItems()
    {
        if(!TokenService.IsLoggedIn())
        {
            return new IListItem[] { new ListItem(new SignInCommand()) { Title = "Sign in to My Anime List" } };
        }

        var res = DataProvider.GetSuggestedAnimeAsync().GetAwaiter().GetResult();

        return res.Select(item => new ListItem(new LinkCommand(item) { })
        {
            Title = item.Title,
        }).ToArray();
    }
}
