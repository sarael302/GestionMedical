using System;
using System.Collections.Generic;

namespace GestionMedical.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string? Adresse { get; set; }

    public DateOnly DateNaissance { get; set; }

    public string? Telephone { get; set; }

    public virtual ICollection<DossierMedical> DossierMedicals { get; set; } = new List<DossierMedical>();

    public virtual ICollection<RendezVou> RendezVous { get; set; } = new List<RendezVou>();
}
