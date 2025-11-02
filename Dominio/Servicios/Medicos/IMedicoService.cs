using Dominio.Application.DTOs;
using Dominio.Shared;

namespace Dominio.Servicios.Medicos
{
    public interface IMedicoService
    {
        Task<MedicoDto> ObtenerPorIdAsync(int id);
        Task<List<MedicoDto>> ObtenerTodosAsync(string? filtro);
        Task<List<MedicoDto>> ObtenerMedicosPorEspecialidadAsync(int especialidadId);
        Task<ServiceResponse> AgregarAsync(MedicoDto entity);
        Task<ServiceResponse> ModificarAsync(MedicoDto entity);
        Task<ServiceResponse> EliminarAsync(int id);
        Task<List<EspecialidadDto>> ObtenerEspecialidadesAsync();
    }
}
