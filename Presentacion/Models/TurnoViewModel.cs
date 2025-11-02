using Presentacion.Core;
using Presentacion.Core.DTOs;
using Presentacion.Core.Responses;

namespace Presentacion.Models
{
    public class TurnoViewModel
    {
        public List<TurnoDto> Turnos { get; set; }
        public ServiceResponse RespuestaServidor { get; set; }
    }

    public class GestionarTurnoViewModel
    {
        public TipoOperacion TipoOperacion { get; set; }
        public TurnoDto Turno { get; set; }
        public List<MedicoDto> Medicos { get; set; }
        public List<PacienteDto> Pacientes { get; set; }
        public List<EspecialidadDto> Especialidades { get; set; }
        public ServiceResponse RespuestaServidor { get; set; }
        public TipoUsuarioDto TipoUsuario { get; set; }
        public List<TurnoDto> TurnosPaciente { get; set; }
        public string? MensajeConfirmacion { get; set; }

        public GestionarTurnoViewModel()
        {
            RespuestaServidor = new ServiceResponse();
            TurnosPaciente = new List<TurnoDto>();
        }
    }
}
