using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Data;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.Pages.Forms;

public sealed class AnimeUpdateForm : FormContent
{
    private readonly Anime _anime;
    private readonly DataUpdater _dataUpdater;
    private readonly Dictionary<string, string> _templateSubstituitions;

    public AnimeUpdateForm(Anime anime, DataUpdater dataUpdater)
    {
        _anime = anime;
        _dataUpdater = dataUpdater;

        if (anime.Status != AnimeStatusType.Unknown)
        {
            _templateSubstituitions = new()
            {
                { "{{total_episodes}}", $"{_anime.Episodes}" },
                { "{{num_episodes_watched}}", $"{_anime.NumEpisodesWatched}" },
                { "{{score}}", $"{_anime.Score}" },
                { "{{status}}", $"{DataHelper.GetStringForUserAnimePageType(_anime.Status)}" },
            };
        }
        else
        {
            _templateSubstituitions = new()
            {
                { "{{total_episodes}}", $"{_anime.Episodes}" },
                { "{{num_episodes_watched}}", "1" },
                { "{{score}}", "1" },
                { "{{status}}", "dropped" },
            };
        }

        TemplateJson = LoadTemplate();
    }

    public override ICommandResult SubmitForm(string inputs) => DoSubmitForm(inputs).GetAwaiter().GetResult();

#pragma warning disable CA1822 // Mark members as static
    private async Task<ICommandResult> DoSubmitForm(string inputs)
    {
        Debug.WriteLine(inputs);

        await Task.CompletedTask;

        return CommandResult.KeepOpen();
    }

    private string LoadTemplate()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Pages", "Forms", "Templates", $"AnimeUpdateTemplate.json");
        var template = File.ReadAllText(path, Encoding.Default) ?? throw new FileNotFoundException(path);
        template = FillInTemplate(template);
        Debug.WriteLine(template);
        return template;
    }

    private string FillInTemplate(string template)
    {
        foreach (var substituition in _templateSubstituitions)
        {
            template = template.Replace(substituition.Key, substituition.Value);
        }

        return template;
    }
}
