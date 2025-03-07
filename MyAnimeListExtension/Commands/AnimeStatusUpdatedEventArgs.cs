using System;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.Commands;

public class AnimeStatusUpdatedEventArgs : EventArgs
{
    public AnimeStatusType NewStatus { get; set; }

    public AnimeStatusUpdatedEventArgs(AnimeStatusType newStatus)
    {
        NewStatus = newStatus;
    }
}
