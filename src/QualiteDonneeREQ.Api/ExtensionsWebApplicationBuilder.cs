using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;

using QualiteDonneeREQ.BaseDonnee;

public static class ExtensionsWebApplicationBuilder
{
    private class ConfigurationAutorisation
    {
        public string AdresseMetadonnees { get; set; } = string.Empty;

        public bool ObligerMetadonneesHttps { get; set; } = false;

        public string Audience { get; set; } = string.Empty;

        public string RoleLecture { get; set; } = string.Empty;

        public string RoleEcriture { get; set; } = string.Empty;
    }

    public static void AjouterAuthentification(this WebApplicationBuilder builder)
    {
        var configuration = new ConfigurationAutorisation();
        builder.Configuration.GetSection("Autorisation").Bind(configuration);

        builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.MetadataAddress = configuration.AdresseMetadonnees;
            options.RequireHttpsMetadata = false;
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
        });
    }

    public static void AjouterAutorisation(this WebApplicationBuilder builder)
    {
        var configuration = new ConfigurationAutorisation();
        builder.Configuration.GetSection("Autorisation").Bind(configuration);

        builder.Services.AddAuthorizationBuilder().AddPolicy(
            "acces_lecture", 
            politique =>
            {
                politique.RequireRole(configuration.RoleLecture);
            }
        );

        builder.Services.AddAuthorizationBuilder().AddPolicy(
            "acces_ecriture", 
            politique =>
            {
                politique.RequireRole(configuration.RoleEcriture);
            }
        );
    }

    public static void AjouterBaseDonnees(this WebApplicationBuilder builder)
    {
        var chaineConnexion = builder.Configuration.GetValue<string>("BaseDonnees:ChaineConnexion");
        if (string.IsNullOrWhiteSpace(chaineConnexion))
        {
            throw new InvalidOperationException(
                "La chaine de connexion de la base de donnees doit être configuree."
            );
        }

        builder.Services.AddDbContext<ContexteBD>(options =>
            options.UseNpgsql(chaineConnexion));
    }
}