using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyAnimeListExtension;
internal sealed class OAuthClient : IDisposable
{
    private readonly HttpClient _httpClient;

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

    public static void HandleOAuthRedirection(Uri response)
    {
        var queryString = response.Query;
        var queryStringColleection = System.Web.HttpUtility.ParseQueryString(queryString);

        if (queryStringColleection["error"] != null)
        {
            // Handle error
            Debug.WriteLine($"Error: {queryStringColleection["error"]}");
            return;
        }

        if (queryStringColleection["code"] == null)
        {
            Debug.WriteLine("No code found in the response.");
            return;
        }

        // Handle success
        var code = queryStringColleection["code"];
        Debug.WriteLine($"Code: {code}");

        // Use the code to get the access token

    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
