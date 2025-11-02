using Newtonsoft.Json;

namespace Dominio.Entidades
{
    public class Farmaco
    {
        public int FarmacoId { get; set; }
        public string NRegistro { get; set; }
        public string Nombre { get; set; }
        public string LabTitular { get; set; }
    }
}
