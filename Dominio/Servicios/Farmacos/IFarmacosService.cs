using Dominio.Application.DTOs;
using Dominio.Entidades;

namespace Dominio.Servicios.Farmacos
{
    public interface IFarmacosService
    {
        Task<FarmacoDto> BuscarPorNumeroRegistro(string numeroRegitro);
        Task<List<FarmacoDto>> BuscarMedicamentosPorNombre(string nombre);
        Task<List<FarmacoDto>> ObtenerMedicamentosPorPagina(int numeroPagina);
    }
}
