using System;
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

    public event EventHandler? AnimeDeleted;

    public async Task<bool> DeleteAnimeFromMyListAsync(Anime anime)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetAccessToken());

        var uri = new Uri($"https://api.myanimelist.net/v2/anime/{anime.Id}/my_list_status");

        Debug.WriteLine($"Deleting {anime.Title} from list");
        var request = new HttpRequestMessage(HttpMethod.Delete, uri);

        var response = await client.SendAsync(request);

        try
        {
            response.EnsureSuccessStatusCode();
            Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
            AnimeDeleted?.Invoke(this, EventArgs.Empty);
            return true;
        }
        catch
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Debug.WriteLine("Anime not found in list");
                return false;
            }

            throw;
        }
    }
}
