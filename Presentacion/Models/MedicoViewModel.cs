using Presentacion.Core;
using Presentacion.Core.DTOs;
using Presentacion.Core.Responses;

namespace Presentacion.Models
{
    public class MedicoViewModel
    {
        public List<MedicoDto> Medicos { get; set; }
        public ServiceResponse? RespuestaServidor { get; set; }
    }

    public class GestionarMedicoViewModel
    {
        public TipoOperacion TipoOperacion { get; set; }
        public MedicoDto Medico { get; set; }
        public List<EspecialidadDto> Especialidades { get; set; }

        public ServiceResponse RespuestaServidor { get; set; }

        public GestionarMedicoViewModel()
        {
            RespuestaServidor = new ServiceResponse();
        }
    }
}
