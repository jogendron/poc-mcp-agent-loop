using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualiteDonneeREQ.BaseDonnee;
using QualiteDonneeREQ.Modeles;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddCors();
builder.AjouterAuthentification();
builder.AjouterAutorisation();
builder.AjouterBaseDonnees();

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/v1/entreprises", async Task<IResult> (
    ContexteBD contexteBD,
    [FromQuery(Name = "statut-adresse")] string statutAdresse = "tous", 
    string neq = "",
    int skip = 0,
    int limite = 10
) => 
{
    try
    {
        string[] statutsAutorises = {"tous", "structuree", "non-structuree"};
        statutAdresse = statutAdresse.ToLower().Trim();

        if (! statutsAutorises.Contains(statutAdresse))
            throw new ArgumentException($"Le statut de l'adresse doit être une des valeurs suivantes : {string.Join(",", statutsAutorises)}.");

        if (limite < 0 || limite > 1000)
            throw new ArgumentException("La limite doit être entre 0 et 1000");

        IQueryable<Entreprise> entreprises = contexteBD.Entreprises
            .Include(entreprise => entreprise.AdresseStructuree);

        switch (statutAdresse)
        {
            case "tous":
                break;

            case "structuree":
                entreprises = entreprises.Where(entreprise => entreprise.AdresseStructuree != null);
                break;

            case "non-structuree":
                entreprises = entreprises.Where(entreprise => entreprise.AdresseStructuree == null);
                break;
        }

        if (! string.IsNullOrEmpty(neq))
            entreprises = entreprises.Where(e => e.neq == neq);

        entreprises = entreprises.OrderBy(e => e.neq);

        var entreprisesBd = await entreprises.Skip(skip).Take(limite).ToListAsync();

        return Results.Ok(entreprisesBd.Select(e => new QualiteDonneeREQ.Modeles.EntrepriseBonifiee(e)));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { erreur = ex.Message });
    }
    catch (Exception)
    {
        return Results.Problem("Une erreur interne est survenue.");
    }    
}).RequireAuthorization("acces_lecture");

app.MapPut("/api/v1/entreprises/{neq}/adresse-structuree", async Task<IResult>(
    ContexteBD contexteBD,
    string neq,
    [FromBody] AdresseStructuree adresseStructuree
) => {
    try
    {
        ArgumentNullException.ThrowIfNull(adresseStructuree);

        var entreprise = await contexteBD.Entreprises
            .Include(e => e.AdresseStructuree)
            .FirstOrDefaultAsync(e => e.neq == neq);

        if (entreprise is null)
            return Results.NotFound(new { erreur = $"Aucune entreprise trouvée pour le NEQ {neq}." });

        var adresse = entreprise.AdresseStructuree;
        if (adresse is null)
        {
            adresse = new AdresseStructureeEntreprise
            {
                Neq = entreprise.neq
            };
            entreprise.AdresseStructuree = adresse;
        }

        adresse.Pays = adresseStructuree.Pays;
        adresse.Province = adresseStructuree.Province;
        adresse.Ville = adresseStructuree.Ville;
        adresse.Rue = adresseStructuree.Rue;
        adresse.CodePostal = adresseStructuree.CodePostal;
        adresse.NumeroCivique = adresseStructuree.NumeroCivique;
        adresse.NumeroAppartement = adresseStructuree.NumeroAppartement;

        await contexteBD.SaveChangesAsync();

        return Results.Ok(adresseStructuree);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { erreur = ex.Message });
    }
    catch (Exception)
    {
        return Results.Problem("Une erreur interne est survenue.");
    }
}).RequireAuthorization("acces_ecriture");

app.MapDelete("/api/v1/entreprises/{neq}/adresse-structuree", async Task<IResult>(
    ContexteBD contexteBD,
    string neq
) =>
{
    try
    {
        var entreprise = await contexteBD.Entreprises
            .Include(e => e.AdresseStructuree)
            .FirstOrDefaultAsync(e => e.neq == neq);

        if (entreprise is null)
            return Results.NotFound(new { erreur = $"Aucune entreprise trouvée pour le NEQ {neq}." });

        if (entreprise.AdresseStructuree is null)
            return Results.NotFound(new { erreur = $"Aucune adresse structurée trouvée pour le NEQ {neq}." });

        contexteBD.AdressesStructureesEntreprises.Remove(entreprise.AdresseStructuree);
        await contexteBD.SaveChangesAsync();

        return Results.NoContent();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { erreur = ex.Message });
    }
    catch (Exception)
    {
        return Results.Problem("Une erreur interne est survenue.");
    }
}).RequireAuthorization("acces_ecriture");

app.Run();