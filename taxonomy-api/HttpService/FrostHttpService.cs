using System.Net;
using System.Net.Http.Headers;
using API.Taxonomy.Domain.Model;

namespace taxonomy_api.HttpService;

public class FrostHttpService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FrostHttpService> _logger;
    private readonly FrostTokenStore _tokenStore;
    private readonly string _baseAddress;
    private readonly string _tokenAddress;
    private readonly string _frostOrg;

    public FrostHttpService(HttpClient httpClient, ILogger<FrostHttpService> logger, FrostTokenStore tokenStore)
    {
        (_httpClient, _logger, _tokenStore) = (httpClient, logger, tokenStore);

        _baseAddress = _httpClient.BaseAddress != null ? _httpClient.BaseAddress.AbsoluteUri : string.Empty;
        _tokenAddress = Environment.GetEnvironmentVariable("FROST_TOKEN_URL") ?? string.Empty;
        _frostOrg = Environment.GetEnvironmentVariable("FROST_ORG") ?? string.Empty;
    }

    public virtual async Task<Taxonomy?> GetTaxonomyByIdAsync(Guid taxonomyId)
    {
        var token = GetToken();

        if (token == string.Empty)
        {
            return null;
        }
 
        try
        {
            var uri = new Uri($"{_baseAddress}v2/taxonomy/getTreeHierarchyV5/{taxonomyId}");
            var httpResponse = await DoGet(uri, token, 0);

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await httpResponse.Content.ReadFromJsonAsync<FrostResponse>();
                if (response != null &&
                    response.Data != null &&
                    response.Data.Children != null)
                {
                    return response.Data.Children.Single();
                }
            }
            else
            {
                string responseContent = await httpResponse.Content.ReadAsStringAsync();
                LogWarnTaxonomy(taxonomyId, httpResponse.StatusCode, responseContent);
            }
        }
        catch (Exception ex)
        {
            LogErrorTaxonomy(taxonomyId, ex);
        }
        return null;
    }

    public virtual async Task<FrostAssociationResponse?> GetAssociations(Guid taxonomyId)
    {
        var token = GetToken();

        if (token == string.Empty)
        {
            return null;
        }

        try
        {
            var uri = new Uri($"{_baseAddress}v1/{_frostOrg}/ims/case/v1p0/CFPackages/{taxonomyId}");
            var httpResponse = await DoGet(uri, token, 0);

            if (httpResponse.IsSuccessStatusCode)
            {
                return await httpResponse.Content.ReadFromJsonAsync<FrostAssociationResponse>();
            }
            else
            {
                string responseContent = await httpResponse.Content.ReadAsStringAsync();
                LogWarnAssociation(taxonomyId, httpResponse.StatusCode, responseContent);
            }
        }
        catch (Exception ex)
        {
            LogErrorAssociation(taxonomyId, ex);
        }
        return null;
    }

    private async Task<HttpResponseMessage> DoGet(Uri uri, string token, int attempts)
    {
        var request = new HttpRequestMessage()
        {
            RequestUri = uri,
            Method = HttpMethod.Get,
        };
        request.Headers.Add("Authorization", token);
        var httpResponse = await _httpClient.SendAsync(request);

        if (httpResponse.StatusCode.Equals(HttpStatusCode.Unauthorized) && attempts < 2)
        {
            _tokenStore.ResetToken(token); //reset and retry once incase token revoked
            var newToken = GetToken();
            if (newToken != string.Empty) { 
                return await DoGet(uri, newToken, ++attempts);
            }
        }

        return httpResponse;
    }

    private string GetToken()
    {
        return _tokenStore.GetToken(GetNewTokenRequest);
    }

    private AuthResponse? GetNewTokenRequest()
    {
        var requestData = new AuthRequest();
        if (requestData.ClientId == string.Empty || requestData.ClientSecret == string.Empty || _tokenAddress == string.Empty)
        {
            _logger.LogError("Token configuration missing");
            return null;
        }

        HttpContent httpContent = new FormUrlEncodedContent(new List<KeyValuePair<string,string>>()
        {
            KeyValuePair.Create("client_id",  requestData.ClientId),
            KeyValuePair.Create("client_secret",  requestData.ClientSecret),
            KeyValuePair.Create("grant_type",  "client_credentials")
        });

        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        var tokenResponse = _httpClient.PostAsync(_tokenAddress, httpContent).Result;

        if (tokenResponse.IsSuccessStatusCode)
        {
            return tokenResponse.Content.ReadFromJsonAsync<AuthResponse>().Result;
        }
        string errorContent = tokenResponse.Content.ReadAsStringAsync().Result;
        LogErrorToken(tokenResponse.StatusCode, errorContent);
        return null;
    }

    private void LogWarnTaxonomy(Guid id, HttpStatusCode statusCode, string content)
    {
        _logger.LogWarning("Could not get associations for taxonomy {id}, API returned {statusCode} {content}", id, statusCode, content);
    }

    private void LogWarnAssociation(Guid id, HttpStatusCode statusCode, string content)
    {
        _logger.LogWarning("Could not get associations for taxonomy {id}, API returned {statusCode} {content}", id, statusCode, content);
    }

    private void LogErrorToken(HttpStatusCode statusCode, string content)
    {
        _logger.LogError("Could not get a token, {statusCode} {content}", statusCode, content);
    }

    private void LogErrorTaxonomy(Guid id, Exception ex)
    {
        _logger.LogError(ex, "Could not get taxonomy {id}", id);
    }

    private void LogErrorAssociation(Guid id, Exception ex)
    {
        _logger.LogError(ex, "Could not get associations for taxonomy {id}", id);
    }
}

