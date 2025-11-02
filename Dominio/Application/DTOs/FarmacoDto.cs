using Newtonsoft.Json;

namespace Dominio.Application.DTOs
{
    public class FarmacoDto
    {
        public int? FarmacoId { get; set; }

        [JsonProperty("nregistro")]
        public string? NRegistro { get; set; }

        [JsonProperty("nombre")]
        public string? Nombre { get; set; }

        [JsonProperty("labtitular")]
        public string? LabTitular { get; set; }
    }
}
