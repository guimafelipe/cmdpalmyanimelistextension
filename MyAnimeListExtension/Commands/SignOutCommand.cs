using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Authentication;

namespace MyAnimeListExtension.Commands;

internal sealed partial class SignOutCommand : InvokableCommand
{
    internal SignOutCommand()
    {
        Name = "Sign out from My Anime List";
    }

    public override CommandResult Invoke()
    {
        TokenService.LogoutUser();
        var toast = new ToastStatusMessage(new StatusMessage()
        {
            Message = "You have been signed out from My Anime List",
            State = MessageState.Success
        });

        toast.Show();

        return CommandResult.KeepOpen();
    }
}
