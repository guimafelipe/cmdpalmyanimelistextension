using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MyAnimeListExtension.Authentication;
using MyAnimeListExtension.Models;
using Windows.Globalization;

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

    private static string QueryFields => "id,title,main_picture,synopsis,alternative_titles," +
        "genres,num_episodes,mean,media_type,studios,num_list_users,start_season,rank";

    private List<Anime> GetFromJsonData(string? json)
    {
        var res = new List<Anime>();

        if (json == null)
        {
            return res;
        }

        var jsonDocument = JsonDocument.Parse(json);

        var root = jsonDocument.RootElement;
        var data = root.GetProperty("data").EnumerateArray();
        foreach (var item in data)
        {
            var prettyJson = JsonSerializer.Serialize(item, _jsonSerializerOptions);
            Debug.WriteLine(prettyJson);
            var anime = new Anime
            {
                Id = item.GetProperty("node").GetProperty("id").GetInt32(),
                Title = item.GetProperty("node").GetProperty("title").GetString() ?? string.Empty,
                ImageUrl = item.GetProperty("node").GetProperty("main_picture").GetProperty("large").GetString() ?? string.Empty,
                EnglishTitle = item.GetProperty("node").GetProperty("alternative_titles").GetProperty("en").GetString() ?? string.Empty,
                Synopsis = item.GetProperty("node").GetProperty("synopsis").GetString() ?? string.Empty,
                Genres = item.GetProperty("node").GetProperty("genres").EnumerateArray().Select(genre => genre.GetProperty("name").GetString() ?? string.Empty).ToList(),
                Episodes = item.GetProperty("node").GetProperty("num_episodes").GetInt32(),
                MediaType = item.GetProperty("node").GetProperty("media_type").GetString() ?? string.Empty,
                NumListUsers = item.GetProperty("node").GetProperty("num_list_users").GetInt32(),
                Studios = item.GetProperty("node").GetProperty("studios").EnumerateArray().Select(studio => studio.GetProperty("name").GetString() ?? string.Empty).ToList(),
                StartSeason = item.GetProperty("node").GetProperty("start_season").GetProperty("season").GetString() ?? string.Empty,
                StartYear = item.GetProperty("node").GetProperty("start_season").GetProperty("year").GetInt32(),
            };

            if (item.GetProperty("node").TryGetProperty("rank", out var rank))
            {
                anime.Rank = rank.GetInt32();
            }

            if (item.GetProperty("node").TryGetProperty("mean", out var mean))
            {
                anime.Mean = mean.GetDouble();

            }

            res.Add(anime);
        }

        return res;
    }

    public async Task<List<Anime>> GetSuggestedAnimeAsync()
    {
        using var client = GetClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetAccessToken());

        var query = new Dictionary<string, string>
        {
            { "fields", QueryFields },
        };

        var uriString = GetUriWithQuery("anime/suggestions", query);

        Debug.WriteLine($"Requesting: {uriString}");

        HttpResponseMessage response = await client.GetAsync(uriString);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
        var prettyJson = JsonSerializer.Serialize(jsonElement, _jsonSerializerOptions);
        Debug.WriteLine(prettyJson);

        return GetFromJsonData(jsonResponse);
    }

    public async Task<List<Anime>> GetAnimeRankingAsync()
    {
        using var client = GetClient();

        var query = new Dictionary<string, string>
        {
            { "limit", "100" },
            { "fields", QueryFields }
        };

        var uriString = GetUriWithQuery("anime/ranking", query);

        HttpResponseMessage response = await client.GetAsync(uriString);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
        var prettyJson = JsonSerializer.Serialize(jsonElement, _jsonSerializerOptions);
        Debug.WriteLine(prettyJson);

        return GetFromJsonData(jsonResponse);
    }

    // 1 = January, 2 = February, ...
    private static string GetSeasonForMonth(int month)
    {
        return month switch
        {
            1 or 2 or 3 => "winter",
            4 or 5 or 6 => "spring",
            7 or 8 or 9 => "summer",
            10 or 11 or 12 => "fall",
            _ => "winter",
        };
    }

    public async Task<List<Anime>> GetSeasonAnimeAsync()
    {
        using var client = GetClient();

        var query = new Dictionary<string, string>
        {
            { "sort", "anime_num_list_users" },
            { "limit", "100" },
            { "fields", QueryFields }
        };

        var season = GetSeasonForMonth(DateTime.Now.Month);
        var year = DateTime.Now.Year;

        var uriString = GetUriWithQuery($"anime/season/{year}/{season}", query);

        HttpResponseMessage response = await client.GetAsync(uriString);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
        var prettyJson = JsonSerializer.Serialize(jsonElement, _jsonSerializerOptions);
        Debug.WriteLine(prettyJson);

        return GetFromJsonData(jsonResponse);
    }
}
