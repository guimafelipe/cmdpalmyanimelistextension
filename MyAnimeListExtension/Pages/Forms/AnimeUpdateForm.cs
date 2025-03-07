using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
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
    private string _cachedTemplate { get; set; } = string.Empty;

    public AnimeUpdateForm(Anime anime, DataUpdater dataUpdater)
    {
        _anime = anime;
        _dataUpdater = dataUpdater;

        UpdateFormTemplate();
    }

    private Dictionary<string, string> GenerateTemplateSubstituitions(Anime anime)
    {
        // If status is set, the anime is on the list
        if (anime.Status != AnimeStatusType.Unknown)
        {
            return new()
            {
                { "{{total_episodes}}", $"{_anime.Episodes}" },
                { "{{num_episodes_watched}}", $"{_anime.NumEpisodesWatched}" },
                { "{{score}}", $"{_anime.Score}" },
                { "{{status}}", $"{DataHelper.GetStringForUserAnimePageType(_anime.Status)}" },
            };
        }
        else
        {
            return new()
            {
                { "{{total_episodes}}", $"{_anime.Episodes}" },
                { "{{num_episodes_watched}}", "0" },
                { "{{score}}", "" },
                { "{{status}}", "" },
            };
        }
    }

    public override ICommandResult SubmitForm(string inputs) => DoSubmitForm(inputs).GetAwaiter().GetResult();

    private bool ValidateEpisodes(int episodes)
    {
        return episodes >= 0 && episodes <= _anime.Episodes;
    }

    private async Task<ICommandResult> DoSubmitForm(string inputs)
    {
        Debug.WriteLine(inputs);
        var jsonNode = JsonNode.Parse(inputs);

        var args = new UpdateAnimeStatusArgs();

        var episodesWatchedString = jsonNode!["episodes"]!.ToString();

        if (episodesWatchedString != string.Empty)
        {
            if (!int.TryParse(episodesWatchedString, out var numEpisodesWatched) || !ValidateEpisodes(numEpisodesWatched))
            {
                var toastInvalid = new ToastStatusMessage(new StatusMessage()
                {
                    Message = $"Please enter a valid number of episodes watched (between 0 and {_anime.Episodes}).",
                    State = MessageState.Error,
                });
                toastInvalid.Show();
                return CommandResult.KeepOpen();
            }

            args.NumEpisodesWatched = numEpisodesWatched;
        }

        var scoreString = jsonNode!["score"]!.ToString();
        if (scoreString != string.Empty && int.TryParse(scoreString, out var score))
        {
            args.Score = score;
        }

        args.Status = jsonNode!["status"]?.ToString();

        await _dataUpdater.UpdateAnimeStatusAsync(_anime, args);

        var toast = new ToastStatusMessage(new StatusMessage()
        {
            Message = "Anime status updated successfully.",
        });

        toast.Show();

        if (!string.IsNullOrEmpty(args.Status))
        {
            _anime.Status = DataHelper.GetAnimeStatusTypeFromString(args.Status!);
        }

        _anime.Score = args.Score ?? 0;
        _anime.NumEpisodesWatched = args.NumEpisodesWatched ?? 0;

        UpdateFormTemplate();

        return CommandResult.KeepOpen();
    }

    public void UpdateFormTemplate()
    {
        var templateSubstituitions = GenerateTemplateSubstituitions(_anime);
        TemplateJson = LoadTemplate(templateSubstituitions);
        OnPropertyChanged(nameof(TemplateJson));
    }

    private string LoadTemplate(Dictionary<string, string> templateSubstituitions)
    {
        // To limit the number of file reads, cache the template
        if (!string.IsNullOrEmpty(_cachedTemplate))
        {
            return FillInTemplate(_cachedTemplate, templateSubstituitions);
        }

        var path = Path.Combine(AppContext.BaseDirectory, "Pages", "Forms", "Templates", $"AnimeUpdateTemplate.json");
        _cachedTemplate = File.ReadAllText(path, Encoding.Default) ?? throw new FileNotFoundException(path);

        return FillInTemplate(_cachedTemplate, templateSubstituitions);
    }

    private static string FillInTemplate(string template, Dictionary<string, string> templateSubstituitions)
    {
        foreach (var substituition in templateSubstituitions)
        {
            template = template.Replace(substituition.Key, substituition.Value);
        }

        return template;
    }
}
