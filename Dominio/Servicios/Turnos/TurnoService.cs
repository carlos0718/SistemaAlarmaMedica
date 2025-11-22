using AutoMapper;
using Dominio.Application.DTOs;
using Dominio.Entidades;
using Dominio.Servicios.OrdenesMedicas;
using Dominio.Shared;

namespace Dominio.Servicios.Turnos
{
    public class TurnoService : ITurnoService
    {
        private readonly IMapper _mapper;
        private readonly ITurnoRepository _turnoRepository;
        private readonly IOrdenMedicaRepository _ordenMedicaRepository;

        public TurnoService(IMapper mapper, ITurnoRepository turnoRepository, IOrdenMedicaRepository ordenMedicaRepository)
        {
            _mapper = mapper;
            _turnoRepository = turnoRepository;
            _ordenMedicaRepository = ordenMedicaRepository;
        }

        /// <summary>
        /// Detecta si un turno fue atendido (si existe una orden médica para ese paciente-médico)
        /// </summary>
        private async Task<bool> FueAtendidoAsync(int pacienteId, int medicoId)
        {
            return await _ordenMedicaRepository.ExisteOrdenMedicaAsync(pacienteId, medicoId);
        }

        /// <summary>
        /// Llena la propiedad FueAtendido de los turnos
        /// </summary>
        private async Task<List<TurnoDto>> LlenarFueAtendidoAsync(List<TurnoDto> turnos)
        {
            foreach (var turno in turnos)
            {
                if (turno.PacienteId.HasValue && turno.MedicoId.HasValue)
                {
                    turno.FueAtendido = await FueAtendidoAsync(turno.PacienteId.Value, turno.MedicoId.Value);
                }
            }
            return turnos;
        }

        public async Task<TurnoDto> ObtenerPorIdAsync(int id)
        {
            var turnoDb = await _turnoRepository.GetByIdAsync(id);
            var turnoDto = _mapper.Map<TurnoDto>(turnoDb);

            if (turnoDto.PacienteId.HasValue && turnoDto.MedicoId.HasValue)
            {
                turnoDto.FueAtendido = await FueAtendidoAsync(turnoDto.PacienteId.Value, turnoDto.MedicoId.Value);
            }

            return turnoDto;
        }

        public async Task<List<TurnoDto>> ObtenerTodosAsync()
        {
            var turnosDb = await _turnoRepository.ObtenerTodosConIncludesAsync();
            var turnosDto = _mapper.Map<List<TurnoDto>>(turnosDb);
            return await LlenarFueAtendidoAsync(turnosDto);
        }

        public async Task<List<TurnoDto>> ObtenerTurnosPorPacienteAsync(int pacienteId)
        {
            var turnosDb = await _turnoRepository.ObtenerTurnosPorPacienteAsync(pacienteId);
            var turnosDto = _mapper.Map<List<TurnoDto>>(turnosDb);
            return await LlenarFueAtendidoAsync(turnosDto);
        }

        public async Task<List<TurnoDto>> ObtenerTurnosPorMedicoAsync(int medicoId)
        {
            var turnosDb = await _turnoRepository.ObtenerTurnosPorMedicoAsync(medicoId);
            var turnosDto = _mapper.Map<List<TurnoDto>>(turnosDb);
            return await LlenarFueAtendidoAsync(turnosDto);
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
