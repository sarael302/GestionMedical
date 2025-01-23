using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionMedical.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Microsoft.AspNetCore.Authorization;

namespace GestionMedical.Controllers
{
    public class DossierMedicalsController : Controller
{
    private readonly GestionMedicalContext _context;

    public DossierMedicalsController(GestionMedicalContext context)
    {
        _context = context;
    }
        [Authorize]
        // GET: DossierMedicals
        // GET: DossierMedicals
        public async Task<IActionResult> Index(string search)
        {
            var dossiersMedicaux = _context.DossierMedicals
                .Include(d => d.Consultation)
                .Include(d => d.Medecin)
                .Include(d => d.Patient)
                .Include(d => d.Consultation.RendezVous)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                dossiersMedicaux = dossiersMedicaux.Where(d => d.Patient.Nom.Contains(search) || d.Patient.Prenom.Contains(search));
            }

            var result = await dossiersMedicaux
                .Select(d => new
                {
                    DossierMedicalId = d.DossierMedicalId,
                    PatientNomComplet = d.Patient != null ? $"{d.Patient.Nom} {d.Patient.Prenom}" : "N/A",
                    PatientDateNaissance = d.Patient != null ? d.Patient.DateNaissance : (DateOnly?)null,
                    MedecinNomComplet = d.Medecin != null ? $"{d.Medecin.Nom} {d.Medecin.Prenom}" : "N/A",
                    MedecinSpecialite = d.Medecin != null ? d.Medecin.Specialite : "N/A",
                    DateRendezVous = d.Consultation != null && d.Consultation.RendezVous != null ? d.Consultation.RendezVous.DateHeure : (DateTime?)null,
                    DetailConsultation = d.Consultation != null
                        ? $"Résultat: {d.Consultation.Resultat ?? "N/A"}\n\nOrdonnance: {d.Consultation.Ordonnance ?? "N/A"}\n\nOrdre Médical: {d.Consultation.OrdreMedical ?? "N/A"}"
                        : "Aucune consultation associée"
                })
                .ToListAsync();

            return View(result);
        }

        [Authorize]
        // GET: DossierMedicals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dossierMedical = await _context.DossierMedicals
                .Include(d => d.Consultation)
                .Include(d => d.Medecin)
                .Include(d => d.Patient)
                .Include(d => d.Consultation.RendezVous)
                .FirstOrDefaultAsync(m => m.DossierMedicalId == id);

            if (dossierMedical == null)
            {
                return NotFound();
            }

            return View(dossierMedical);
        }
        [Authorize]
        // GET: DossierMedicals/Create
        public IActionResult Create()
        {
            ViewData["ConsultationId"] = new SelectList(_context.Consultations, "ConsultationId", "ConsultationId");
            return View();
        }
        [Authorize]
        // POST: DossierMedicals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConsultationId")] DossierMedical dossierMedical)
        {
            if (ModelState.IsValid)
            {
                var consultation = await _context.Consultations
                    .Include(c => c.RendezVous)
                    .FirstOrDefaultAsync(c => c.ConsultationId == dossierMedical.ConsultationId);

                if (consultation == null)
                {
                    return NotFound();
                }

                dossierMedical.PatientId = consultation.RendezVous.PatientId;
                dossierMedical.MedecinId = consultation.RendezVous.MedecinId;

                _context.Add(dossierMedical);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConsultationId"] = new SelectList(_context.Consultations, "ConsultationId", "ConsultationId", dossierMedical.ConsultationId);
            return View(dossierMedical);
        }
        [Authorize]
        // GET: DossierMedicals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dossierMedical = await _context.DossierMedicals.FindAsync(id);
            if (dossierMedical == null)
            {
                return NotFound();
            }
            ViewData["ConsultationId"] = new SelectList(_context.Consultations, "ConsultationId", "ConsultationId", dossierMedical.ConsultationId);
            return View(dossierMedical);
        }
        [Authorize]
        // POST: DossierMedicals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DossierMedicalId,ConsultationId")] DossierMedical dossierMedical)
        {
            if (id != dossierMedical.DossierMedicalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDossier = await _context.DossierMedicals.FindAsync(id);
                    if (existingDossier == null)
                    {
                        return NotFound();
                    }

                    var consultation = await _context.Consultations
                        .Include(c => c.RendezVous)
                        .FirstOrDefaultAsync(c => c.ConsultationId == dossierMedical.ConsultationId);

                    if (consultation == null)
                    {
                        return NotFound();
                    }

                    existingDossier.ConsultationId = dossierMedical.ConsultationId;
                    existingDossier.PatientId = consultation.RendezVous.PatientId;
                    existingDossier.MedecinId = consultation.RendezVous.MedecinId;

                    _context.Update(existingDossier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DossierMedicalExists(dossierMedical.DossierMedicalId))
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
            ViewData["ConsultationId"] = new SelectList(_context.Consultations, "ConsultationId", "ConsultationId", dossierMedical.ConsultationId);
            return View(dossierMedical);
        }
        [Authorize]
        // GET: DossierMedicals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dossierMedical = await _context.DossierMedicals
                .Include(d => d.Consultation)
                .Include(d => d.Medecin)
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(m => m.DossierMedicalId == id);
            if (dossierMedical == null)
            {
                return NotFound();
            }

            return View(dossierMedical);
        }
        [Authorize]
        // POST: DossierMedicals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dossierMedical = await _context.DossierMedicals.FindAsync(id);
            if (dossierMedical != null)
            {
                _context.DossierMedicals.Remove(dossierMedical);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //pdf method
        public async Task<IActionResult> ExportToPdf(int id)
        {
            var dossierMedical = await _context.DossierMedicals
                .Include(d => d.Consultation)
                .Include(d => d.Medecin)
                .Include(d => d.Patient)
                .Include(d => d.Consultation.RendezVous)
                .FirstOrDefaultAsync(m => m.DossierMedicalId == id);

            if (dossierMedical == null)
            {
                return NotFound();
            }

            using (MemoryStream ms = new MemoryStream())
            {
                // Create document with larger margins for better readability
                Document document = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Add logo
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "cabinet.jpg");
                if (System.IO.File.Exists(logoPath))
                {
                    Image logo = Image.GetInstance(logoPath);
                    float pageWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                    float targetHeight = 60f; // Fixed height for consistency
                    logo.ScaleToFit(pageWidth * 0.3f, targetHeight); // 30% of page width
                    logo.SetAbsolutePosition(document.LeftMargin, document.PageSize.Height - document.TopMargin - targetHeight);
                    document.Add(logo);
                }

                // Add header with clinic info
                Font headerFont = FontFactory.GetFont("Arial", 10, Font.NORMAL, new BaseColor(108, 117, 125));
                Paragraph header = new Paragraph();
                header.Alignment = Element.ALIGN_RIGHT;
                header.Add(new Chunk("Cabinet Médical\n", headerFont));
                header.Add(new Chunk("123 Avenue de la Santé\n", headerFont));
                header.Add(new Chunk("Téléphone: +212 123456789\n", headerFont));
                document.Add(header);

                // Add spacing
                document.Add(new Paragraph("\n\n"));

                // Add title with custom styling
                Font titleFont = FontFactory.GetFont("Arial", 20, Font.BOLD, new BaseColor(37, 99, 235)); // Blue color
                Paragraph title = new Paragraph("Dossier Médical", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                document.Add(title);

                // Create styled separator
                LineSeparator line = new LineSeparator(1f, 100f, new BaseColor(229, 231, 235), Element.ALIGN_CENTER, -1);
                document.Add(new Chunk(line));
                document.Add(new Paragraph("\n"));

                // Create sections with better styling
                void AddSection(string title, PdfPTable content)
                {
                    Font sectionTitleFont = FontFactory.GetFont("Arial", 12, Font.BOLD, new BaseColor(31, 41, 55));
                    Paragraph sectionTitle = new Paragraph(title, sectionTitleFont);
                    sectionTitle.SpacingBefore = 15f;
                    sectionTitle.SpacingAfter = 10f;
                    document.Add(sectionTitle);
                    document.Add(content);
                }

                // Style for table cells
                Font labelFont = FontFactory.GetFont("Arial", 10, Font.BOLD, new BaseColor(107, 114, 128));
                Font valueFont = FontFactory.GetFont("Arial", 11, Font.NORMAL, new BaseColor(17, 24, 39));

                // Patient Information Section
                PdfPTable patientTable = new PdfPTable(2) { WidthPercentage = 100 };
                patientTable.SetWidths(new float[] { 1f, 2f });
                patientTable.DefaultCell.Border = Rectangle.NO_BORDER;
                patientTable.DefaultCell.PaddingBottom = 8f;

                void AddTableRow(PdfPTable table, string label, string value)
                {
                    table.AddCell(new PdfPCell(new Phrase(label, labelFont)) { Border = 0 });
                    table.AddCell(new PdfPCell(new Phrase(value, valueFont)) { Border = 0 });
                }

                AddTableRow(patientTable, "Patient:", $"{dossierMedical.Patient?.Nom} {dossierMedical.Patient?.Prenom}");
                AddTableRow(patientTable, "Date de naissance:", dossierMedical.Patient?.DateNaissance.ToString("dd/MM/yyyy"));
                AddSection("Informations du Patient", patientTable);

                // Doctor Information Section
                PdfPTable doctorTable = new PdfPTable(2) { WidthPercentage = 100 };
                doctorTable.SetWidths(new float[] { 1f, 2f });
                doctorTable.DefaultCell.Border = Rectangle.NO_BORDER;
                doctorTable.DefaultCell.PaddingBottom = 8f;

                AddTableRow(doctorTable, "Médecin:", $"{dossierMedical.Medecin?.Nom} {dossierMedical.Medecin?.Prenom}");
                AddTableRow(doctorTable, "Spécialité:", dossierMedical.Medecin?.Specialite);
                AddSection("Informations du Médecin", doctorTable);

                // Consultation Details Section
                PdfPTable consultationTable = new PdfPTable(2) { WidthPercentage = 100 };
                consultationTable.SetWidths(new float[] { 1f, 2f });
                consultationTable.DefaultCell.Border = Rectangle.NO_BORDER;
                consultationTable.DefaultCell.PaddingBottom = 8f;

                AddTableRow(consultationTable, "Date:", dossierMedical.Consultation?.RendezVous?.DateHeure.ToString("dd/MM/yyyy HH:mm"));
                AddTableRow(consultationTable, "Résultat:", dossierMedical.Consultation?.Resultat);
                AddTableRow(consultationTable, "Ordonnance:", dossierMedical.Consultation?.Ordonnance);
                AddTableRow(consultationTable, "Ordre Médical:", dossierMedical.Consultation?.OrdreMedical);
                AddSection("Détails de la Consultation", consultationTable);

                // Add footer with QR code and timestamp
                document.Add(new Paragraph("\n"));
                PdfPTable footer = new PdfPTable(2) { WidthPercentage = 100 };
                footer.DefaultCell.Border = Rectangle.NO_BORDER;

                // Add QR Code
                BarcodeQRCode qrCode = new BarcodeQRCode($"DossierMedical_{id}_PROJET_DOTNET", 50, 50, null);
                Image qrCodeImage = qrCode.GetImage();
                qrCodeImage.ScaleAbsolute(50f, 50f);
                footer.AddCell(new PdfPCell(qrCodeImage) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT });

                // Add timestamp
                Font footerFont = FontFactory.GetFont("Arial", 8, Font.ITALIC, new BaseColor(156, 163, 175));
                footer.AddCell(new PdfPCell(new Phrase($"Document généré le: {DateTime.Now:dd/MM/yyyy HH:mm}", footerFont))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    VerticalAlignment = Element.ALIGN_BOTTOM
                });

                document.Add(footer);

                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", $"DossierMedical_{id}.pdf");
            }
        }



        private bool DossierMedicalExists(int id)
        {
            return _context.DossierMedicals.Any(e => e.DossierMedicalId == id);
        }
    }
}

