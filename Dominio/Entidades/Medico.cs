namespace Dominio.Entidades
{
    public class Medico
    {
        public int MedicoId { get; set; }
        public string Apellido { get; set;}
        public string Nombre { get; set; }
        public string Matricula { get; set;}

        public int EspecialidadId { get; set; }
        public Especialidad Especialidad { get; set; }
    }
}
