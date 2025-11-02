using AutoMapper;
using Dominio.Application.DTOs;
using Dominio.Entidades;
using Dominio.Shared;

namespace Dominio.Servicios.Turnos
{
    public class TurnoService : ITurnoService
    {
        private readonly IMapper _mapper;
        private readonly ITurnoRepository _turnoRepository;

        public TurnoService(IMapper mapper, ITurnoRepository turnoRepository)
        {
            _mapper = mapper;
            _turnoRepository = turnoRepository;
        }

        public async Task<TurnoDto> ObtenerPorIdAsync(int id)
        {
            var turnoDb = await _turnoRepository.GetByIdAsync(id);
            return _mapper.Map<TurnoDto>(turnoDb);
        }

        public async Task<List<TurnoDto>> ObtenerTodosAsync()
        {
            var turnosDb = await _turnoRepository.GetAllAsync();
            return _mapper.Map<List<TurnoDto>>(turnosDb);
        }

        public async Task<List<TurnoDto>> ObtenerTurnosPorPacienteAsync(int pacienteId)
        {
            var turnosDb = await _turnoRepository.ObtenerTurnosPorPacienteAsync(pacienteId);
            return _mapper.Map<List<TurnoDto>>(turnosDb);
        }

        public async Task<List<TurnoDto>> ObtenerTurnosPorMedicoAsync(int medicoId)
        {
            var turnosDb = await _turnoRepository.ObtenerTurnosPorMedicoAsync(medicoId);
            return _mapper.Map<List<TurnoDto>>(turnosDb);
        }

        public async Task<ServiceResponse> AgregarAsync(TurnoDto entity)
        {
            var response = new ServiceResponse();
            try
            {
                // Validaciones adicionales
                if (!entity.PacienteId.HasValue || entity.PacienteId.Value <= 0)
                    throw new InvalidOperationException("Debe seleccionar un paciente válido.");

                if (!entity.MedicoId.HasValue || entity.MedicoId.Value <= 0)
                    throw new InvalidOperationException("Debe seleccionar un médico válido.");

                if (!entity.FechaTurno.HasValue)
                    throw new InvalidOperationException("Debe seleccionar una fecha para el turno.");

                if (entity.FechaTurno.Value <= DateTime.Now)
                    throw new InvalidOperationException("La fecha del turno debe ser mayor a la fecha actual.");

                // Asignar estado PENDIENTE por defecto si no se especifica
                if (!entity.Estado.HasValue)
                {
                    entity.Estado = EstadoTurno.PENDIENTE;
                }

                var turno = _mapper.Map<Turno>(entity);
                await _turnoRepository.AddAsync(turno);
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }
            return response;
        }

        public async Task<ServiceResponse> ModificarAsync(TurnoDto entity)
        {
            var response = new ServiceResponse();

            try
            {
                var turnoDb = await _turnoRepository.GetByIdAsync(entity.TurnoId.Value);
                if (turnoDb == null)
                    throw new InvalidOperationException($"No se encontró el turno con ID {entity.TurnoId}");

                if (entity.FechaTurno <= DateTime.Now)
                    throw new InvalidOperationException("La fecha del turno debe ser mayor a la fecha actual.");

                _mapper.Map(entity, turnoDb);
                await _turnoRepository.UpdateAsync(turnoDb);
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
                var turnoDb = await _turnoRepository.GetByIdAsync(id);
                if (turnoDb == null)
                    throw new InvalidOperationException($"El ID del Turno no existe en la base de datos");

                await _turnoRepository.DeleteAsync(turnoDb);
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }
            return response;
        }
    }
}
