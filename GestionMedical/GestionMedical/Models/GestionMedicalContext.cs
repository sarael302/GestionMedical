using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GestionMedical.Models;

namespace GestionMedical.Models;

public partial class GestionMedicalContext : DbContext
{
    public GestionMedicalContext()
    {
    }

    public GestionMedicalContext(DbContextOptions<GestionMedicalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Consultation> Consultations { get; set; }

    public virtual DbSet<DossierMedical> DossierMedicals { get; set; }

    public virtual DbSet<Medecin> Medecins { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PersonnelMedical> PersonnelMedicals { get; set; }

    public virtual DbSet<RendezVou> RendezVous { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-SH8RC4M; Database=gestionMedical;Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Consultation>(entity =>
        {
            entity.HasKey(e => e.ConsultationId).HasName("PK__Consulta__5D014A98BFD65BE3");

            entity.ToTable("Consultation");

            entity.Property(e => e.OrdreMedical).HasMaxLength(50);
            entity.Property(e => e.Prix).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.RendezVous).WithMany(p => p.Consultations)
                .HasForeignKey(d => d.RendezVousId)
                .HasConstraintName("FK__Consultat__Rende__440B1D61");
        });

        modelBuilder.Entity<DossierMedical>(entity =>
        {
            entity.HasKey(e => e.DossierMedicalId).HasName("PK__DossierM__19050E45D417C5A4");

            entity.ToTable("DossierMedical");

            entity.HasOne(d => d.Consultation).WithMany(p => p.DossierMedicals)
                .HasForeignKey(d => d.ConsultationId)
                .HasConstraintName("FK__DossierMe__Consu__49C3F6B7");

            entity.HasOne(d => d.Medecin).WithMany(p => p.DossierMedicals)
                .HasForeignKey(d => d.MedecinId)
                .HasConstraintName("FK__DossierMe__Medec__48CFD27E");

            entity.HasOne(d => d.Patient).WithMany(p => p.DossierMedicals)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__DossierMe__Patie__47DBAE45");
        });

        modelBuilder.Entity<Medecin>(entity =>
        {
            entity.HasKey(e => e.MedecinId).HasName("PK__Medecin__69D27AFB3CD03740");

            entity.ToTable("Medecin");

            entity.Property(e => e.Nom).HasMaxLength(100);
            entity.Property(e => e.Prenom).HasMaxLength(100);
            entity.Property(e => e.Specialite).HasMaxLength(100);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patient__970EC366A7AAEC07");

            entity.ToTable("Patient");

            entity.Property(e => e.Adresse).HasMaxLength(255);
            entity.Property(e => e.Nom).HasMaxLength(100);
            entity.Property(e => e.Prenom).HasMaxLength(100);
            entity.Property(e => e.Telephone).HasMaxLength(15);
        });

        modelBuilder.Entity<PersonnelMedical>(entity =>
        {
            entity.HasKey(e => e.PersonnelId).HasName("PK__Personne__CAFBCB4FE8DEC8B9");

            entity.ToTable("PersonnelMedical");

            entity.Property(e => e.Fonction).HasMaxLength(100);
            entity.Property(e => e.Nom).HasMaxLength(100);
            entity.Property(e => e.Prenom).HasMaxLength(100);
        });

        modelBuilder.Entity<RendezVou>(entity =>
        {
            entity.HasKey(e => e.RendezVousId).HasName("PK__RendezVo__C4B748C7D003D9D7");

            entity.Property(e => e.DateHeure).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Medecin).WithMany(p => p.RendezVous)
                .HasForeignKey(d => d.MedecinId)
                .HasConstraintName("FK__RendezVou__Medec__3F466844");

            entity.HasOne(d => d.Patient).WithMany(p => p.RendezVous)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__RendezVou__Patie__3E52440B");
        });
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PKNotific4E3E04AD7882750B");

            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
            entity.Property(e => e.DateSent).HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(false);

            entity.HasOne(d => d.Medecin).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.MedecinId)
                .HasConstraintName("FKNotificMedec__51F18345");
        });



        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

public DbSet<GestionMedical.Models.Notification> Notification { get; set; } = default!;
}
