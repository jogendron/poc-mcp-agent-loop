namespace QualiteDonneeREQ.Modeles;

public class Entreprise
{
    public Entreprise()
    {
    }

    public string neq { get; set; } = string.Empty;
    public AdresseNonStructuree? AdresseNonStructuree { get; set; }
    public AdresseStructuree? AdresseStructuree { get; set; }
}