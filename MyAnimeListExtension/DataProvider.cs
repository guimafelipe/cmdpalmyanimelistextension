using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MyAnimeListExtension.Authentication;
using MyAnimeListExtension.Models;

namespace MyAnimeListExtension;

internal sealed class DataProvider
{
    private readonly TokenService _tokenService;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    public DataProvider(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    private static HttpClient GetClient()
    {
        var client = new HttpClient();
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

        return client;
    }

    public async Task<List<Anime>> GetSuggestedAnimeAsync()
    {
        using var client = GetClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetAccessToken());
        HttpResponseMessage response = await client.GetAsync("anime/suggestions");
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
        var prettyJson = JsonSerializer.Serialize(jsonElement, _jsonSerializerOptions);
        Debug.WriteLine(prettyJson);

        var jsonDocument = JsonDocument.Parse(jsonResponse);

        var root = jsonDocument.RootElement;
        var data = root.GetProperty("data").EnumerateArray();
        var res = new List<Anime>();
        foreach (var item in data)
        {
            var anime = new Anime
            {
                Id = item.GetProperty("node").GetProperty("id").GetInt32(),
                Title = item.GetProperty("node").GetProperty("title").GetString() ?? string.Empty,
                ImageUrl = item.GetProperty("node").GetProperty("main_picture").GetProperty("medium").GetString() ?? string.Empty,
            };
            Debug.WriteLine(anime.Title);
            Debug.WriteLine(anime.ImageUrl);
            res.Add(anime);
        }

        return res;
    }

    private static string GetUriWithQuery(string uri, Dictionary<string, string> query)
    {
        var sb = new StringBuilder(uri);

        if (query.Count == 0)
        {
            return sb.ToString();
        }

        var addedQuestionMark = false;


        foreach (var (key, value) in query)
        {
            sb.Append(addedQuestionMark ? '&' : '?');
            addedQuestionMark = true;

            sb.Append(key);
            sb.Append('=');
            sb.Append(value);
        }

        return sb.ToString();
    }

    public async Task<List<Anime>> GetAnimeRankingAsync()
    {
        using var client = GetClient();

        HttpResponseMessage response = await client.GetAsync("anime/ranking");
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
        var prettyJson = JsonSerializer.Serialize(jsonElement, _jsonSerializerOptions);
        Debug.WriteLine(prettyJson);

        var jsonDocument = JsonDocument.Parse(jsonResponse);

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

    public async Task<List<Anime>> GetSeasonAnimeAsync()
    {
        using var client = GetClient();

        var query = new Dictionary<string, string>
        {
            { "sort", "anime_num_list_users" },
            { "limit", "100" },
            { "fields", "id,title,main_picture,synopsis" },
        };

        var uriString = GetUriWithQuery("anime/season/2025/winter", query);

        HttpResponseMessage response = await client.GetAsync(uriString);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
        var prettyJson = JsonSerializer.Serialize(jsonElement, _jsonSerializerOptions);
        Debug.WriteLine(prettyJson);

        var jsonDocument = JsonDocument.Parse(jsonResponse);

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
