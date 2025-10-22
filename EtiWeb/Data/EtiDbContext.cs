using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


namespace EtiWeb.Data;

public partial class EtiDbContext : DbContext
{
    public EtiDbContext()
    {
    }

    public EtiDbContext(DbContextOptions<EtiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Orders> Orders { get; set; }
    public virtual DbSet<Request> Requests { get; set; }
    public virtual DbSet<RequestFile> Files { get; set; }

    public virtual DbSet<SalesOrders> SalesOrders { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=LUISQUIROZ;Database=EtiflexDB;Trusted_Connection=True;TrustServerCertificate=True;");


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Request>(entity =>
        {
            entity.ToTable("Requests");

            entity.HasKey(e => e.PrintRequestId);

            entity.Property(e => e.RequestNumber).HasMaxLength(50);
            entity.Property(e => e.DeliveryType).HasMaxLength(50);
            entity.Property(e => e.ClientData_ClientName).HasMaxLength(100);
            entity.Property(e => e.ClientData_ClientCode).HasMaxLength(50); 
            entity.Property(e => e.ClientData_AddressLine1).HasMaxLength(200);
            entity.Property(e => e.ClientData_AddressLine2).HasMaxLength(200);
            entity.Property(e => e.ClientData_AddressLine3).HasMaxLength(200);
            entity.Property(e => e.ClientData_PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.ClientData_Attent).HasMaxLength(100);
            entity.Property(e => e.Notes).HasColumnType("nvarchar(max)");

            entity.HasMany(e => e.Files)
                  .WithOne(f => f.Request)
                  .HasForeignKey(f => f.PrintRequestId);
        });

        modelBuilder.Entity<RequestFile>(entity =>
        {
            entity.ToTable("RequestFile");

            entity.HasKey(e => e.PrintRequestId);

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Url).HasColumnType("nvarchar(max)");
        });


        modelBuilder.Entity<Orders>(entity =>
        {
            entity.ToTable("Orders");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Quantity);
            entity.Property(e => e.ClientName);
            entity.Property(e => e.Address);
        });



        modelBuilder.Entity<SalesOrders>(entity =>
        {
            entity.ToTable("SalesOrders");

            entity.Property(e => e.SalesOrderId).HasColumnName("id");
            entity.Property(e => e.ReferenceAtCustomer);
            entity.Property(e => e.LineComment1);
            entity.Property(e => e.ProductId);
            entity.Property(e => e.OrderQuantity);
            entity.Property(e => e.UnitPrice);
            entity.Property(e => e.AddressId);
            entity.Property(e => e.ExpectedDate);
            entity.Property(e => e.TransactionId);
            entity.Property(e => e.TransactionTime);
            entity.Property(e => e.TransactionAmount);
            entity.Property(e => e.TransactionMethod);
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.OrderState);
            

        });







        OnModelCreatingPartial(modelBuilder);
    }





    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
