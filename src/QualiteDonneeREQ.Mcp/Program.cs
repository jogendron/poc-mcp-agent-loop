using System.ComponentModel;
using ApiQualiteDonnees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using ModelContextProtocol.Server;

using QualiteDonneeREQ.Modeles;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.AjouterAuthentification();
builder.AjouterAutorisation();

var configurationApi = new ConfigurationApi();
builder.Configuration.GetSection("ConfigurationApiQualiteDonnees").Bind(configurationApi);
builder.Services.AddSingleton(configurationApi);

builder.Services.AddMemoryCache();
builder.Services.AddMcpServer()
    .AddAuthorizationFilters()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .WithToolsFromAssembly();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapMcp().RequireAuthorization();

app.Run(builder.Configuration["Url"] ?? "http://localhost:3001");

[McpServerToolType]
[Authorize(Policy = "amelioration_adresses_entreprises")]
[Description("Obtient une entreprise du REQ avec une adresse non structuree")]
public sealed class AmeliorationAdressesEntreprisesTool
{
    private readonly ConfigurationApi _configurationApi;
    private HttpClient _clientHttp;

    public AmeliorationAdressesEntreprisesTool(
        ConfigurationApi configurationApi,
        IMemoryCache cache
    )
    {
        _configurationApi = configurationApi;
        _clientHttp = new HttpClient(new GestionnaireJeton(_configurationApi, cache, new HttpClient()));
    }

    [McpServerTool, Description("Obtient une entreprise avec une adresses non structuree")]
    public async Task<IEnumerable<Entreprise>> ObtenirEntrepriseAvecAdresseNonStructuree()
    {
        var url = _configurationApi.EndpointObtenirEntrepriseAvecAdresseNonStructuree;
        url += "&limite=1";

        var reponse = await _clientHttp.GetAsync(url);
        var entreprises = await reponse.Content.ReadFromJsonAsync<IEnumerable<Entreprise>>();

        return entreprises ?? throw new InvalidOperationException("Impossible de recuperer un lot d'entreprises");
    }

    [McpServerTool, Description("Assigne une adresse structuree a une entreprise")]
    public async Task InscrireAdresseStructureeEntreprise(
        string neq,
        AdresseStructuree adresseStructuree
    )
    {
        var url = _configurationApi.EndpointInscrireAdresseStructureeEntreprise;
        url = url.Replace("{neq}", neq);

        var reponse = await _clientHttp.PutAsJsonAsync(url, adresseStructuree);

        if (! reponse.IsSuccessStatusCode)
            throw new InvalidOperationException($"Impossible d'inscrire une adresse structuree pour l'entreprise {neq}");
    }
}
