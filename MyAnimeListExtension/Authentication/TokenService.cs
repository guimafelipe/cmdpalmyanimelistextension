using System.Net;

namespace MyAnimeListExtension.Authentication;

public sealed class TokenService
{
    private static readonly CredentialVault credentialVault = new();

    public static bool IsLoggedIn()
    {
        var accessToken = credentialVault.GetCredentials("MyAnimeList")?.Password;
        return !string.IsNullOrEmpty(accessToken);
    }

    public static string GetAccessToken()
    {
        var accessToken = credentialVault.GetCredentials("MyAnimeList")?.Password;
        return accessToken!;
    }

    public static void SaveOrOverwriteAccessToken(string accessToken)
    {
        credentialVault.SaveCredentials("MyAnimeList", new NetworkCredential(string.Empty, accessToken).SecurePassword);
    }

    public static void SaveOrOverwriteRefreshToken(string refreshToken)
    {
        credentialVault.SaveCredentials("MyAnimeListRefresh", new NetworkCredential(string.Empty, refreshToken).SecurePassword);
    }

    public static void LoginUser()
    {
        OAuthClient.AccessTokenChanged += OAuthTokenEventHandler;
        OAuthClient.BeginOAuthRequest();
    }

    public static void OAuthTokenEventHandler(object? sender, OAuthEventArgs e)
    {
        if(e.Error != null)
        {
            throw e.Error;
        }

        SaveOrOverwriteAccessToken(e.AccessToken!);
        SaveOrOverwriteRefreshToken(e.RefreshToken!);
    }
}
