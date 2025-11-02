using Presentacion.Core.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Presentacion.Models
{
    public class CompleteGoogleRegistrationViewModel
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres")]
        [Display(Name = "Nombre de usuario")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un tipo de usuario")]
        [Display(Name = "Tipo de usuario")]
        public TipoUsuarioDto TipoUsuario { get; set; }

        // Campos para Paciente
        [StringLength(50)]
        [Display(Name = "Documento")]
        public string? DocumentoPaciente { get; set; }

        [StringLength(255)]
        [Display(Name = "Apellido")]
        public string? ApellidoPaciente { get; set; }

        [StringLength(255)]
        [Display(Name = "Nombre")]
        public string? NombrePaciente { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime? FechaNacimientoPaciente { get; set; }

        // Campos para Médico
        [StringLength(50)]
        [Display(Name = "Matrícula")]
        public string? MatriculaMedico { get; set; }

        [StringLength(255)]
        [Display(Name = "Apellido")]
        public string? ApellidoMedico { get; set; }

        [StringLength(255)]
        [Display(Name = "Nombre")]
        public string? NombreMedico { get; set; }

        [Display(Name = "Especialidad")]
        public int? EspecialidadId { get; set; }

        public List<TipoUsuarioDto> TiposUsuario { get; set; } = new List<TipoUsuarioDto>();
        public List<EspecialidadDto> Especialidades { get; set; } = new List<EspecialidadDto>();
    }
}
