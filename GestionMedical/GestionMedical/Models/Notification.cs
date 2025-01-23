using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionMedical.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime DateSent { get; set; }

        [Required]
        public string Status { get; set; }

        [ForeignKey("Medecin")]
        public int MedecinId { get; set; }

        public virtual Medecin Medecin { get; set; }
    }
}