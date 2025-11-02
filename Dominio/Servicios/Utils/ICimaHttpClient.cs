using Dominio.Application.DTOs;

namespace Dominio.Servicios.Utils
{
    public interface ICimaHttpClient
    {
        Task<List<FarmacoDto>> ObtenerMedicamentosPorNumeroRegistro(string numeroRegistro);
        Task<List<FarmacoDto>> ObtenerMedicamentosPorPagina(int numeroPagina);
        Task<List<FarmacoDto>> BuscarMedicamentosPorNombre(string nombre);
    }
}
