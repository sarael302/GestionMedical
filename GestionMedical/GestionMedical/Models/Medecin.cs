using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace GestionMedical.Models;

public partial class Medecin
{
    public int MedecinId { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string? Specialite { get; set; }

    public bool Disponible  { get; set; }

    public DateOnly? DateRetour { get; set; }

    public string NomComplet
    {
        get { return $"{Nom} {Prenom}"; }
    }
    public virtual ICollection<DossierMedical> DossierMedicals { get; set; } = new List<DossierMedical>();

    public virtual ICollection<RendezVou> RendezVous { get; set; } = new List<RendezVou>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();


}
