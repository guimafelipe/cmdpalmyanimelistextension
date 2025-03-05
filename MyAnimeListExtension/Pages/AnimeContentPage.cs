using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MyAnimeListExtension.Commands;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.Pages;

public class AnimeContentPage : ContentPage
{
    private readonly Anime _anime;

    public AnimeContentPage(Anime anime)
    {
        _anime = anime;
        Icon = new IconInfo(anime.ImageUrl);
        Name = "View Anime information";
        Commands = new CommandContextItem[]
        {
            new(new LinkCommand(anime)),
        };
        Details = new Details()
        {
            HeroImage = new IconInfo(anime.ImageUrl),
            Metadata = [
                new DetailsElement()
                {
                    Key = "Title",
                    Data = new DetailsLink()
                    {
                        Text = anime.EnglishTitle,
                    },
                },
                new DetailsElement()
                {
                    Key = "Episodes",
                    Data = new DetailsLink()
                    {
                        Text = $"{anime.Episodes}",
                    },
                },
                new DetailsElement()
                {
                    Data = new DetailsSeparator()
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
        var template = new MarkdownContent
        {
            Body = $$"""
            # {{_anime.Title}}
            ## {{_anime.EnglishTitle}}

            {{_anime.Synopsis}}
            """
        };

        return [template];
    }
}
