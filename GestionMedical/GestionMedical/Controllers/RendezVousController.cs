using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionMedical.Models;
using Microsoft.AspNetCore.Authorization;
namespace GestionMedical.Controllers
{
    public class RendezVousController : Controller
    {
        private readonly GestionMedicalContext _context;

        public RendezVousController(GestionMedicalContext context)
        {
            _context = context;
        }

        // GET: RendezVous
        [Authorize]
        public async Task<IActionResult> Index(string search)
        {
            var rendezVousQuery = _context.RendezVous
                .Include(r => r.Medecin)
                .Include(r => r.Patient)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                DateTime? searchDate = null;
                if (DateTime.TryParse(search, out var parsedDate))
                {
                    searchDate = parsedDate.Date;
                }

                rendezVousQuery = rendezVousQuery.Where(r =>
                    r.Status.ToLower().Contains(search) || 
                    (r.Medecin.Nom + " " + r.Medecin.Prenom).ToLower().Contains(search) || 
                    (searchDate != null && r.DateHeure.Date == searchDate) 
                );
            }

            ViewData["Search"] = search;
            return View(await rendezVousQuery.ToListAsync());
        }



        // GET: RendezVous/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rendezVou = await _context.RendezVous
                .Include(r => r.Medecin)
                .Include(r => r.Patient)
                .FirstOrDefaultAsync(m => m.RendezVousId == id);
            if (rendezVou == null)
            {
                return NotFound();
            }

            return View(rendezVou);
        }

        // GET: RendezVous/Create
        public IActionResult Create()
        {
            var availableMedecins = _context.Medecins
                                            .Where(m => m.Disponible)
                                            .Select(m => new { m.MedecinId, FullName = m.Nom + " " + m.Prenom })
                                            .ToList();

            ViewData["MedecinId"] = new SelectList(availableMedecins, "MedecinId", "FullName");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Nom");
            return View();
        }

        // POST: RendezVous/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RendezVousId,PatientId,MedecinId,DateHeure,Status")] RendezVou rendezVou)
        {
            // Check if the selected date is in the past
            if (rendezVou.DateHeure <= DateTime.Now)
            {
                ModelState.AddModelError("DateHeure", "La date du rendez-vous doit être dans le futur.");
            }

            // Check if the selected doctor is available
            if (!_context.Medecins.Any(m => m.MedecinId == rendezVou.MedecinId && m.Disponible))
            {
                ModelState.AddModelError("MedecinId", "Le médecin sélectionné n'est pas disponible.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(rendezVou);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var availableMedecins = _context.Medecins
                                            .Where(m => m.Disponible)
                                            .Select(m => new { m.MedecinId, FullName = m.Nom + " " + m.Prenom })
                                            .ToList();

            ViewData["MedecinId"] = new SelectList(availableMedecins, "MedecinId", "FullName", rendezVou.MedecinId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Nom", rendezVou.PatientId);
            return View(rendezVou);
        }


        // GET: RendezVous/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rendezVou = await _context.RendezVous.FindAsync(id);
            if (rendezVou == null)
            {
                return NotFound();
            }

            var availableMedecins = _context.Medecins
                                            .Where(m => m.Disponible)
                                            .Select(m => new { m.MedecinId, FullName = m.Nom + " " + m.Prenom })
                                            .ToList();

            ViewData["MedecinId"] = new SelectList(availableMedecins, "MedecinId", "FullName", rendezVou.MedecinId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Nom", rendezVou.PatientId);
            return View(rendezVou);
        }

        // POST: RendezVous/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RendezVousId,PatientId,MedecinId,DateHeure,Status")] RendezVou rendezVou)
        {
            if (id != rendezVou.RendezVousId)
            {
                return NotFound();
            }

            // Check if the selected date is in the past
            if (rendezVou.DateHeure <= DateTime.Now)
            {
                ModelState.AddModelError("DateHeure", "La date du rendez-vous doit être dans le futur.");
            }

            // Check if the selected doctor is available
            if (!_context.Medecins.Any(m => m.MedecinId == rendezVou.MedecinId && m.Disponible))
            {
                ModelState.AddModelError("MedecinId", "Le médecin sélectionné n'est pas disponible.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rendezVou);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RendezVouExists(rendezVou.RendezVousId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var availableMedecins = _context.Medecins
                                            .Where(m => m.Disponible)
                                            .Select(m => new { m.MedecinId, FullName = m.Nom + " " + m.Prenom })
                                            .ToList();

            ViewData["MedecinId"] = new SelectList(availableMedecins, "MedecinId", "FullName", rendezVou.MedecinId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Nom", rendezVou.PatientId);
            return View(rendezVou);
        }


        // GET: RendezVous/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rendezVou = await _context.RendezVous
                .Include(r => r.Medecin)
                .Include(r => r.Patient)
                .FirstOrDefaultAsync(m => m.RendezVousId == id);
            if (rendezVou == null)
            {
                return NotFound();
            }

            return View(rendezVou);
        }

        // POST: RendezVous/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rendezVou = await _context.RendezVous.FindAsync(id);
            if (rendezVou != null)
            {
                _context.RendezVous.Remove(rendezVou);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RendezVouExists(int id)
        {
            return _context.RendezVous.Any(e => e.RendezVousId == id);
        }
    }
}
