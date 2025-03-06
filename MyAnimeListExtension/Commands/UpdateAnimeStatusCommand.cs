using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Data;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.Commands;

public sealed partial class UpdateAnimeStatusCommand : InvokableCommand
{
    private readonly Anime _anime;
    private readonly AnimeStatusType _status;
    private readonly DataUpdater _dataUpdater;

    internal UpdateAnimeStatusCommand(Anime anime, AnimeStatusType status, DataUpdater dataUpdater)
    {
        _anime = anime;
        _dataUpdater = dataUpdater;
        _status = status;
        Name = $"{status}";
    }

    public override CommandResult Invoke()
    {
        _dataUpdater.UpdateAnimeStatusAsync(_anime, _status).GetAwaiter().GetResult();
        var toast = new ToastStatusMessage($"Status for {_anime.Title} has been updated to {_status}");
        toast.Show();
        return CommandResult.KeepOpen();
    }
}
