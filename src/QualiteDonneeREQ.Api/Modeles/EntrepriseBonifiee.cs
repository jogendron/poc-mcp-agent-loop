namespace QualiteDonneeREQ.Modeles;

public class EntrepriseBonifiee
{
    public EntrepriseBonifiee()
    {
    }

    public EntrepriseBonifiee(BaseDonnee.Entreprise entrepriseBd)
    {
        ArgumentNullException.ThrowIfNull(entrepriseBd);

        neq = entrepriseBd.neq;
        ind_fail = entrepriseBd.ind_fail;
        dat_immat = entrepriseBd.dat_immat;
        cod_regim_juri = entrepriseBd.cod_regim_juri;
        cod_intval_emplo_que = entrepriseBd.cod_intval_emplo_que;
        dat_cess_prevu = entrepriseBd.dat_cess_prevu;
        cod_stat_immat = entrepriseBd.cod_stat_immat;
        cod_forme_juri = entrepriseBd.cod_forme_juri;
        dat_stat_immat = entrepriseBd.dat_stat_immat;
        cod_regim_juri_consti = entrepriseBd.cod_regim_juri_consti;
        dat_depo_declr = entrepriseBd.dat_depo_declr;
        an_decl = entrepriseBd.an_decl;
        an_prod = entrepriseBd.an_prod;
        dat_limit_prod = entrepriseBd.dat_limit_prod;
        an_prod_pre = entrepriseBd.an_prod_pre;
        dat_limit_prod_pre = entrepriseBd.dat_limit_prod_pre;
        dat_maj_index_nom = entrepriseBd.dat_maj_index_nom;
        cod_act_econ_cae = entrepriseBd.cod_act_econ_cae;
        no_act_econ_assuj = entrepriseBd.no_act_econ_assuj;
        desc_act_econ_assuj = entrepriseBd.desc_act_econ_assuj;
        cod_act_econ_cae2 = entrepriseBd.cod_act_econ_cae2;
        no_act_econ_assuj2 = entrepriseBd.no_act_econ_assuj2;
        desc_act_econ_assuj2 = entrepriseBd.desc_act_econ_assuj2;
        nom_loclt_consti = entrepriseBd.nom_loclt_consti;
        dat_consti = entrepriseBd.dat_consti;
        ind_conven_unmn_actnr = entrepriseBd.ind_conven_unmn_actnr;
        ind_ret_tout_pouvr = entrepriseBd.ind_ret_tout_pouvr;
        ind_limit_resp = entrepriseBd.ind_limit_resp;
        dat_deb_resp = entrepriseBd.dat_deb_resp;
        dat_fin_resp = entrepriseBd.dat_fin_resp;
        objet_soc = entrepriseBd.objet_soc;
        no_mtr_volont = entrepriseBd.no_mtr_volont;
        adr_domcl_adr_disp = entrepriseBd.adr_domcl_adr_disp;
        AdresseNonStructuree = new AdresseNonStructuree
        {
            adr_domcl_lign1_adr = entrepriseBd.adr_domcl_lign1_adr,
            adr_domcl_lign2_adr = entrepriseBd.adr_domcl_lign2_adr,
            adr_domcl_lign3_adr = entrepriseBd.adr_domcl_lign3_adr,
            adr_domcl_lign4_adr = entrepriseBd.adr_domcl_lign4_adr
        };

        if (entrepriseBd.AdresseStructuree is { } adresseStructureeBd)
        {
            AdresseStructuree = new AdresseStructuree
            {
                Pays = adresseStructureeBd.Pays,
                Province = adresseStructureeBd.Province,
                Ville = adresseStructureeBd.Ville,
                Rue = adresseStructureeBd.Rue,
                CodePostal = adresseStructureeBd.CodePostal,
                NumeroCivique = adresseStructureeBd.NumeroCivique,
                NumeroAppartement = adresseStructureeBd.NumeroAppartement
            };
        }
    }

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
    public AdresseNonStructuree? AdresseNonStructuree { get; set; }
    public AdresseStructuree? AdresseStructuree { get; set; }
}