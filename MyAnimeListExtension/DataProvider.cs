using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension;

internal sealed class DataProvider
{
    private static readonly HttpClient client = new();

    public DataProvider()
    {
        // Set up the HttpClient with the base address and any necessary headers
        client.BaseAddress = new Uri("https://api.myanimelist.net/v2/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        // Add your API key here
        var apiKey = Environment.GetEnvironmentVariable("MAL_CLIENT_ID");
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("API key is not set in the environment variables.");
        }
        client.DefaultRequestHeaders.Add("X-MAL-CLIENT-ID", apiKey);
    }

    public static async Task<List<Anime>> GetAnimeRankingAsync()
    {
        HttpResponseMessage response = await client.GetAsync("anime/ranking");
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
#pragma warning disable CA1869 // Cache and reuse 'JsonSerializerOptions' instances
        var prettyJson = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
#pragma warning restore CA1869 // Cache and reuse 'JsonSerializerOptions' instances
        Debug.WriteLine(prettyJson);

        JsonDocument jsonDocument = JsonDocument.Parse(jsonResponse);

        var root = jsonDocument.RootElement;
        var data = root.GetProperty("data").EnumerateArray();
        var res = new List<Anime>();
        foreach (var item in data)
        {
            var anime = new Anime
            {
                Id = item.GetProperty("node").GetProperty("id").GetInt32(),
                Title = item.GetProperty("node").GetProperty("title").GetString() ?? string.Empty,
                ImageUrl = item.GetProperty("node").GetProperty("main_picture").GetProperty("large").GetString() ?? string.Empty,
            };
            Debug.WriteLine(anime.Title);
            Debug.WriteLine(anime.ImageUrl);
            res.Add(anime);
        }

        return res;
    }
}
