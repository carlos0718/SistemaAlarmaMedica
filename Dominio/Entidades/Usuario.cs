namespace Dominio.Entidades
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Contrasena { get; set; }
        public TipoUsuario? TipoUsuario { get; set; }
        public bool Activo { get; set; }
        public string? GoogleId { get; set; }
        public string? Email { get; set; }

        // Relaciones con Paciente y Medico
        public int? PacienteId { get; set; }
        public Paciente? Paciente { get; set; }

        public int? MedicoId { get; set; }
        public Medico? Medico { get; set; }
    }
}
