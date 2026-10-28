using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Memory;

namespace ApiQualiteDonnees;

public sealed class GestionnaireJeton : DelegatingHandler
{
    private ConfigurationApi _configuration;
    private readonly IMemoryCache _cache;
    private readonly HttpClient _clientHttp;

    public GestionnaireJeton(ConfigurationApi configuration, IMemoryCache cache, HttpClient clientHttp)
    {
        _configuration = configuration;
        _cache = cache;
        _clientHttp = clientHttp;

        InnerHandler = new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        const string CLE_CACHE = "jeton_acces";

        _cache.TryGetValue(CLE_CACHE, out string? jeton);

        if (string.IsNullOrEmpty(jeton))
        {
            var semaphore = new SemaphoreSlim(1);
            await semaphore.WaitAsync();

            _cache.TryGetValue(CLE_CACHE, out jeton);

            if (string.IsNullOrEmpty(jeton))
            {
                var contenuRequete = new FormUrlEncodedContent(
                    new Dictionary<string,string>()
                    {
                        {"grant_type", "client_credentials"},
                        {"client_id", _configuration.ClientId},
                        {"client_secret", _configuration.ClientSecret}
                    }
                );
                var reponse = await _clientHttp.PostAsync(_configuration.AdresseEndpointJeton, contenuRequete);
                var contenuReponse = reponse.Content;
                var reponseJeton = await contenuReponse.ReadFromJsonAsync<ReponseJeton>();

                if (reponseJeton != null)
                {
                    jeton = reponseJeton.AccessToken;
                    _cache.Set(CLE_CACHE, jeton, DateTimeOffset.Now.AddSeconds(reponseJeton.ExpiresIn - 15));    
                }
                else
                {
                    throw new InvalidOperationException("Impossible d'obtenir un jeton d'authentification pour l'API de qualite de donnees");
                }
            }
        }

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", jeton);

        return await base.SendAsync(request, cancellationToken);
    }
}