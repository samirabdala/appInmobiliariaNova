using Microsoft.EntityFrameworkCore;

namespace inmobiliaria_AT.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Propietario> Propietario { get; set; }

        public DbSet<Contrato> Contrato { get; set; }

        public DbSet<Inquilino> Inquilino { get; set; }

        public DbSet<Inmueble> Inmueble { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tipo> Tipo { get; set; }

        public DbSet<Pago> Pago { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inmueble>()
                .HasOne(i => i.Tipo)
                .WithMany(t => t.Inmuebles)
                .HasForeignKey(i => i.TipoId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Contrato>()
                .HasOne(c => c.Prop)
                .WithMany(p => p.Contratos)
                .HasForeignKey(c => c.PropId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Contrato>()
                .HasOne(c => c.Inqui)                
                .WithMany(i => i.Contratos)          
                .HasForeignKey(c => c.InquiId)       
                .OnDelete(DeleteBehavior.Cascade);  

            modelBuilder.Entity<Contrato>()
                .HasOne(c => c.Inmu)                 
                .WithMany(i => i.Contratos)         
                .HasForeignKey(c => c.InmuId)       
                .OnDelete(DeleteBehavior.Cascade);  

            /* modelBuilder.Entity<Pago>()
             .HasOne(p => p.Concepto)  
             .WithMany(c => c.Pagos)      
             .HasForeignKey(p => p.ConceptoId)        
             .OnDelete(DeleteBehavior.Cascade);  

         modelBuilder.Entity<Pago>()
 .HasOne<Concepto>()
 .WithMany()
 .HasForeignKey(p => p.ConceptoId)
 .OnDelete(DeleteBehavior.Cascade);

*/

            modelBuilder.Entity<Inmueble>()
               .Property(i => i.IdPropietario)
               .HasColumnName("IdPropietario");

            modelBuilder.Entity<Contrato>()
               .Property(c => c.PropId)
               .HasColumnName("Prop");

            modelBuilder.Entity<Contrato>()
            .Property(c => c.InquiId)
            .HasColumnName("Inqui");

            modelBuilder.Entity<Pago>()
                       .Property(p => p.ConceptoId)
                       .HasColumnName("Concepto");

            modelBuilder.Entity<Contrato>()
                            .Property(c => c.InmuId)
                            .HasColumnName("Inmu");


        }




    }
}
