using System;
using System.Collections.Generic;

namespace GestionMedical.Models;

public partial class PersonnelMedical
{
    public int PersonnelId { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string? Fonction { get; set; }
    public string HoraireParDefaut
    {
        get
        {
            return Fonction switch
            {
                "Infirmier/Infirmière" => "8h00 - 16h00",
                "Sage-Femme" => "9h00 - 17h00",
                "Aide-Soignant(e)" => "7h00 - 15h00",
                "Secrétaire Médicale" => "8h30 - 16h30",
                "Technicien(ne) de Laboratoire" => "9h00 - 17h00",
                "Radiologue (Technicien)" => "8h00 - 14h00",
                "Pharmacien(ne)" => "9h00 - 18h00",
                "Assistant(e) dentaire" => "10h00 - 18h00",
                "Brancardier" => "7h00 - 14h00",
                "Responsable d'Hygiène et Stérilisation" => "8h00 - 15h00",
                _ => "Non spécifié"
            };
        }
    }

}
