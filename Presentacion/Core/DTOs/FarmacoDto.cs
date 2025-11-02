using Newtonsoft.Json;

namespace Presentacion.Core.DTOs
{
    public class FarmacoDto
    {
        public int? FarmacoId { get; set; }

        public string? NRegistro { get; set; }

        public string? Nombre { get; set; }

        public string? LabTitular { get; set; }
    }
}
