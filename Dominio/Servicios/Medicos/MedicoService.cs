using AutoMapper;
using Dominio.Application.DTOs;
using Dominio.Entidades;
using Dominio.Servicios.OrdenesMedicas;
using Dominio.Shared;
using Microsoft.EntityFrameworkCore;

namespace Dominio.Servicios.Medicos
{
    public class MedicoService : IMedicoService
    {
        private readonly IMapper _mapper;
        private readonly IMedicoRepository _medicoRepository;
        private readonly IOrdenMedicaRepository _ordenMedicaRepository;
        public MedicoService(IMapper mapper, IMedicoRepository medicoRepository, IOrdenMedicaRepository ordenMedicaRepository)
        {
            _mapper = mapper;
            _medicoRepository = medicoRepository;
            _ordenMedicaRepository = ordenMedicaRepository;
        }

        public async Task<MedicoDto> ObtenerPorIdAsync(int id)
        {
            var medicoDb = await _medicoRepository.GetByIdWithIncludesAsync(id, "MedicoId", query => query.Include(m => m.Especialidad)
            );

            return _mapper.Map<MedicoDto>(medicoDb);
        }

        public async Task<List<MedicoDto>> ObtenerTodosAsync(string? filtro)
        {
            var medicosDb = await _medicoRepository.GetAllAsyncWithIncludesAndThen(
                    query => query
                        .Include(s => s.Especialidad)
                );

            if (!string.IsNullOrEmpty(filtro))
                medicosDb = medicosDb.Where(m =>
                    m.Nombre.ToLower().Contains(filtro) || m.Apellido.ToLower().Contains(filtro)
                    || m.Especialidad.Nombre.ToLower().Contains(filtro))
                .ToList();

            return _mapper.Map<List<MedicoDto>>(medicosDb);
        }

        public async Task<List<MedicoDto>> ObtenerMedicosPorEspecialidadAsync(int especialidadId)
        {
            var medicosDb = await _medicoRepository.GetAllAsyncWithIncludesAndThen(
                    query => query
                        .Include(s => s.Especialidad)
                        .Where(m => m.EspecialidadId == especialidadId)
                );

            return _mapper.Map<List<MedicoDto>>(medicosDb);
        }

        public async Task<ServiceResponse> AgregarAsync(MedicoDto entity)
        {
            var response = new ServiceResponse();
            try
            {
                var validarMatricula = (await _medicoRepository.GetAllAsync()).Any(m => m.Matricula == entity.Matricula);
                if (validarMatricula)
                    throw new InvalidOperationException($"El numero de matricula {entity.Matricula} ya existe en la base de datos");

                var medico = _mapper.Map<Medico>(entity);
                await _medicoRepository.AddAsync(medico);
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }
            return response;
        }

        public async Task<ServiceResponse> ModificarAsync(MedicoDto entity)
        {
            var response = new ServiceResponse();

            try
            {
                var medico = _mapper.Map<Medico>(entity);
                await _medicoRepository.UpdateAsync(medico);
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
                if ((await _medicoRepository.GetByIdAsync(id)) == null)
                    throw new InvalidOperationException($"El Id del Medico no exite en la base de datos");

                if (await _ordenMedicaRepository.ExisteMedicoEnOrdenesMedicasAsync(id))
                    throw new InvalidOperationException($"No se puede realizar la eliminación. El Médico esta relacionado a una Orden Médica.");

                var medicoDb = await _medicoRepository.GetByIdAsync(id);
                await _medicoRepository.DeleteAsync(medicoDb);
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }
            return response;
        }

        public async Task<List<EspecialidadDto>> ObtenerEspecialidadesAsync()
        {
            var especialidadesDb = await _medicoRepository.ObtenerEspecialidadesAsync();

            return _mapper.Map<List<EspecialidadDto>>(especialidadesDb);
        }
    }
}
