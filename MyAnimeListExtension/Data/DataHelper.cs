using System;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.Data;

public static class DataHelper
{
    public static string GetStringForUserAnimePageType(AnimeStatusType type)
    {
        return type switch
        {
            AnimeStatusType.Dropped => "dropped",
            AnimeStatusType.OnHold => "on_hold",
            AnimeStatusType.PlanToWatch => "plan_to_watch",
            AnimeStatusType.Watching => "watching",
            AnimeStatusType.Completed => "completed",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }
}
