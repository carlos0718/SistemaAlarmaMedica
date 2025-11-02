using Presentacion.Core;
using Presentacion.Core.DTOs;
using Presentacion.Core.Responses;

namespace Presentacion.Models
{
    public class PacienteViewModel
    {
        public List<PacienteDto> Pacientes { get; set; }
        public ServiceResponse? RespuestaServidor { get; set; }
        public TipoUsuarioDto TipoUsuario { get; set; }
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
