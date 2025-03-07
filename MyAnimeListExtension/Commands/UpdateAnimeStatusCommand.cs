using System.Threading.Tasks;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Data;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.Commands;

public sealed partial class UpdateAnimeStatusCommand : InvokableCommand
{
    private readonly Anime _anime;
    private readonly AnimeStatusType _status;
    private readonly DataUpdater _dataUpdater;

    private static string GetIconForStatus(AnimeStatusType status) => status switch
    {
        AnimeStatusType.PlanToWatch => "\uE710",
        _ => string.Empty,
    };

    private static string GetNameForStatus(AnimeStatusType status) => status switch
    {
        AnimeStatusType.PlanToWatch => "Plan to watch",
        _ => string.Empty,
    };

    internal UpdateAnimeStatusCommand(Anime anime, AnimeStatusType status, DataUpdater dataUpdater)
    {
        _anime = anime;
        _dataUpdater = dataUpdater;
        _status = status;

        Name = GetNameForStatus(status);
        Icon = new IconInfo(GetIconForStatus(status));
    }

    public override ICommandResult Invoke() => DoInvoke().GetAwaiter().GetResult();

    private async Task<ICommandResult> DoInvoke()
    {
        await _dataUpdater.UpdateAnimeStatusAsync(_anime, _status);
        var toast = new ToastStatusMessage($"Status for {_anime.Title} has been updated to {GetNameForStatus(_status)}");
        toast.Show();
        return CommandResult.KeepOpen();
    }
}
