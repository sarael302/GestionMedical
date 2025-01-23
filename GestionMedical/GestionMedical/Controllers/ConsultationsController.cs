using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionMedical.Models;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace GestionMedical.Controllers
{
    public class ConsultationsController : Controller
    {
        private readonly GestionMedicalContext _context;

        public ConsultationsController(GestionMedicalContext context)
        {
            _context = context;
        }

        // GET: Consultations
        [Authorize]
        public async Task<IActionResult> Index(string search)
        {
            ViewData["CurrentFilter"] = search;

            var consultations = await _context.Consultations
                .Include(c => c.RendezVous)
                    .ThenInclude(r => r.Patient)
                .Include(c => c.RendezVous)
                    .ThenInclude(r => r.Medecin)
                .ToListAsync();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                consultations = consultations.Where(c =>
                    c.RendezVous.Patient.Nom.ToLower().Contains(search) ||
                    c.RendezVous.Medecin.Nom.ToLower().Contains(search) ||
                    c.OrdreMedical.ToLower().Contains(search) ||
                    SearchByDate(c.RendezVous.DateHeure, search) ||
                    SearchByTime(c.RendezVous.DateHeure, search)
                ).ToList();
            }

            return View(consultations);
        }

        private bool SearchByDate(DateTime dateTime, string search)
        {
            // Try to parse the search string as a year
            if (int.TryParse(search, out int year) && year >= 1900 && year <= 9999)
            {
                return dateTime.Year == year;
            }

            // Try to parse the search string as a month and year (MM/yyyy or yyyy/MM)
            string[] parts = search.Split('/');
            if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out year))
                {
                    return dateTime.Year == year && dateTime.Month == month;
                }
                if (int.TryParse(parts[1], out month) && int.TryParse(parts[0], out year))
                {
                    return dateTime.Year == year && dateTime.Month == month;
                }
            }

            // Try to parse the search string as a month name or number
            if (int.TryParse(search, out int monthNumber) && monthNumber >= 1 && monthNumber <= 12)
            {
                return dateTime.Month == monthNumber;
            }

            // Try to parse the search string as a month name
            for (int i = 1; i <= 12; i++)
            {
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).ToLower();
                if (monthName.StartsWith(search))
                {
                    return dateTime.Month == i;
                }
            }

            return false;
        }

        private bool SearchByTime(DateTime dateTime, string search)
        {
            // Search by hour
            if (int.TryParse(search, out int hour) && hour >= 0 && hour <= 23)
            {
                return dateTime.Hour == hour;
            }

            // Search by hour:minute
            string[] timeParts = search.Split(':');
            if (timeParts.Length == 2 && int.TryParse(timeParts[0], out int searchHour) && int.TryParse(timeParts[1], out int searchMinute))
            {
                return dateTime.Hour == searchHour && dateTime.Minute == searchMinute;
            }

            return false;
        }

        [Authorize]
        // GET: Consultations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultation = await _context.Consultations
    .Include(c => c.RendezVous)
        .ThenInclude(r => r.Patient)
    .Include(c => c.RendezVous)
        .ThenInclude(r => r.Medecin)
    .FirstOrDefaultAsync(m => m.ConsultationId == id);

            if (consultation == null)
            {
                return NotFound();
            }

            return View(consultation);
        }
        [Authorize]
        // GET: Consultations/Create
        public IActionResult Create(int? rendezVousId)
        {
            var consultation = new Consultation();
            if (rendezVousId.HasValue)
            {
                var rendezVous = _context.RendezVous
                    .Include(r => r.Patient)
                    .Include(r => r.Medecin)
                    .FirstOrDefault(r => r.RendezVousId == rendezVousId && r.Status == "Confirmé");

                if (rendezVous != null)
                {
                    consultation.RendezVous = rendezVous;
                    consultation.RendezVousId = rendezVous.RendezVousId;
                }
                else
                {
                    return NotFound("Rendez-vous not found or not confirmed.");
                }
            }

            ViewData["RendezVousId"] = new SelectList(_context.RendezVous.Where(r => r.Status == "Confirmé"), "RendezVousId", "RendezVousId");
            ViewData["OrdreMedicalOptions"] = new SelectList(new List<string> { "Analyse", "Radio", "CertificatMedical" });
            return View(consultation);
        }

        [Authorize]
        // POST: Consultations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConsultationId,RendezVousId,Prix,Resultat,Ordonnance,OrdreMedical")] Consultation consultation)
        {
            if (ModelState.IsValid)
            {
                var rendezVous = await _context.RendezVous.FindAsync(consultation.RendezVousId);
                if (rendezVous != null && rendezVous.Status == "Confirmé")
                {
                    _context.Add(consultation);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("RendezVousId", "Invalid or unconfirmed Rendez-vous.");
                }
            }
            ViewData["RendezVousId"] = new SelectList(_context.RendezVous.Where(r => r.Status == "Confirmé"), "RendezVousId", "RendezVousId", consultation.RendezVousId);
            return View(consultation);
        }
        [Authorize]
        // GET: Consultations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultation = await _context.Consultations
                .Include(c => c.RendezVous)
                    .ThenInclude(r => r.Patient)
                .Include(c => c.RendezVous)
                    .ThenInclude(r => r.Medecin)
                .FirstOrDefaultAsync(m => m.ConsultationId == id);

            if (consultation == null)
            {
                return NotFound();
            }

            ViewData["RendezVousId"] = new SelectList(_context.RendezVous.Where(r => r.Status == "Confirmé"), "RendezVousId", "RendezVousId", consultation.RendezVousId);
            ViewData["OrdreMedicalOptions"] = new SelectList(new List<string> { "Analyse", "Radio", "CertificatMedical" }, consultation.OrdreMedical);
            return View(consultation);
        }


        [Authorize]
        // POST: Consultations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ConsultationId,RendezVousId,Prix,Resultat,Ordonnance,OrdreMedical")] Consultation consultation)
        {
            if (id != consultation.ConsultationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingConsultation = await _context.Consultations
                        .Include(c => c.RendezVous)
                            .ThenInclude(r => r.Patient)
                        .Include(c => c.RendezVous)
                            .ThenInclude(r => r.Medecin)
                        .FirstOrDefaultAsync(c => c.ConsultationId == id);

                    if (existingConsultation == null)
                    {
                        return NotFound();
                    }

                    existingConsultation.Prix = consultation.Prix;
                    existingConsultation.Resultat = consultation.Resultat;
                    existingConsultation.Ordonnance = consultation.Ordonnance;
                    existingConsultation.OrdreMedical = consultation.OrdreMedical;

                    _context.Update(existingConsultation);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsultationExists(consultation.ConsultationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            var updatedConsultation = await _context.Consultations
                .Include(c => c.RendezVous)
                    .ThenInclude(r => r.Patient)
                .Include(c => c.RendezVous)
                    .ThenInclude(r => r.Medecin)
                .FirstOrDefaultAsync(c => c.ConsultationId == id);

            ViewData["RendezVousId"] = new SelectList(_context.RendezVous.Where(r => r.Status == "Confirmé"), "RendezVousId", "RendezVousId", updatedConsultation.RendezVousId);
            ViewData["OrdreMedicalOptions"] = new SelectList(new List<string> { "Analyse", "Radio", "CertificatMedical" }, updatedConsultation.OrdreMedical);
            return View(updatedConsultation);
        }
        [Authorize]
        // GET: Consultations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultation = await _context.Consultations
    .Include(c => c.RendezVous)
        .ThenInclude(r => r.Patient)
    .Include(c => c.RendezVous)
        .ThenInclude(r => r.Medecin)
    .FirstOrDefaultAsync(m => m.ConsultationId == id);

            if (consultation == null)
            {
                return NotFound();
            }

            return View(consultation);
        }
        [Authorize]
        // POST: Consultations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation != null)
            {
                _context.Consultations.Remove(consultation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConsultationExists(int id)
        {
            return _context.Consultations.Any(e => e.ConsultationId == id);
        }
    }
}
