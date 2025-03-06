using MyAnimeListExtension.Data;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.Commands;

public sealed class CommandFactory
{
    private readonly DataUpdater _dataUpdater;

    public CommandFactory(DataUpdater dataUpdater)
    {
        _dataUpdater = dataUpdater;
    }

    public UpdateAnimeStatusCommand CreateUpdateAnimeStatusCommand(Anime anime, AnimeStatusType status)
    {
        return new UpdateAnimeStatusCommand(anime, status, _dataUpdater);
    }
}
