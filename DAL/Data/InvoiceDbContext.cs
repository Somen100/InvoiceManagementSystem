using InvoiceMgmt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace InvoiceMgmt.DAL.Data
{
    public class InvoiceDbContext : DbContext
    {
        public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : base(options) { }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AuditTrail> AuditTrails { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RoleMaster> Roles { get; set; }
        public DbSet<InvoiceFile> InvoiceFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Convert InvoiceStatus enum to string
            modelBuilder.Entity<Invoice>()
                .Property(i => i.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), v)) 
                .IsRequired(); 

         
            modelBuilder.Entity<Invoice>()
                .HasQueryFilter(i => i.IsActive); 

            
            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique(); 

            modelBuilder.Entity<Customer>().HasQueryFilter(c => c.IsActive);
            modelBuilder.Entity<Product>().HasQueryFilter(p => p.IsActive);
            modelBuilder.Entity<InvoiceItem>().HasQueryFilter(ii => ii.IsActive);

            // Configure RoleMaster
            modelBuilder.Entity<RoleMaster>()
                .HasKey(r => r.RoleId);

            // Configure User
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<Customer>()
             .Property(c => c.CustomerId)
             .ValueGeneratedOnAdd();
        }


        public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries<Invoice>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.IsActive = true; // New invoices are active by default.
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow; // Update the timestamp.
            }
        }

        // serilog:
            var auditEntries = new List<AuditTrail>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditTrail || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var tableName = entry.Entity.GetType().Name;
                var operation = entry.State.ToString(); // Added, Modified, Deleted
                var changes = new Dictionary<string, object>();

                foreach (var property in entry.Properties)
                {
                    if (property.IsModified || entry.State == EntityState.Added || entry.State == EntityState.Deleted)
                    {
                        changes[property.Metadata.Name] = property.CurrentValue;
                    }
                }

                auditEntries.Add(new AuditTrail
                {
                    TableName = tableName,
                    Operation = operation,
                    Changes = System.Text.Json.JsonSerializer.Serialize(changes),
                    Timestamp = DateTime.UtcNow,
                    PerformedBy = "SYSTEM", // Replace with logged-in user info if available
                });
            }

            if (auditEntries.Any())
            {
                AuditTrails.AddRange(auditEntries);
            }
            //save
            return base.SaveChanges();
    }
//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

//        {

//            optionsBuilder.UseSqlServer("Server=sql.bsite.net\\MSSQL2016SQL; Initial Catalog=somen100_InvoiceDB; User Id=somen100_InvoiceDB; Password=sa; TrustServerCertificate=True;"
//);

//        }

    }
}

