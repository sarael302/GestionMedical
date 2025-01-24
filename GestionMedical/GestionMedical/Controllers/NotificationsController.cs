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
    public class NotificationsController : Controller
    {
        private readonly GestionMedicalContext _context; 

        public NotificationsController(GestionMedicalContext context)
        {
            _context = context;
        }

        // GET: Notifications
        [Authorize] // Ensure the user is authenticated
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.Name != "admin@gmail.com") // admin email
            {
                return Forbid(); // Return a 403 Forbidden response if not the admin
            }

            var today = DateTime.Today;
            var rendezVous = await _context.RendezVous
                .Include(r => r.Medecin)
                .Include(r => r.Patient)
                .Where(r => r.DateHeure.Date >= today)
                .ToListAsync();

            var notifications = rendezVous.Select(r => new Notification
            {
                Message = $"Vous avez un rendez-vous programmé pour le {r.DateHeure.ToString("dd/MM/yyyy")} à {r.DateHeure.ToString("HH:mm")} avec le patient {r.Patient.Nom} {r.Patient.Prenom}.",
                DateSent = DateTime.Now,
                Status = "Sent",
                MedecinId = r.MedecinId.Value,
                Medecin = r.Medecin
            }).ToList();

            return View(notifications);
        }

        // GET: Notifications/Create
        public IActionResult Create()
        {
            ViewData["MedecinId"] = new SelectList(_context.Medecins, "MedecinId", "NomComplet");
            var notification = new Notification
            {
                DateSent = DateTime.Now,
                Status = "Sent"
            };
            return View(notification);
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NotificationId,Message,MedecinId")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                notification.DateSent = DateTime.Now;
                notification.Status = "Sent";
                _context.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedecinId"] = new SelectList(_context.Medecins, "MedecinId", "NomComplet", notification.MedecinId);
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            ViewData["MedecinId"] = new SelectList(_context.Medecins, "MedecinId", "NomComplet", notification.MedecinId);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NotificationId,Message,DateSent,Status,MedecinId")] Notification notification)
        {
            if (id != notification.NotificationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.NotificationId))
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
            ViewData["MedecinId"] = new SelectList(_context.Medecins, "MedecinId", "NomComplet", notification.MedecinId);
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(m => m.NotificationId == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.NotificationId == id);
        }
    }
}

