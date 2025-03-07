using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        Icon = new IconInfo("\uE74D");
    }

    public event EventHandler<AnimeStatusUpdatedEventArgs>? AnimeDeleted;

    public override ICommandResult Invoke() => DoInvoke().GetAwaiter().GetResult();

    private async Task<ICommandResult> DoInvoke()
    {
        string message;

        try
        {
            await _dataUpdater.DeleteAnimeFromMyListAsync(_anime);
            _anime.Status = AnimeStatusType.Unknown;
            _anime.Episodes = 0;
            _anime.Score = 0;
            message = $"{_anime.Title} has been deleted from your list";
        }
        catch(HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            message = $"Anime {_anime.Title} was not your list";
        }

        AnimeDeleted?.Invoke(this, new AnimeStatusUpdatedEventArgs(AnimeStatusType.Unknown));
        var toast = new ToastStatusMessage(message);
        toast.Show();

        return CommandResult.KeepOpen();
    }
}
