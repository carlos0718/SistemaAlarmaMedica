using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace Infraestructura.ContextoBD
{
    public class AplicacionBDContexto : DbContext, IAplicacionBDContexto
    {
        private IDbContextTransaction _transaction;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build()
                .GetSection("ConnectionStrings")["LocalDbConnection"];

            optionsBuilder.UseSqlServer(connection);
        }

        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<LineaOrdenMedica> LineaOrdenMedicas { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<OrdenMedica> OrdenMedicas { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Especialidad>().ToTable("Especialidades");
            modelBuilder.Entity<Especialidad>().HasKey(u => u.EspecialidadId);
            modelBuilder.Entity<Especialidad>().Property(u => u.Nombre).IsRequired();

            modelBuilder.Entity<LineaOrdenMedica>().ToTable("LineaOrdenMedicas");
            modelBuilder.Entity<LineaOrdenMedica>().HasKey(u => u.LineaOrdenMedicaId);
            modelBuilder.Entity<LineaOrdenMedica>().Property(u => u.Cantidad).IsRequired();
            modelBuilder.Entity<LineaOrdenMedica>().Property(u => u.UnicaAplicacion).IsRequired();

            modelBuilder.Entity<Medico>().ToTable("Medicos");
            modelBuilder.Entity<Medico>().HasKey(u => u.MedicoId);
            modelBuilder.Entity<Medico>().Property(u => u.Apellido).IsRequired();
            modelBuilder.Entity<Medico>().Property(u => u.Nombre).IsRequired();
            modelBuilder.Entity<Medico>().Property(u => u.Matricula).IsRequired();

            modelBuilder.Entity<OrdenMedica>().ToTable("OrdenMedicas");
            modelBuilder.Entity<OrdenMedica>().HasKey(u => u.OrdenMedicaId);
            modelBuilder.Entity<OrdenMedica>().Property(u => u.Fecha).IsRequired();
            modelBuilder.Entity<OrdenMedica>().Property(u => u.EntregadaAlPaciente).IsRequired();

            modelBuilder.Entity<Paciente>().ToTable("Pacientes");
            modelBuilder.Entity<Paciente>().HasKey(u => u.PacienteId);
            modelBuilder.Entity<Paciente>().Property(u => u.Documento).IsRequired();
            modelBuilder.Entity<Paciente>().Property(u => u.Apellido).IsRequired();
            modelBuilder.Entity<Paciente>().Property(u => u.Nombre).IsRequired();
            modelBuilder.Entity<Paciente>().Property(u => u.FechaNacimiento).IsRequired();

            modelBuilder.Entity<Turno>().ToTable("Turnos");
            modelBuilder.Entity<Turno>().HasKey(u => u.TurnoId);
            modelBuilder.Entity<Turno>().Property(u => u.FechaTurno).IsRequired();

            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Usuario>().HasKey(u => u.UsuarioId);
            modelBuilder.Entity<Usuario>().Property(u => u.Nombre).IsRequired();
            modelBuilder.Entity<Usuario>().Property(u => u.Contrasena).IsRequired();
            modelBuilder.Entity<Usuario>().Property(u => u.Activo).IsRequired();

            modelBuilder.Entity<Medico>()
               .HasOne(pr => pr.Especialidad)
               .WithMany()
               .HasForeignKey(si => si.EspecialidadId)
               .IsRequired();

            modelBuilder.Entity<OrdenMedica>()
                .HasOne(pr => pr.Paciente)
                .WithMany()
                .HasForeignKey(si => si.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdenMedica>()
                .HasOne(pr => pr.Medico)
                .WithMany()
                .HasForeignKey(si => si.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdenMedica>()
                .HasMany(s => s.LineaOrdenMedica)
                .WithOne(si => si.OrdenMedica)
                .HasForeignKey(si => si.OrdenMedicaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Turno>()
                .HasOne(pr => pr.Paciente)
                .WithMany()
                .HasForeignKey(si => si.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Turno>()
                .HasOne(pr => pr.Medico)
                .WithMany()
                .HasForeignKey(si => si.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
        }

        public void RollbackTransaction()
        {
            _transaction.Rollback();
        }
    }
}
