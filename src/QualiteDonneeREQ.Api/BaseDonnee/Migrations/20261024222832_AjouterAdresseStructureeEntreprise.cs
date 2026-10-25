using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualiteDonneeREQ.Api.BaseDonnee.Migrations
{
    /// <inheritdoc />
    public partial class AjouterAdresseStructureeEntreprise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "qualite_donnees");

            migrationBuilder.CreateTable(
                name: "adresse_structuree_entreprise",
                schema: "qualite_donnees",
                columns: table => new
                {
                    neq = table.Column<string>(type: "text", nullable: false),
                    pays = table.Column<string>(type: "text", nullable: true),
                    province = table.Column<string>(type: "text", nullable: true),
                    ville = table.Column<string>(type: "text", nullable: true),
                    rue = table.Column<string>(type: "text", nullable: true),
                    code_postal = table.Column<string>(type: "text", nullable: true),
                    numero_civique = table.Column<string>(type: "text", nullable: true),
                    numero_appartement = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adresse_structuree_entreprise", x => x.neq);
                    table.ForeignKey(
                        name: "FK_adresse_structuree_entreprise_entreprise_neq",
                        column: x => x.neq,
                        principalSchema: "donnees_ouvertes",
                        principalTable: "entreprise",
                        principalColumn: "neq",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adresse_structuree_entreprise",
                schema: "qualite_donnees");
        }
    }
}
