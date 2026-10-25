using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QualiteDonneeREQ.BaseDonnee;

[Table("entreprise", Schema = "donnees_ouvertes")]
public class Entreprise
{
    [Key]
    public string neq { get; set; } = string.Empty;
    public bool? ind_fail { get; set; }
    public DateOnly? dat_immat { get; set; }
    public string? cod_regim_juri { get; set; }
    public string? cod_intval_emplo_que { get; set; }
    public DateOnly? dat_cess_prevu { get; set; }
    public string? cod_stat_immat { get; set; }
    public string? cod_forme_juri { get; set; }
    public DateOnly? dat_stat_immat { get; set; }
    public string? cod_regim_juri_consti { get; set; }
    public DateOnly? dat_depo_declr { get; set; }
    public int? an_decl { get; set; }
    public int? an_prod { get; set; }
    public DateOnly? dat_limit_prod { get; set; }
    public int? an_prod_pre { get; set; }
    public DateOnly? dat_limit_prod_pre { get; set; }
    public DateOnly? dat_maj_index_nom { get; set; }
    public string? cod_act_econ_cae { get; set; }
    public int? no_act_econ_assuj { get; set; }
    public string? desc_act_econ_assuj { get; set; }
    public string? cod_act_econ_cae2 { get; set; }
    public int? no_act_econ_assuj2 { get; set; }
    public string? desc_act_econ_assuj2 { get; set; }
    public string? nom_loclt_consti { get; set; }
    public DateOnly? dat_consti { get; set; }
    public bool? ind_conven_unmn_actnr { get; set; }
    public bool? ind_ret_tout_pouvr { get; set; }
    public bool? ind_limit_resp { get; set; }
    public DateOnly? dat_deb_resp { get; set; }
    public DateOnly? dat_fin_resp { get; set; }
    public string? objet_soc { get; set; }
    public string? no_mtr_volont { get; set; }
    public bool? adr_domcl_adr_disp { get; set; }
    public string? adr_domcl_lign1_adr { get; set; }
    public string? adr_domcl_lign2_adr { get; set; }
    public string? adr_domcl_lign3_adr { get; set; }
    public string? adr_domcl_lign4_adr { get; set; }
    public AdresseStructureeEntreprise? AdresseStructuree { get; set; }
}