using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Authentication;

namespace MyAnimeListExtension.Commands;

internal sealed partial class SignInCommand : InvokableCommand
{

    internal SignInCommand()
    {
        Name = "Sign in to My Anime List";
    }

    public override CommandResult Invoke()
    {
        TokenService.LoginUser();
        return CommandResult.KeepOpen();
    }
}
