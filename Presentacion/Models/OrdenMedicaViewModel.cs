using Presentacion.Core;
using Presentacion.Core.DTOs;
using Presentacion.Core.Responses;

namespace Presentacion.Models
{
    public class OrdenMedicaViewModel
    {
        public List<OrdenMedicaDto> OrdenesMedicas { get; set; }
        public ServiceResponse RespuestaServidor { get; set; }
    }

    public class GestionarOrdenMedicaViewModel
    {
        public TipoOperacion TipoOperacion { get; set; }
        public OrdenMedicaDto OrdenMedica { get; set; }
        public List<MedicoDto> Medicos { get; set; }
        public List<PacienteDto> Pacientes { get; set; }
        public List<string> ObrasSociales { get; set; }
        public ServiceResponse RespuestaServidor { get; set; }

        public GestionarOrdenMedicaViewModel()
        {
            RespuestaServidor = new ServiceResponse();
        }
    }
}
