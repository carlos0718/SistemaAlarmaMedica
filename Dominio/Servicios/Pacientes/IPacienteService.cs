using Dominio.Application.DTOs;
using Dominio.Entidades;
using Dominio.Shared;

namespace Dominio.Servicios.Pacientes
{
    public interface IPacienteService
    {
        Task<PacienteDto> ObtenerPorIdAsync(int id);
        Task<List<PacienteDto>> ObtenerTodosAsync(string? filtro);
        Task<ServiceResponse> AgregarAsync(PacienteDto entity);
        Task<ServiceResponse> ModificarAsync(PacienteDto entity);
        Task<ServiceResponse> EliminarAsync(int id);
        Task<Paciente> ExisteDocumentoAsync(int dni);
    }
}
