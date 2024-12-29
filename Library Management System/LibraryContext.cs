using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System
{
    internal class LibraryContext : DbContext
    {
        public DbSet<Entity.Clients> Clients { get; set; }
        public DbSet<Entity.Address> Address { get; set; }
        public DbSet<Entity.Books> Books { get; set; }
        public DbSet<Entity.Loan> Loan { get; set; }

        public LibraryContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=ZALMAN\\SQLEXPRESSPZ;Database=Library;User Id=sa;Password=1234;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Settings
            modelBuilder.Entity<Entity.Clients>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Surname).IsRequired();
                entity.Property(e => e.Phone).IsRequired();
                entity.HasOne(d => d.Address)
                    .WithOne()
                    .HasForeignKey<Entity.Clients>(d => d.AddressId);
            });

            modelBuilder.Entity<Entity.Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.City).IsRequired();
                entity.Property(e => e.Street).IsRequired();
                entity.Property(e => e.Number).IsRequired();
            });

            modelBuilder.Entity<Entity.Books>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Author).IsRequired();
            });

            modelBuilder.Entity<Entity.Loan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LoanDate).IsRequired();
                entity.Property(e => e.ReturnDate).IsRequired();
                entity.HasOne(d => d.client).WithOne().HasForeignKey<Entity.Loan>(d => d.ClientId);
                entity.HasOne(d => d.book).WithOne().HasForeignKey<Entity.Loan>(d => d.BookId);
            });

            // Data
            modelBuilder.Entity<Entity.Clients>().HasData(
                new Entity.Clients { Id = 1, Name = "Piotr", Surname = "Zarzycki", Phone = "123456789", AddressId = 1 },
                new Entity.Clients { Id = 2, Name = "Jane", Surname = "Doe", Phone = "987654321", AddressId = 2 }
            );

            modelBuilder.Entity<Entity.Address>().HasData(
                new Entity.Address { Id = 1, City = "Warsaw", Street = "Złota", Number = "44" },
                new Entity.Address { Id = 2, City = "London", Street = "Oxford Street", Number = "102" }
            );

            modelBuilder.Entity<Entity.Books>().HasData(
                new Entity.Books { Id = 1, Title = "Pan Tadeusz", Author = "Adam Mickiewicz" },
                new Entity.Books { Id = 2, Title = "Harry Potter", Author = "J. K. Rowling" }
            );

            modelBuilder.Entity<Entity.Loan>().HasData(
                new Entity.Loan { Id = 1, ClientId = 1, BookId = 1, LoanDate = DateTime.Now, ReturnDate = DateTime.Now.AddDays(14), IsReturned = false },
                new Entity.Loan { Id = 2, ClientId = 2, BookId = 2, LoanDate = DateTime.Now, ReturnDate = DateTime.Now.AddDays(14), IsReturned = false }
            );
        }



    }
}
