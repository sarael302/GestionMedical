using System;
using System.Collections.Generic;

namespace GestionMedical.Models;

public partial class DossierMedical
{
    public int DossierMedicalId { get; set; }

    public int? PatientId { get; set; }

    public int? MedecinId { get; set; }

    public int? ConsultationId { get; set; }

    public virtual Consultation? Consultation { get; set; }

    public virtual Medecin? Medecin { get; set; }

    public virtual Patient? Patient { get; set; }
}
