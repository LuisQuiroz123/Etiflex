using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace WebApi.Data
{
    public partial class EtiDbContext : DbContext
    {
        public EtiDbContext()
        {
        }

        public EtiDbContext(DbContextOptions<EtiDbContext> options)
            : base(options)
        {
        }

        //public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<PrintRequest> PrintRequests { get; set; } //datos impresion
        public virtual DbSet<RequestFile> RequestFiles { get; set; } //archivos de impresion

        public virtual DbSet<PrintStatus> PrintStatuses { get; set; } //estatus

        public virtual DbSet<SalesOrder> SalesOrders { get; set; } //ordenes de venta
        public DbSet<SalesOrderLine> SalesOrderLines { get; set; }





        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("Server=LUISQUIROZ;Database=EtiflexDB;Trusted_Connection=True;TrustServerCertificate=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // PrintRequest
            modelBuilder.Entity<PrintRequest>(entity =>
            {
                entity.ToTable("Requests");
                entity.HasKey(e => e.PrintRequestId);
                entity.Property(e => e.RequestNumber).HasMaxLength(50).IsRequired();
                entity.Property(e => e.DeliveryType).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.OwnsOne(e => e.ClientData);
                entity.HasMany(e => e.RequestFiles)
                      .WithOne(f => f.PrintRequest)
                      .HasForeignKey(f => f.PrintRequestId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // RequestFile
            modelBuilder.Entity<RequestFile>(entity =>
            {
                entity.ToTable("RequestFiles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Url).HasColumnType("nvarchar(max)").IsRequired();
            });

            // RequestFile
            modelBuilder.Entity<PrintStatus>(entity =>
            {
                entity.ToTable("PrintStatuses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Code).IsRequired();
                entity.Property(e => e.TrackingId).HasMaxLength(100);
            });


            modelBuilder.Entity<SalesOrder>(entity =>
            {
                entity.ToTable("SalesOrders");
                entity.HasKey(e => e.SalesOrderId);
                entity.Property(e => e.ReferenceAtCustomer).HasMaxLength(50).IsRequired();
                entity.Property(e => e.LineComment1).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ProductId).HasMaxLength(50).IsRequired();
                entity.Property(e => e.OrderQuantity).IsRequired();


            });

            modelBuilder.Entity<SalesOrder>()
               .HasMany(o => o.InvoiceLines)
               .WithOne(l => l.SalesOrder)
               .HasForeignKey(l => l.SalesOrderId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SalesOrderLine>(entity =>
            {
                entity.ToTable("SalesOrderLines");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InvoicePriceLineId).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UnitPrice).IsRequired();
                entity.Property(e => e.Type).HasMaxLength(50).IsRequired();
            });


            //modelBuilder.Entity<Orders>(entity =>
            //{
            //    entity.ToTable("Orders");

            //    entity.Property(e => e.Id).HasColumnName("id");
            //    entity.Property(e => e.ProductId).HasMaxLength(50).IsUnicode(false);
            //    entity.Property(e => e.Quantity);
            //    entity.Property(e => e.ClientName);
            //    entity.Property(e => e.Address);
            //});

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
