using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IsTakipSistemiMVC.Models;

public partial class IsTakipDbContext : DbContext
{
    public IsTakipDbContext()
    {
    }

    public IsTakipDbContext(DbContextOptions<IsTakipDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Birimler> Birimlers { get; set; }

    public virtual DbSet<Durumlar> Durumlars { get; set; }

    public virtual DbSet<Isler> Islers { get; set; }

    public virtual DbSet<Personeller> Personellers { get; set; }

    public virtual DbSet<YetkiTurler> YetkiTurlers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=IsTakipDb;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Birimler>(entity =>
        {
            entity.HasKey(e => e.BirimId);

            entity.ToTable("Birimler");

            entity.Property(e => e.BirimId).HasColumnName("birimId");
            entity.Property(e => e.BirimAd)
                .HasMaxLength(50)
                .HasColumnName("birimAd");
        });

        modelBuilder.Entity<Durumlar>(entity =>
        {
            entity.HasKey(e => e.DurumId);

            entity.ToTable("Durumlar");

            entity.Property(e => e.DurumId).HasColumnName("durumId");
            entity.Property(e => e.DurumAd)
                .HasMaxLength(50)
                .HasColumnName("durumAd");
        });

        modelBuilder.Entity<Isler>(entity =>
        {
            entity.HasKey(e => e.IsId);

            entity.ToTable("Isler");

            entity.Property(e => e.IsId).HasColumnName("isId");
            entity.Property(e => e.IletilenTarih)
                .HasColumnType("datetime")
                .HasColumnName("iletilenTarih");
            entity.Property(e => e.IsAciklama).HasColumnName("isAciklama");
            entity.Property(e => e.IsBaslik).HasColumnName("isBaslik");
            entity.Property(e => e.IsDurumId).HasColumnName("isDurumId");
            entity.Property(e => e.IsPersonelId).HasColumnName("isPersonelId");
            entity.Property(e => e.IsYorum).HasColumnName("isYorum");
            entity.Property(e => e.IsOkunma).HasColumnName("isOkunma");
            entity.Property(e => e.YapilanTarih)
                .HasColumnType("datetime")
                .HasColumnName("yapilanTarih");

            entity.HasOne(d => d.IsDurum).WithMany(p => p.Islers)
                .HasForeignKey(d => d.IsDurumId)
                .HasConstraintName("FK_Isler_Durumlar");

            entity.HasOne(d => d.IsPersonel).WithMany(p => p.Islers)
                .HasForeignKey(d => d.IsPersonelId)
                .HasConstraintName("FK_Isler_Personeller");
        });

        modelBuilder.Entity<Personeller>(entity =>
        {
            entity.HasKey(e => e.PersonelId);

            entity.ToTable("Personeller");

            entity.Property(e => e.PersonelId).HasColumnName("personelId");
            entity.Property(e => e.PersonelAdSoyad)
                .HasMaxLength(50)
                .HasColumnName("personelAdSoyad");
            entity.Property(e => e.PersonelBirimId).HasColumnName("personelBirimId");
            entity.Property(e => e.PersonelKullaniciAd)
                .HasMaxLength(50)
                .HasColumnName("personelKullaniciAd");
            entity.Property(e => e.PersonelParola)
                .HasMaxLength(50)
                .HasColumnName("personelParola");
            entity.Property(e => e.PersonelYetkiTurId).HasColumnName("personelYetkiTurId");
            entity.Property(e => e.PersonelTelefonNo)
                .HasMaxLength(10)
                .HasColumnName("personelTelefonNo");

            entity.HasOne(d => d.PersonelBirim).WithMany(p => p.Personellers)
                .HasForeignKey(d => d.PersonelBirimId)
                .HasConstraintName("FK_Personeller_Birimler");

            entity.HasOne(d => d.PersonelYetkiTur).WithMany(p => p.Personellers)
                .HasForeignKey(d => d.PersonelYetkiTurId)
                .HasConstraintName("FK_Personeller_yetkiTurler");
        });

        modelBuilder.Entity<YetkiTurler>(entity =>
        {
            entity.HasKey(e => e.YetkiTurId);

            entity.ToTable("yetkiTurler");

            entity.Property(e => e.YetkiTurId).HasColumnName("yetkiTurId");
            entity.Property(e => e.YetkiTurAd)
                .HasMaxLength(50)
                .HasColumnName("yetkiTurAd");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
