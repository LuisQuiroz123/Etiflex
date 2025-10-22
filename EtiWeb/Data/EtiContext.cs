using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EtiWeb.Data
{
    public class EtiContext : DbContext
    {
        public EtiContext(DbContextOptions<EtiContext> options) : base(options)
        {
        }

        // Tablas
        public DbSet<Orders> orders { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestFile> RequestFiles { get; set; }

        public DbSet<SalesOrders> SalesOrders  { get; set; }

        // Vistas

        public DbSet<SalesOrderLine> SalesOrderLines { get; set; }

        public DbSet<RequestDetailView> vw_RequestDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indica que vw_RequestDetails no tiene clave primaria (es una vista SQL)
            modelBuilder.Entity<RequestDetailView>()
                .HasNoKey()
                .ToView("vw_RequestDetails");


            // Configurar SalesOrders
            modelBuilder.Entity<SalesOrders>(entity =>
            {
                entity.ToTable("SalesOrders");
                entity.HasKey(e => e.SalesOrderId);

                entity.HasMany(e => e.Lines)
                      .WithOne(l => l.SalesOrder)
                      .HasForeignKey(l => l.SalesOrderId);
            });

            // Configurar SalesOrderLine
            modelBuilder.Entity<SalesOrderLine>(entity =>
            {
                entity.ToTable("SalesOrderLines");
                entity.HasKey(e => e.Id);
            });




        }

    }

    // Modelo de ejemplo
    [Table("Orders", Schema = "Orders")]
    public class Orders
    {
        public int Id { get; set; }

        [Display(Name = "Código del Producto")]
        public string ProductId { get; set; } = string.Empty;
        [Display(Name = "Cantidad")]
        public int Quantity { get; set; }
        [Display(Name = "Cliente")]
        public string ClientName { get; set; } = string.Empty;
        [Display(Name = "Direccion")]
        public string Address { get; set; } = string.Empty;
    }


    //autenticacion 
    public class CreateUserViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsActive { get; set; } = true;

        public string? GivenName { get; set; }
        public string? Surename1 { get; set; }
        public string? Surename2 { get; set; }

        public List<string> SelectedRoles { get; set; } = new();
    }


    [Table("Requests", Schema = "dbo")]
    public class Request
    {
        [Key]
        [Column("PrintRequestId")]
        public Guid PrintRequestId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string RequestNumber { get; set; }

        [Required]
        [MaxLength(20)]
        public required string DeliveryType { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public required Guid ClientData_TransactionId { get; set; }

        [Required]
        [MaxLength(200)]
        public required string ClientData_ClientName { get; set; }

        [Required]
        [MaxLength(50)]
        public required string ClientData_ClientCode { get; set; }

        [MaxLength(100)]
        public string? ClientData_AddressLine1 { get; set; }

        [MaxLength(100)]
        public string? ClientData_AddressLine2 { get; set; }

        [MaxLength(100)]
        public string? ClientData_AddressLine3 { get; set; }

        [MaxLength(50)]
        public string? ClientData_PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? ClientData_Attent { get; set; }

        // Relación con archivos
        public virtual List<RequestFile> Files { get; set; } = new();
       // public List<RequestFile> Files { get; set; } = new List<RequestFile>();
    }

    [Table("RequestFiles", Schema = "dbo")]
    public class RequestFile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("PrintRequestId")]
        public Guid PrintRequestId { get; set; } // FK hacia Request

        [Required]
        [MaxLength(50)]
        public required string Type { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Name { get; set; }

        [Required]
        public int TotalLabels { get; set; }

        [MaxLength(500)]
        public string? Url { get; set; }

        // Relación con Request
        [ForeignKey("PrintRequestId")]
        public virtual Request Request { get; set; } = null!;
    }

    [Table("vw_RequestDetails")]
    public class RequestDetailView
    {
        public Guid PrintRequestId { get; set; }

        public string RequestNumber { get; set; } = string.Empty;

        public string DeliveryType { get; set; } = string.Empty;

        public string ClientData_ClientName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public int TotalLabels { get; set; }
    }


    public class SalesOrderViewModel
    {
        public Guid SalesOrderId { get; set; }
        public string ReferenceAtCustomer { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public int OrderQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TransactionMethod { get; set; } = string.Empty;
        public string OrderState { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Líneas de detalle
        public List<SalesOrderLineViewModel> Lines { get; set; } = new();
    }

    public class SalesOrderLineViewModel
    {
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }

    public class SalesByMonthVM
    {
        public string Month { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public class OrderStateVM
    {
        public string Estado { get; set; } = string.Empty;
        public int Total { get; set; }
    }



    // vistas


    [Table("SalesOrders")]
    public class SalesOrders
    {
        [Key]
        public Guid SalesOrderId { get; set; } = Guid.NewGuid();

        public string? ReferenceAtCustomer { get; set; }
        public string? LineComment1 { get; set; }
        public string? ProductId { get; set; }
        public int? OrderQuantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? AddressId { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public Guid? TransactionId { get; set; }
        public DateTime? TransactionTime { get; set; }
        public decimal? TransactionAmount { get; set; }
        public string? TransactionMethod { get; set; }
        public string? OrderState { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public virtual ICollection<SalesOrderLine> Lines { get; set; } = new List<SalesOrderLine>();



    }

    [Table("SalesOrderLines")]
    public class SalesOrderLine
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid SalesOrderId { get; set; } // FK a SalesOrders

        [Required, MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        // Relación inversa con la orden
        [ForeignKey("SalesOrderId")]
        public virtual SalesOrders SalesOrder { get; set; } = null!;
    }


    public class PrintLabelViewModel
    {
        public Guid PrintRequestId { get; set; }
        public string SelectedPrinterBrand { get; set; } = "Zebra"; 
        public List<string> AvailablePrinters { get; set; } = new List<string> { "Zebra", "Sato", "Godex" };
        public string ZplCode { get; set; } = string.Empty; 
    }




}