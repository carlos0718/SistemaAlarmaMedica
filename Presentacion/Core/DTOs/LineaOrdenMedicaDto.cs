namespace Presentacion.Core.DTOs
{
    public class LineaOrdenMedicaDto
    {
        public int? LineaOrdenMedicaId { get; set; }
        public int? Cantidad { get; set; }

        public string? NumeroRegistro { get; set; }
        public string? Nombre { get; set; }

        public int? FrecuenciaHoras { get; set; }

        public int? OrdenMedicaId { get; set; }

        public OrdenMedicaDto? OrdenMedica { get; set; }
        
        public bool? UnicaAplicacion { get; set; }
        public string? Observacion { get; set; }

        public string TextoFrecuencia => !FrecuenciaHoras.HasValue ? "Única aplicación" : $"{FrecuenciaHoras} Hs";
    }
}
