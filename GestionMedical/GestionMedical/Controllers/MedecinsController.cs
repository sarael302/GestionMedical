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
    public class MedecinsController : Controller
    {
        private readonly GestionMedicalContext _context;

        public MedecinsController(GestionMedicalContext context)
        {
            _context = context;
        }

        // GET: Medecins
        // GET: Medecins
        [Authorize]
        public async Task<IActionResult> Index(string search)
        {
            // Récupérer tous les médecins
            var medecins = _context.Medecins.AsQueryable();

            // Filtrer si une recherche est saisie
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                medecins = medecins.Where(m =>
                    (m.Nom + " " + m.Prenom).ToLower().Contains(search) || 
                    (m.Specialite != null && m.Specialite.ToLower().Contains(search)) || 
                    (search == "disponible" && m.Disponible) || 
                    (search == "non disponible" && !m.Disponible) 
                );
            }

            ViewData["CurrentFilter"] = search;

            return View(await medecins.ToListAsync());
        }


        // GET: Medecins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medecin = await _context.Medecins
                .FirstOrDefaultAsync(m => m.MedecinId == id);
            if (medecin == null)
            {
                return NotFound();
            }

            return View(medecin);
        }

        // GET: Medecins/Create
        public IActionResult Create()
        {
            ViewData["SpecialiteOptions"] = new SelectList(new List<string>
            {
                "Médecine Générale",
                "Cardiologie",
                "Dermatologie",
                "Gynécologie",
                "Pédiatrie",
                "Radiologie",
                "Chirurgie Générale",
                "Ophtalmologie",
                "Neurologie",
                "Orthopédie"
            });

            return View(new Medecin { Disponible = true });
        }

        // POST: Medecins/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedecinId,Nom,Prenom,Specialite,Disponible,DateRetour")] Medecin medecin)
        {
            if (medecin.Disponible)
            {
                medecin.DateRetour = null;
            }

            if (ModelState.IsValid)
            {
                _context.Add(medecin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medecin);
        }

        // GET: Medecins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medecin = await _context.Medecins.FindAsync(id);
            if (medecin == null)
            {
                return NotFound();
            }

            ViewData["SpecialiteOptions"] = new SelectList(new List<string>
            {
                "Médecine Générale",
                "Cardiologie",
                "Dermatologie",
                "Gynécologie",
                "Pédiatrie",
                "Radiologie",
                "Chirurgie Générale",
                "Ophtalmologie",
                "Neurologie",
                "Orthopédie"
            });

            return View(medecin);
        }

        // POST: Medecins/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedecinId,Nom,Prenom,Specialite,Disponible,DateRetour")] Medecin medecin)
        {
            if (id != medecin.MedecinId)
            {
                return NotFound();
            }

            if (medecin.Disponible)
            {
                medecin.DateRetour = null;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medecin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedecinExists(medecin.MedecinId))
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
            return View(medecin);
        }

        // GET: Medecins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medecin = await _context.Medecins
                .FirstOrDefaultAsync(m => m.MedecinId == id);
            if (medecin == null)
            {
                return NotFound();
            }

            return View(medecin);
        }

        // POST: Medecins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medecin = await _context.Medecins.FindAsync(id);
            if (medecin != null)
            {
                _context.Medecins.Remove(medecin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedecinExists(int id)
        {
            return _context.Medecins.Any(e => e.MedecinId == id);
        }
    }
}
