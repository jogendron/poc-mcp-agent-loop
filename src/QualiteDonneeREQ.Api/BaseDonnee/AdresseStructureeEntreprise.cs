using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace QualiteDonneeREQ.BaseDonnee;

[Table("adresse_structuree_entreprise", Schema = "qualite_donnees")]
public class AdresseStructureeEntreprise
{
    [Key]
    [Column("neq")]
    [ForeignKey(nameof(Entreprise))]
    public string Neq { get; set; } = string.Empty;

    [Column("pays")]
    public string? Pays { get; set; }

    [Column("province")]
    public string? Province { get; set; }

    [Column("ville")]
    public string? Ville { get; set; }

    [Column("rue")]
    public string? Rue { get; set; }

    [Column("code_postal")]
    public string? CodePostal { get; set; }

    [Column("numero_civique")]
    public string? NumeroCivique { get; set; }

    [Column("numero_appartement")]
    public string? NumeroAppartement { get; set; }

    [JsonIgnore]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public Entreprise Entreprise { get; set; } = null!;
}
