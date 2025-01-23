using System;
using System.Collections.Generic;

namespace GestionMedical.Models;

public partial class Consultation
{
    public int ConsultationId { get; set; }

    public int? RendezVousId { get; set; }

    public decimal Prix { get; set; }

    public string? Resultat { get; set; }

    public string? Ordonnance { get; set; }

    public string? OrdreMedical { get; set; }

    public virtual ICollection<DossierMedical> DossierMedicals { get; set; } = new List<DossierMedical>();

    public virtual RendezVou? RendezVous { get; set; }
}
