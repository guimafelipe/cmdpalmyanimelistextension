using System.Collections.Generic;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace MyAnimeListExtension.Pages;

internal sealed partial class UserListsPage : ListPage
{
    private readonly List<IListPage> _userPages;

    public UserListsPage(
        IListPage watchingPage,
        IListPage completedPage,
        IListPage onHoldPage,
        IListPage droppedPage,
        IListPage planToWatchPage)
    {

        Icon = IconHelpers.FromRelativePath("Assets\\MALLogo.jpg");
        Title = "My Anime lists";
        Name = "My Anime lists";

        _userPages =
        [
            watchingPage,
            completedPage,
            onHoldPage,
            droppedPage,
            planToWatchPage
        ];
    }

    public override IListItem[] GetItems()
    {
        var res = new List<IListItem>();

        foreach (var page in _userPages)
        {
            res.Add(new ListItem(page)
            {
                Title = page.Title,
                Subtitle = $"My Anime List: {page.Title} page"
            });
        }

        return res.ToArray();
    }
}
