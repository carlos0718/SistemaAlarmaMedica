using Dominio.Application.DTOs;
using Dominio.Shared;

namespace Dominio.Servicios.OrdenesMedicas
{
    public interface IOrdenMedicaService
    {
        Task<OrdenMedicaDto> ObtenerPorIdAsync(int id);
        Task<List<OrdenMedicaDto>> ObtenerTodosAsync(string? nombre);
        Task<ServiceResponse> AgregarAsync(OrdenMedicaDto entity);
        Task<ServiceResponse> ModificarAsync(OrdenMedicaDto entity);
        Task<ServiceResponse> EliminarAsync(int id);
        Task<ServiceResponse> TomarOrdenMedica(int ordenMedicaId);

        Task<List<OrdenMedicaDto>> ObtenerPorDniAsync(int dni);
        Task<ServiceResponse> EmpezarTratamientoLineaOrdenMedicaAsync(int lineaOrdenMedicaId);
    }
}
