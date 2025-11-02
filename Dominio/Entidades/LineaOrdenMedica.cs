namespace Dominio.Entidades
{
    public class LineaOrdenMedica
    {
        public LineaOrdenMedica()
        {
            UnicaAplicacion = false;
            TratamientoEmpezado = false;
        }

        public int LineaOrdenMedicaId { get; set; }
        public int Cantidad { get; set; }

        public string NumeroRegistro { get; set; }
        public string Nombre { get; set; }

        public int? FrecuenciaHoras { get; set; }

        public int OrdenMedicaId { get; set; }
        public OrdenMedica OrdenMedica { get; set; }

        public bool UnicaAplicacion { get; set; }
        public string? Observacion { get; set; }

        public bool TratamientoEmpezado { get; set; }
    }
}
