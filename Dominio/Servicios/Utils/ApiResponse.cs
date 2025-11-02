namespace Dominio.Servicios.Utils
{
    public class ApiResponse<T>
    {
        public int TotalFilas { get; set; }
        public int Pagina { get; set; }
        public int TamanioPagina { get; set; }
        public T Resultados { get; set; }
    }
}
