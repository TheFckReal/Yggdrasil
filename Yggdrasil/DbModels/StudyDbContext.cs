using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Yggdrasil.DbModels;

public partial class StudyDbContext : DbContext
{

    IConfiguration _configuration { get; init; }
    public StudyDbContext(IConfiguration conf)
    {
        _configuration = conf;
    }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<Homework> Homeworks { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_configuration["connectionStr"]);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<File>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Files_pk");

            entity.ToTable("files");

            entity.HasOne(d => d.Homework).WithMany(p => p.Files)
                .HasForeignKey(d => d.HomeworkId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Files_homework_id_fk");
        });

        modelBuilder.Entity<Homework>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("homework_pkey");

            entity.ToTable("homework");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Deadline)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deadline");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Finished).HasColumnName("finished");
            entity.Property(e => e.Subjectid).HasColumnName("subjectid");

            entity.HasOne(d => d.Subject).WithMany(p => p.Homeworks)
                .HasForeignKey(d => d.Subjectid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("subjectkey");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("subjects_pkey");

            entity.ToTable("subjects");

            entity.HasIndex(e => e.Name, "subjects_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ending).HasColumnName("ending");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Teacher)
                .HasColumnType("character varying")
                .HasColumnName("teacher");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
