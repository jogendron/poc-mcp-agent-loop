using Microsoft.EntityFrameworkCore;

namespace QualiteDonneeREQ.BaseDonnee;

public class ContexteBD : DbContext
{
    public ContexteBD(DbContextOptions<ContexteBD> options)
        : base(options)
    {
    }

    public DbSet<Entreprise> Entreprises => Set<Entreprise>();
    public DbSet<AdresseStructureeEntreprise> AdressesStructureesEntreprises =>
        Set<AdresseStructureeEntreprise>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entreprise>().ToTable(
            table => table.ExcludeFromMigrations()
        );
    }
}