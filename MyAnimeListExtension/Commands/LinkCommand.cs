using System.Diagnostics;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.Commands;

internal sealed partial class LinkCommand : InvokableCommand
{
    private readonly string _htmlUrl;

    internal LinkCommand(Anime anime)
    {
        _htmlUrl = $"https://myanimelist.net/anime/{anime.Id}";
        Name = "Open link";
        Icon = new IconInfo("\uE8A7");
    }

    public override CommandResult Invoke()
    {
        Process.Start(new ProcessStartInfo(_htmlUrl) { UseShellExecute = true });
        return CommandResult.KeepOpen();
    }
}
