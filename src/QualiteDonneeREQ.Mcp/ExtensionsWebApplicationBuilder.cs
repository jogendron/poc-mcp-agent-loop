using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ModelContextProtocol.AspNetCore.Authentication;

public static class ExtensionsWebApplicationBuilder
{
    private class ConfigurationAutorisation
    {
        public string AdresseMetadonnees { get; set; } = string.Empty;

        public bool ObligerMetadonneesHttps { get; set; } = false;

        public string Audience { get; set; } = string.Empty;

        public string RoleEntreprise { get; set; } = string.Empty;
    }

    public static void AjouterAuthentification(this WebApplicationBuilder builder)
    {
        var configuration = new ConfigurationAutorisation();
        builder.Configuration.GetSection("Autorisation").Bind(configuration);

        builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = McpAuthenticationDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.MetadataAddress = configuration.AdresseMetadonnees;
            options.RequireHttpsMetadata = configuration.ObligerMetadonneesHttps;
            options.Audience = configuration.Audience;

            options.Events.OnTokenValidated = (contexte) =>
            {
                var identite = contexte.Principal?.Identity as ClaimsIdentity;
                var claim = identite?.FindFirst("resource_access");

                if (identite is not null && claim is not null)
                {
                    using var document = JsonDocument.Parse(claim.Value);

                    if (
                        document.RootElement.TryGetProperty(configuration.Audience, out var client) &&
                        client.TryGetProperty("roles", out var roles)
                    )
                    {
                        foreach (var role in roles.EnumerateArray())
                        {
                            if (role.GetString() is { } name)
                            {
                                identite.AddClaim(new Claim(ClaimTypes.Role, name));
                            }
                        }
                    }
                }

                return Task.CompletedTask;
            };
        })
        .AddMcp(options =>
        {
            options.ResourceMetadata = new()
            {
                AuthorizationServers = { DeriverAdresseAutorite(configuration.AdresseMetadonnees) }
            };
        });
    }

    public static void AjouterAutorisation(this WebApplicationBuilder builder)
    {
        var configuration = new ConfigurationAutorisation();
        builder.Configuration.GetSection("Autorisation").Bind(configuration);

        builder.Services.AddAuthorizationBuilder().AddPolicy(
            "amelioration_adresses_entreprises", 
            politique =>
            {
                politique.RequireRole(configuration.RoleEntreprise);
            }
        );
    }

    private static string DeriverAdresseAutorite(string adresseMetadonnees)
    {
        const string suffixeOpenIdConfiguration = "/.well-known/openid-configuration";

        if (!Uri.TryCreate(adresseMetadonnees, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException(
                "Autorisation:AdresseMetadonnees doit être une URL absolue."
            );
        }

        var chemin = uri.AbsolutePath.TrimEnd('/');
        if (!chemin.EndsWith(suffixeOpenIdConfiguration, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Autorisation:AdresseAutorite doit être configurée lorsque l'URL des métadonnées " +
                "ne se termine pas par /.well-known/openid-configuration."
            );
        }

        var adresseAutorite = new UriBuilder(uri)
        {
            Path = chemin[..^suffixeOpenIdConfiguration.Length],
            Query = string.Empty,
            Fragment = string.Empty
        };

        return adresseAutorite.Uri.AbsoluteUri.TrimEnd('/');
    }
}   