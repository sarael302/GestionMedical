using System;
using System.Collections.Generic;

namespace GestionMedical.Models;

public partial class RendezVou
{
    public int RendezVousId { get; set; }

    public int? PatientId { get; set; }

    public int? MedecinId { get; set; }

    public DateTime DateHeure { get; set; }

    public string? Status { get; set; }
   

    public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();

    public virtual Medecin? Medecin { get; set; }

    public virtual Patient? Patient { get; set; }

}
