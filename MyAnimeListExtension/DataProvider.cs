using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension;

internal class DataProvider
{
    private static readonly HttpClient client = new HttpClient();

    public DataProvider()
    {
        // Set up the HttpClient with the base address and any necessary headers
        client.BaseAddress = new Uri("https://api.myanimelist.net/v2/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        // Add your API key here
        client.DefaultRequestHeaders.Add("X-MAL-CLIENT-ID", "e04e2ca4d6808e515b85dc14b8687ec1");
    }

    public static async Task<List<Anime>> GetAnimeRankingAsync()
    {
        HttpResponseMessage response = await client.GetAsync("anime/ranking");
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        JsonDocument jsonDocument = JsonDocument.Parse(jsonResponse);

        var root = jsonDocument.RootElement;
        var data = root.GetProperty("data").EnumerateArray();
        var res = new List<Anime>();
        foreach (var item in data)
        {
            var anime = new Anime
            {
                Title = item.GetProperty("node").GetProperty("title").GetString(),
                ImageUrl = item.GetProperty("node").GetProperty("main_picture").GetProperty("large").GetString(),
            };
            Debug.WriteLine(anime.Title);
            Debug.WriteLine(anime.ImageUrl);
            res.Add(anime);
        }

        return res;
    }
}
