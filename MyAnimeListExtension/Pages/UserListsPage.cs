using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace MyAnimeListExtension.Pages;

internal sealed partial class UserListsPage : ListPage
{
    private readonly IListPage _watchingPage;
    private readonly IListPage _completedPage;
    private readonly IListPage _onHoldPage;
    private readonly IListPage _droppedPage;
    private readonly IListPage _planToWatchPage;

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

        _watchingPage = watchingPage;
        _completedPage = completedPage;
        _onHoldPage = onHoldPage;
        _droppedPage = droppedPage;
        _planToWatchPage = planToWatchPage;
    }

    public override IListItem[] GetItems()
    {
        return new IListItem[]
        {
            new ListItem(_watchingPage)
            {
               Title = _watchingPage.Title,
            },
            new ListItem(_completedPage)
            {
                Title = _completedPage.Title,
            },
            new ListItem(_onHoldPage)
            {
                Title = _onHoldPage.Title,
            },
            new ListItem(_droppedPage) 
            { 
                Title = _droppedPage.Title 
            },
            new ListItem(_planToWatchPage)
            {
                Title = _planToWatchPage.Title 
            }
        };
    }
}
