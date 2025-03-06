using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyAnimeListExtension.Authentication;

public sealed class OAuthClient
{
    public event EventHandler<OAuthEventArgs>? AccessTokenChanged;

    public OAuthClient()
    {
    }

    private Uri CreateOAuthRequestUri()
    {
        var client_id = Environment.GetEnvironmentVariable("MAL_CLIENT_ID");
        var code_challenge = Environment.GetEnvironmentVariable("MAL_CODE_CHALLENGE");
        var redirect_uri = "cmdpalmalext://oauth_redirect_uri/";
        return new Uri($"https://myanimelist.net/v1/oauth2/authorize?response_type=code&client_id={client_id}&code_challenge={code_challenge}&state=state&redirect_uri={redirect_uri}");
    }

    private Uri CreateRequestTokenUri()
    {
        return new Uri($"https://myanimelist.net/v1/oauth2/token");
    }

    public void BeginOAuthRequest()
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

    public async Task RefreshAccessToken(string refreshToken)
    {
        // Use the code to get the access token
        var tokenUri = CreateRequestTokenUri();

        var request = new HttpRequestMessage(HttpMethod.Post, tokenUri)
        {
            Content =
            new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", Environment.GetEnvironmentVariable("MAL_CLIENT_ID")!),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
            }),
        };

        request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
        using var client = new HttpClient();

        var responseMessage = await client.SendAsync(request);
        responseMessage.EnsureSuccessStatusCode();

        var responseContent = await responseMessage.Content.ReadAsStringAsync();
        var responseJson = JsonDocument.Parse(responseContent);

        var accessToken = responseJson.RootElement.GetProperty("access_token").GetString();
        var newRefreshToken = responseJson.RootElement.GetProperty("refresh_token").GetString();
        var expiresIn = responseJson.RootElement.GetProperty("expires_in").GetInt32();

        AccessTokenChanged?.Invoke(null, new OAuthEventArgs(accessToken, refreshToken));
    }

    public async Task HandleOAuthRedirection(Uri response)
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

            Debug.WriteLine($"Access Token: {accessToken}");
            Debug.WriteLine($"Refresh Token: {refreshToken}");
            Debug.WriteLine($"Expires In: {expiresIn}");

            AccessTokenChanged?.Invoke(null, new OAuthEventArgs(accessToken, refreshToken));
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
}
