using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAnimeListExtension.Authentication;

public sealed class OAuthEventArgs : EventArgs
{
    public string? AccessToken { get;  }
    
    public string? RefreshToken { get; }

    public Exception? Error { get; }

    public OAuthEventArgs(string? token, string? refreshToken, Exception? error = null)
    {
        AccessToken = token;
        RefreshToken = refreshToken;
        Error = error;
    }
}
