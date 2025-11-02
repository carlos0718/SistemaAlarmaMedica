using AutoMapper;
using Dominio.Application.DTOs;
using Dominio.Entidades;
using Dominio.Shared;
using Microsoft.EntityFrameworkCore;

namespace Dominio.Servicios.OrdenesMedicas
{
    public class OrdenMedicaService : IOrdenMedicaService
    {
        private readonly IMapper _mapper;
        private readonly IOrdenMedicaRepository _ordenMedicaRepository;

        public OrdenMedicaService(IMapper mapper, IOrdenMedicaRepository ordenMedicaRepository)
        {
            _mapper = mapper;
            _ordenMedicaRepository = ordenMedicaRepository;
        }

        public async Task<OrdenMedicaDto> ObtenerPorIdAsync(int id)
        {
            var ordenDb = await _ordenMedicaRepository.GetByIdWithIncludesAsync(
                id,
                "OrdenMedicaId",
                query => query
                    .Include(o => o.Paciente)
                    .Include(o => o.Medico)
                        .ThenInclude(m => m.Especialidad)
                    .Include(o => o.LineaOrdenMedica)
            );
            return _mapper.Map<OrdenMedicaDto>(ordenDb);
        }

        public async Task<List<OrdenMedicaDto>> ObtenerTodosAsync(string? filtro)
        {
            var ordenesDb = await _ordenMedicaRepository.GetAllAsyncWithIncludesAndThen(
                    query => query
                        .Include(o => o.Paciente)
                        .Include(o => o.Medico)
                            .ThenInclude(m => m.Especialidad)
                        .Include(o => o.LineaOrdenMedica)
            );

            if (!string.IsNullOrEmpty(filtro))
                ordenesDb = ordenesDb.Where(o =>
                    o.OrdenMedicaId.ToString().Contains(filtro)
                    || o.Paciente.Nombre.ToLower().Contains(filtro) || o.Paciente.Apellido.ToLower().Contains(filtro)
                    || o.Medico.Apellido.ToLower().Contains(filtro) || o.Medico.Nombre.ToLower().Contains(filtro))
                .ToList();

            return _mapper.Map<List<OrdenMedicaDto>>(ordenesDb);
        }

        public async Task<ServiceResponse> AgregarAsync(OrdenMedicaDto entity)
        {
            var response = new ServiceResponse();
            try
            {
                entity.Fecha = DateTime.Now;

                var orden = _mapper.Map<OrdenMedica>(entity);
                await _ordenMedicaRepository.AddAsync(orden);
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }
            return response;
        }

        public async Task<ServiceResponse> ModificarAsync(OrdenMedicaDto entity)
        {
            var response = new ServiceResponse();

            try
            {
                var ordenOriginal = await _ordenMedicaRepository.GetByIdWithIncludesAsync(
                    entity.OrdenMedicaId.Value,
                    "OrdenMedicaId",
                    query => query
                        .Include(o => o.LineaOrdenMedica)
                );

                _mapper.Map(entity, ordenOriginal);

                await _ordenMedicaRepository.UpdateAsync(ordenOriginal);

            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }
            return response;
        }

        public async Task<ServiceResponse> EliminarAsync(int id)
        {
            var response = new ServiceResponse();

            try
            {
                var ordenDb = await _ordenMedicaRepository.GetByIdAsync(id);
                if (ordenDb == null)
                    throw new InvalidOperationException($"El Id de la Orden Médica no exite en la base de datos");

                if (ordenDb.EntregadaAlPaciente)
                    throw new InvalidOperationException($"La Orden Médica ya fue tomada por Paciente. No se puede eliminar");

                await _ordenMedicaRepository.DeleteAsync(ordenDb);
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }
            return response;
        }

        public async Task<ServiceResponse> TomarOrdenMedica(int ordenMedicaId)
        {
            var response = new ServiceResponse();

            try
            {
                var ordenOriginal = await _ordenMedicaRepository.GetByIdAsync(ordenMedicaId);
                ordenOriginal.EntregadaAlPaciente = true;
                await _ordenMedicaRepository.UpdateAsync(ordenOriginal);
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }
            return response;
        }

        public async Task<List<OrdenMedicaDto>> ObtenerPorDniAsync(int dni)
        {
            var ordenesDb = await _ordenMedicaRepository.GetAllAsyncWithIncludesAndThen(
                    query => query
                        .Include(o => o.Paciente)
                        .Include(o => o.Medico)
                            .ThenInclude(m => m.Especialidad)
                        .Include(o => o.LineaOrdenMedica)
                        .Where(o => o.Paciente.Documento == dni)
            );

            return _mapper.Map<List<OrdenMedicaDto>>(ordenesDb);
        }

        public async Task<ServiceResponse> EmpezarTratamientoLineaOrdenMedicaAsync(int lineaOrdenMedicaId)
        {
            var response = new ServiceResponse();

            try
            {
                var resultado = await _ordenMedicaRepository.ActualizarLineaOrdenMedicaAsync(lineaOrdenMedicaId);
                if (!resultado)
                {
                    response.AddError($"La línea de orden médica con ID {lineaOrdenMedicaId} no existe en la base de datos");
                }
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }
            return response;
        }
    }
}
