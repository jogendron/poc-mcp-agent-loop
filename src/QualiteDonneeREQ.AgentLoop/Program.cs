using Mcp;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using OllamaSharp;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

var configurationMcp = new ConfigurationMcp();
builder.Configuration.GetSection("ConfigurationMcp").Bind(configurationMcp);
builder.Services.AddSingleton(configurationMcp);
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IChatClient>(_ =>
{
    IChatClient ollamaClient = new OllamaApiClient(
        new Uri(builder.Configuration["ConfigurationOllama:Url"] ?? throw new ArgumentException("L'url du endpoint Ollama n'a pas ete renseignee")),
        builder.Configuration["ConfigurationOllama:Modele"] ?? throw new ArgumentException("Le modele a utiliser dans Ollama n'a pas ete renseigne"));

    return new ChatClientBuilder(ollamaClient)
        .UseFunctionInvocation()
        .Build();
});
builder.Services.AddHostedService<AgentLoopWorker>();

await builder.Build().RunAsync();

internal sealed class AgentLoopWorker(
    IChatClient chatClient,
    ConfigurationMcp configurationMcp,
    IMemoryCache cache,
    IConfiguration configuration,
    IHostApplicationLifetime applicationLifetime,
    ILogger<AgentLoopWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var tokenClient = new HttpClient();
        using var handler = new GestionnaireJeton(configurationMcp, cache, tokenClient);
        using var httpClient = new HttpClient(handler);

        var transport = new HttpClientTransport(
            new HttpClientTransportOptions
            {
                Endpoint = new Uri(
                    configuration["ConfigurationMcp:AdresseServeurMcp"] ?? throw new ArgumentException("L'adresse du serveur mcp n'a pas ete renseignee")),
                TransportMode = HttpTransportMode.StreamableHttp
            },
            httpClient
        );

        await using var mcpClient = await McpClient.CreateAsync(transport);

        IList<McpClientTool> tools = await mcpClient.ListToolsAsync();
        foreach (var tool in tools)
        {
            logger.LogInformation("{ToolName}: {ToolDescription}", tool.Name, tool.Description);
        }

        List<ChatMessage> messages =
        [
            new(ChatRole.System,
            """
                Tu es un agent de correction de données qui travaille pour le Registraire des Enterprises du Québec.
                Ton but est de structurer les adresses non structurées des entreprises.
                Une adresse non structurée est une adresse en texte libre sur 4 lignes.
                Une adresse structurée est un objet où chaque propriété de l'adresse est dans un champ séparé (pays, province, etc.)
                Utilise les outils MCP disponibles pour obtenir la prochaine entreprise dont l'adresse n'est pas structurée
                et pour insérer une adresse structurée (que tu auras toi-même produite à partir de l'adresse non structurée)
                dans l'entreprise.
                Ne prétends jamais avoir effectué une opération si elle a échoué.
            """
            ),
            new(ChatRole.User, "Structure l'adresse de la prochaine entreprise ayant une adresse non structurée.")
        ];

        while (! stoppingToken.IsCancellationRequested)
        {
            var response = await chatClient.GetResponseAsync(
                messages,
                new ChatOptions
                {
                    Tools = [.. tools]
                },
                stoppingToken
            );

            logger.LogInformation("{AgentResponse}", response.Text);
        }
        
        applicationLifetime.StopApplication();
    }
}
