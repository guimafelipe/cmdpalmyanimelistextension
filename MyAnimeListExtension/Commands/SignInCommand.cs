using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Authentication;

namespace MyAnimeListExtension.Commands;

internal sealed partial class SignInCommand : InvokableCommand
{

    private readonly TokenService _tokenService;

    internal SignInCommand(TokenService tokenService)
    {
        Name = "Sign in to My Anime List";
        _tokenService = tokenService;
    }

    public override CommandResult Invoke()
    {
        _tokenService.StartLoginUser();
        return CommandResult.KeepOpen();
    }
}
