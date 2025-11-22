using Presentacion.Core;
using Presentacion.Core.DTOs;
using Presentacion.Core.Responses;

namespace Presentacion.Models
{
    public class PacienteViewModel
    {
        public List<PacienteDto> Pacientes { get; set; }
        public List<PacienteConTurnoDto>? PacientesConTurnos { get; set; } // Para médicos: pacientes con fecha de turno
        public ServiceResponse? RespuestaServidor { get; set; }
        public TipoUsuarioDto TipoUsuario { get; set; }
    }

    // DTO temporal para mostrar paciente + fecha de turno sin modificar la BD
    public class PacienteConTurnoDto
    {
        public PacienteDto Paciente { get; set; }
        public DateTime? FechaTurno { get; set; } // Del join con Turno
        public int? TurnoId { get; set; } // Id del turno para trazabilidad
    }

    public class GestionarPacienteViewModel
    {
        public TipoOperacion TipoOperacion { get; set; }
        public PacienteDto Paciente { get; set; }
        public ServiceResponse RespuestaServidor { get; set; }
        public TipoUsuarioDto TipoUsuario { get; set; }

        public GestionarPacienteViewModel()
        {
            RespuestaServidor = new ServiceResponse();
        }
    }
}
