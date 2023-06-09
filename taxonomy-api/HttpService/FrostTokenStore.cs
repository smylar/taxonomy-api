using System.Runtime.CompilerServices;
using API.Taxonomy.Domain.Model;

namespace taxonomy_api.HttpService;

public class FrostTokenStore
{
    private string currentToken = string.Empty; //may want to encrypt??
    private long tokenExpiryTime = 0;

    public string GetToken(Func<AuthResponse?> getter)
    {
        string token = currentToken;
        // avoid possible reset call between expired check and returning the token setting to empty if not synchronised
        return HasTokenExpired() ? GetNewToken(getter) : token;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void ResetToken(string tokenUsed)
    {
        if (tokenUsed.Equals(currentToken))
        {
            tokenExpiryTime = 0;
            currentToken = string.Empty;
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private string GetNewToken(Func<AuthResponse?> getter)
    {
        if (HasTokenExpired())
        {
            AuthResponse? authResponse = getter.Invoke();
            if (authResponse != null && authResponse.Token != null)
            {
                string token = authResponse.Token;
                currentToken = token;
                tokenExpiryTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + authResponse.ExpiresInSeconds - 30;
            }
        }
        return currentToken;
    }

    private bool HasTokenExpired()
        => DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= tokenExpiryTime || currentToken == string.Empty;
}

