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
    public class PersonnelMedicalsController : Controller
    {
        private readonly GestionMedicalContext _context;

        public PersonnelMedicalsController(GestionMedicalContext context)
        {
            _context = context;
        }

        // GET: PersonnelMedicals
        // GET: PersonnelMedicals
        [Authorize]
        public async Task<IActionResult> Index(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var filteredPersonnel = await _context.PersonnelMedicals
                    .Where(p => (p.Nom + " " + p.Prenom).Contains(search) || p.Fonction.Contains(search))
                    .ToListAsync();
                return View(filteredPersonnel);
            }

            return View(await _context.PersonnelMedicals.ToListAsync());
        }


        // GET: PersonnelMedicals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personnelMedical = await _context.PersonnelMedicals
                .FirstOrDefaultAsync(m => m.PersonnelId == id);
            if (personnelMedical == null)
            {
                return NotFound();
            }

            return View(personnelMedical);
        }

        // GET: PersonnelMedicals/Create
        public IActionResult Create()
        {
            ViewData["FonctionOptions"] = new SelectList(new List<string>
            {
                "Infirmier/Infirmière",
                "Sage-Femme",
                "Aide-Soignant(e)",
                "Secrétaire Médicale",
                "Technicien(ne) de Laboratoire",
                "Radiologue (Technicien)",
                "Pharmacien(ne)",
                "Assistant(e) dentaire",
                "Brancardier",
                "Responsable d'Hygiène et Stérilisation"
            });

            return View();
        }

        // POST: PersonnelMedicals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonnelId,Nom,Prenom,Fonction")] PersonnelMedical personnelMedical)
        {
            if (ModelState.IsValid)
            {
                // HoraireParDefaut is set automatically based on the Fonction
                _context.Add(personnelMedical);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FonctionOptions"] = new SelectList(new List<string>
            {
                "Infirmier/Infirmière",
                "Sage-Femme",
                "Aide-Soignant(e)",
                "Secrétaire Médicale",
                "Technicien(ne) de Laboratoire",
                "Radiologue (Technicien)",
                "Pharmacien(ne)",
                "Assistant(e) dentaire",
                "Brancardier",
                "Responsable d'Hygiène et Stérilisation"
            }, personnelMedical.Fonction);
            return View(personnelMedical);
        }

        // GET: PersonnelMedicals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personnelMedical = await _context.PersonnelMedicals.FindAsync(id);
            if (personnelMedical == null)
            {
                return NotFound();
            }
            ViewData["FonctionOptions"] = new SelectList(new List<string>
            {
                "Infirmier/Infirmière",
                "Sage-Femme",
                "Aide-Soignant(e)",
                "Secrétaire Médicale",
                "Technicien(ne) de Laboratoire",
                "Radiologue (Technicien)",
                "Pharmacien(ne)",
                "Assistant(e) dentaire",
                "Brancardier",
                "Responsable d'Hygiène et Stérilisation"
            }, personnelMedical.Fonction);
            return View(personnelMedical);
        }

        // POST: PersonnelMedicals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonnelId,Nom,Prenom,Fonction")] PersonnelMedical personnelMedical)
        {
            if (id != personnelMedical.PersonnelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // HoraireParDefaut will be updated automatically based on the new Fonction
                    _context.Update(personnelMedical);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonnelMedicalExists(personnelMedical.PersonnelId))
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
            ViewData["FonctionOptions"] = new SelectList(new List<string>
            {
                "Infirmier/Infirmière",
                "Sage-Femme",
                "Aide-Soignant(e)",
                "Secrétaire Médicale",
                "Technicien(ne) de Laboratoire",
                "Radiologue (Technicien)",
                "Pharmacien(ne)",
                "Assistant(e) dentaire",
                "Brancardier",
                "Responsable d'Hygiène et Stérilisation"
            }, personnelMedical.Fonction);
            return View(personnelMedical);
        }

        // GET: PersonnelMedicals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personnelMedical = await _context.PersonnelMedicals
                .FirstOrDefaultAsync(m => m.PersonnelId == id);
            if (personnelMedical == null)
            {
                return NotFound();
            }

            return View(personnelMedical);
        }

        // POST: PersonnelMedicals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personnelMedical = await _context.PersonnelMedicals.FindAsync(id);
            if (personnelMedical != null)
            {
                _context.PersonnelMedicals.Remove(personnelMedical);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonnelMedicalExists(int id)
        {
            return _context.PersonnelMedicals.Any(e => e.PersonnelId == id);
        }
    }
}