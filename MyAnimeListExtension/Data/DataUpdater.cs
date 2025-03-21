﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MyAnimeListExtension.Authentication;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension.Data;

public sealed class DataUpdater
{
    private readonly TokenService _tokenService;

    public DataUpdater(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task UpdateAnimeStatusAsync(Anime anime, AnimeStatusType type)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetAccessToken());

        var uri = new Uri($"https://api.myanimelist.net/v2/anime/{anime.Id}/my_list_status");

        Debug.WriteLine($"Updating status for {anime.Title} to {type}");

        var request = new HttpRequestMessage(HttpMethod.Patch, uri)
        {
            Content =
            new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("status", DataHelper.GetStringForUserAnimePageType(type)),
            }),
        };

        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
    }
    public async Task UpdateAnimeStatusAsync(Anime anime, UpdateAnimeStatusArgs args)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetAccessToken());

        var uri = new Uri($"https://api.myanimelist.net/v2/anime/{anime.Id}/my_list_status");

        Debug.WriteLine($"Updating status for {anime.Title} to {args}");

        var requestDict = new Dictionary<string, string>();
        if (args.Status != null)
        {
            requestDict.Add("status", args.Status);
        }

        if (args.Score != null)
        {
            requestDict.Add("score", $"{args.Score}");
        }

        if (args.NumEpisodesWatched != null)
        {
            requestDict.Add("num_watched_episodes", $"{args.NumEpisodesWatched}");
        }

        var request = new HttpRequestMessage(HttpMethod.Patch, uri)
        {
            Content = new FormUrlEncodedContent(requestDict)
        };

        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
    }

    public async Task DeleteAnimeFromMyListAsync(Anime anime)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetAccessToken());

        var uri = new Uri($"https://api.myanimelist.net/v2/anime/{anime.Id}/my_list_status");

        Debug.WriteLine($"Deleting {anime.Title} from list");
        var request = new HttpRequestMessage(HttpMethod.Delete, uri);

        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        Debug.WriteLine(response.StatusCode);
    }
}
