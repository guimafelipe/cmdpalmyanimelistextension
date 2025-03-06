using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Authentication;

namespace MyAnimeListExtension.Commands;

internal sealed partial class SignOutCommand : InvokableCommand
{
    private readonly TokenService _tokenService;

    internal SignOutCommand(TokenService tokenService)
    {
        _tokenService = tokenService;
        Name = "Sign out from My Anime List";
        Icon = new IconInfo("\uF3B1");
    }

    public override CommandResult Invoke()
    {
        _tokenService.LogoutUser();
        return CommandResult.KeepOpen();
    }
}
