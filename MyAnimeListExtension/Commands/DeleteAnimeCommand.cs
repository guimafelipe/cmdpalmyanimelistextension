using System;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Data;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.Commands;

public sealed class DeleteAnimeCommand : InvokableCommand
{
    private readonly Anime _anime;
    private readonly DataUpdater _dataUpdater;

    public DeleteAnimeCommand(Anime anime, DataUpdater dataUpdater)
    {
        _anime = anime;
        _dataUpdater = dataUpdater;
        Name = "Delete from your list";
    }

    public event EventHandler? AnimeDeleted;

    public override CommandResult Invoke()
    {
        var result = _dataUpdater.DeleteAnimeFromMyListAsync(_anime).GetAwaiter().GetResult();

        var message = result ?
            $"{_anime.Title} has been deleted from your list" 
            : $"Anime {_anime.Title} was not your list";

        var toast = new ToastStatusMessage(message);
        toast.Show();

        AnimeDeleted?.Invoke(this, EventArgs.Empty);

        return CommandResult.KeepOpen();
    }
}
