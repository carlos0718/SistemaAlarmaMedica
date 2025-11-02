using Presentacion.Core;
using Presentacion.Core.DTOs;
using Presentacion.Core.Responses;

namespace Presentacion.Models
{
    public class UsuarioViewModel
    {
        public List<UsuarioDto> Usuarios { get; set; }
        public ServiceResponse? RespuestaServidor { get; set; }
    }

    public class GestionarUsuarioViewModel
    {
        public TipoOperacion TipoOperacion { get; set; }
        public UsuarioDto Usuario { get; set; }
        public List<string> TiposUsuario { get; set; }

        public ServiceResponse RespuestaServidor { get; set; }

        public GestionarUsuarioViewModel()
        {
            RespuestaServidor = new ServiceResponse();
        }
    }
}
