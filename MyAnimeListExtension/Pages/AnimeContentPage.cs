using System;
using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Models;
using MyAnimeListExtension.Pages.Forms;

namespace MyAnimeListExtension.Pages;

public class AnimeContentPage : ContentPage
{
    private readonly Anime _anime;
    private readonly AnimeUpdateForm _form;

    public AnimeContentPage(Anime anime, AnimeUpdateForm form)
    {
        _form = form;
        _anime = anime;
        Icon = new IconInfo(anime.ImageUrl);
        Name = "View Anime information";
        
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

    public void OnAnimeDeleted(object? source, EventArgs e)
    {
        _form.UpdateFormTemplate();
        RaiseItemsChanged(0);
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

        return [_form, markdown];
    }
}
