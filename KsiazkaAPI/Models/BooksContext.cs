using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KsiazkaAPI.Models;

public partial class BooksContext : DbContext
{
    private string _connectionString = "Host=localhost;Database=Books;Username=postgres;Password=Lebronjames1*";


    public BooksContext(DbContextOptions<BooksContext> options)
        : base(options)
    {
        
    }

    public DbSet<Book> Books => Set<Book >();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Bookid).HasName("book_pkey");

            entity.ToTable("book");

            entity.Property(e => e.Bookid)
                .ValueGeneratedOnAdd()
                .HasColumnName("bookid");
            entity.Property(e => e.Author)
                .HasMaxLength(255)
                .HasColumnName("author");
            entity.Property(e => e.ReleaseDate).HasColumnName("release_date");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseNpgsql(_connectionString);
    }
}
