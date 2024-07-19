using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OrderService.Models;

namespace OrderService.Models
{
    public partial class OrderContext:DbContext
    {
        
        public OrderContext(DbContextOptions<OrderContext> options)
            : base(options)
        {
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if (databaseCreator != null)
                {
                    if (!databaseCreator.CanConnect()) databaseCreator.Create();
                    if (!databaseCreator.HasTables()) databaseCreator.CreateTables();

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public virtual DbSet<Item> Items { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
      => optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=OrderManagementDB;Trusted_Connection=True;TrustServerCertificate=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Item__3214EC0770C3D564");

                entity.ToTable("Item");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
