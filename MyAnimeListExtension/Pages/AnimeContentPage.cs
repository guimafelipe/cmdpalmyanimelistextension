using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.Pages;

public class AnimeContentPage : ContentPage
{
    private readonly Anime _anime;
    private readonly IFormContent _form;

    public AnimeContentPage(Anime anime, IFormContent form)
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

    public override IContent[] GetContent()
    {
        var markdown = new MarkdownContent
        {
            Body = $$"""
            # {{_anime.Title}}
            ## {{_anime.EnglishTitle}}

            {{_anime.Synopsis}}
            """
        };

        return [markdown, _form];
    }
}
