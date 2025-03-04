using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyAnimeListExtension;
internal sealed class OAuthClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public static string AccessToken { get; private set; } = string.Empty;

    public OAuthClient()
    {
        _httpClient = new HttpClient();
    }

    private static Uri CreateOAuthRequestUri()
    {
        var client_id = Environment.GetEnvironmentVariable("MAL_CLIENT_ID");
        var code_challenge = Environment.GetEnvironmentVariable("MAL_CODE_CHALLENGE");
        var redirect_uri = "cmdpalmalext://oauth_redirect_uri/";
        return new Uri($"https://myanimelist.net/v1/oauth2/authorize?response_type=code&client_id={client_id}&code_challenge={code_challenge}&state=state&redirect_uri={redirect_uri}");
    }

    private static Uri CreateRequestTokenUri()
    {
        return new Uri($"https://myanimelist.net/v1/oauth2/token");
    }

    public static void BeginOAuthRequest()
    {
        var uri = CreateOAuthRequestUri();
        var options = new Windows.System.LauncherOptions();
        var browserLaunch = false;

        Task.Run(async () =>
        {
            browserLaunch = await Windows.System.Launcher.LaunchUriAsync(uri, options);

            if (browserLaunch)
            {
                Debug.WriteLine($"Uri Launched - Check browser");
            }
            else
            {
                Debug.WriteLine($"Uri Launch failed");
            }
        });
    }


    public static async Task HandleOAuthRedirection(Uri response)
    {
        var queryString = response.Query;
        var queryStringCollection = System.Web.HttpUtility.ParseQueryString(queryString);

        if (queryStringCollection["error"] != null)
        {
            // Handle error
            Debug.WriteLine($"Error: {queryStringCollection["error"]}");
            return;
        }

        if (queryStringCollection["code"] == null)
        {
            Debug.WriteLine("No code found in the response.");
            return;
        }

        // Handle success
        var code = queryStringCollection["code"];
        Debug.WriteLine($"Code: {code}");

        // Use the code to get the access token
        var tokenUri = CreateRequestTokenUri();

        var request = new HttpRequestMessage(HttpMethod.Post, tokenUri)
        {
            Content =
            new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code!),
                new KeyValuePair<string, string>("client_id", Environment.GetEnvironmentVariable("MAL_CLIENT_ID")!),
                new KeyValuePair<string, string>("code_verifier", Environment.GetEnvironmentVariable("MAL_CODE_CHALLENGE")!),
                new KeyValuePair<string, string>("redirect_uri", "cmdpalmalext://oauth_redirect_uri/"),
            }),
        };

        request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

        Debug.WriteLine($"Request: {request.RequestUri}");
        Debug.WriteLine($"Content: {await request.Content.ReadAsStringAsync()}");

        using var client = new HttpClient();
        try
        {
            var responseMessage = await client.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();

            var responseContent = await responseMessage.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent);

            var accessToken = responseJson.RootElement.GetProperty("access_token").GetString();
            var refreshToken = responseJson.RootElement.GetProperty("refresh_token").GetString();
            var expiresIn = responseJson.RootElement.GetProperty("expires_in").GetInt32();
            AccessToken = accessToken!;

            Debug.WriteLine($"Access Token: {accessToken}");
            Debug.WriteLine($"Refresh Token: {refreshToken}");
            Debug.WriteLine($"Expires In: {expiresIn}");
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"Request failed: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"JSON parsing failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unexpected error: {ex.Message}");
        }

    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
