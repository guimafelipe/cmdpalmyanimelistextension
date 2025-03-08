using System;
using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Authentication;
using MyAnimeListExtension.Commands;
using MyAnimeListExtension.Models;
using MyAnimeListExtension.Pages.Forms;

namespace MyAnimeListExtension.Pages;

public class AnimeContentPage : ContentPage
{
    private readonly Anime _anime;
    private readonly AnimeUpdateForm _form;
    private readonly TokenService _tokenService;

    public AnimeContentPage(Anime anime, AnimeUpdateForm form, TokenService tokenService)
    {
        _form = form;
        _anime = anime;
        _tokenService = tokenService;
        Icon = new IconInfo(anime.ImageUrl);
        Name = "View Anime information";

        _form.PropChanged += OnFormPropChanged;

        Details = new Details()
        {
            HeroImage = new IconInfo(anime.ImageUrl),
            Metadata = [
                new DetailsElement()
                {
                    Key = $"☆ {(anime.Mean <= 0 ? "--" : anime.Mean)}",
                    Data = new DetailsLink()
                    {
                        Text = $"Ranked #{(anime.Rank == 0 ? "--" : anime.Rank)}",
                    },
                },
                new DetailsElement()
                {
                    Key = $"{anime.MediaType}",
                    Data = new DetailsLink()
                    {
                        Text = $"Episodes: {anime.Episodes}",
                    },
                },
                new DetailsElement()
                {
                    Key = $"Season",
                    Data = new DetailsLink()
                    {
                        Text = $"{anime.StartSeason} {anime.StartYear}",
                    },
                },
                new DetailsElement()
                {
                    Key = $"Studios",
                    Data = new DetailsTags()
                    {
                        Tags = anime.Studios.Select(studio => new Tag(studio)).ToArray()
                    },
                },
                new DetailsElement()
                {
                    Key = "Genres",
                    Data = new DetailsTags()
                    {
                        Tags = anime.Genres.Select(genre => new Tag(genre)).ToArray()
                    },
                },
                ]
        };
    }

    public void OnFormPropChanged(object? source, IPropChangedEventArgs e)
    {
        RaiseItemsChanged(0);
    }

    public void OnAnimeStatusUpdated(object? source, AnimeStatusUpdatedEventArgs e)
    {
        _form.UpdateFormTemplate();
    }

    public override IContent[] GetContent()
    {
        var markdown = new MarkdownContent
        {
            Body = $"# {_anime.Title}\n"
        };

        if (!string.IsNullOrEmpty(_anime.EnglishTitle))
        {
            markdown.Body += $"## {_anime.EnglishTitle}\n";
        }

        markdown.Body += $"{_anime.Synopsis}\n";

        if (!_tokenService.IsLoggedIn())
        {
            return [markdown];
        }

        return [_form, markdown];
    }
}
