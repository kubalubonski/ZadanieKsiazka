
using Microsoft.EntityFrameworkCore;
namespace WypozyczalniaAPI.Entities;

public partial class RentalContext: DbContext
{
    public RentalContext(DbContextOptions<RentalContext> options)
        : base(options)
    {
        
        //Database.EnsureCreated();
        Database.Migrate();
        
        
    }
    public DbSet<Rental> Rentals => Set<Rental >();
    public DbSet<Customer> Customers => Set<Customer>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.Rentid).HasName("rental_pkey");

            entity.Property(e => e.Rentid)
                .ValueGeneratedOnAdd()
                .HasColumnName("rentid");
            
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Surname)
                    .HasColumnName("surname")
                    .HasMaxLength(255);
            entity.Property(e => e.RentDate)
                .HasColumnName("rent_date");

            entity.Property(e => e.Bookid)
                .HasColumnName("bookid");

            entity.HasOne(e => e.Customer)
                .WithMany(e => e.Rentals)
                .HasForeignKey(e=> e.Customerid).HasConstraintName("customerid")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Customerid).HasName("customer_pkey");

            entity.Property(e => e.Customerid)
                .ValueGeneratedOnAdd()
                .HasColumnName("customerid");
            
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.Property(e => e.Surname)
                    .HasColumnName("surname")
                    .HasMaxLength(255);

            entity.HasMany(e=> e.Rentals)
                    .WithOne(e => e.Customer)
                    .HasForeignKey(e => e.Customerid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
        });
        
        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //Database.EnsureCreated();
        //Database.Migrate();
        //optionsBuilder.UseSqlLite(_connectionString);
    }
}

