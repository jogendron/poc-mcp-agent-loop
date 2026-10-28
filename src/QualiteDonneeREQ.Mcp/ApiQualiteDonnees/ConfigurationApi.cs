namespace ApiQualiteDonnees;

public class ConfigurationApi
{
    public string AdresseEndpointJeton { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string EndpointObtenirEntrepriseAvecAdresseNonStructuree { get; set; } = string.Empty;

    public string EndpointInscrireAdresseStructureeEntreprise { get; set; } = string.Empty;
}