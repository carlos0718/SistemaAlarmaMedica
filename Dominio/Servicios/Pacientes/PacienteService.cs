using AutoMapper;
using Dominio.Application.DTOs;
using Dominio.Entidades;
using Dominio.Servicios.OrdenesMedicas;
using Dominio.Shared;

namespace Dominio.Servicios.Pacientes
{
    public class PacienteService : IPacienteService
    {
        private readonly IMapper _mapper;
        private readonly IPacienteRepository _pacienteRepository;
        private readonly IOrdenMedicaRepository _ordenMedicaRepository;

        public PacienteService(IMapper mapper, IPacienteRepository pacienteRepository, IOrdenMedicaRepository ordenMedicaRepository)
        {
            _mapper = mapper;
            _pacienteRepository = pacienteRepository;
            _ordenMedicaRepository = ordenMedicaRepository;
        }

        public async Task<PacienteDto> ObtenerPorIdAsync(int id)
        {
            var pacienteDb = await _pacienteRepository.GetByIdAsync(id);

            return _mapper.Map<PacienteDto>(pacienteDb);
        }

        public async Task<List<PacienteDto>> ObtenerTodosAsync(string? filtro)
        {
            var pacienteDb = await _pacienteRepository.GetAllAsync();
            
            if (!string.IsNullOrEmpty(filtro))
            {
                pacienteDb = pacienteDb.Where(p => p.Documento.ToString().Contains(filtro) || p.Apellido.ToLower().Contains(filtro) || p.Nombre.ToLower().Contains(filtro)).ToList();
            }

            return _mapper.Map<List<PacienteDto>>(pacienteDb);
        }

        public async Task<ServiceResponse> AgregarAsync(PacienteDto entity)
        {
            var response = new ServiceResponse();
            try
            {
                var validarDocumento = (await _pacienteRepository.GetAllAsync()).Any(m => m.Documento == entity.Documento);
                if (validarDocumento)
                    throw new InvalidOperationException($"Ya existe un paciente con el Documento: {entity.Documento} en la base de datos.");

                var paciente = _mapper.Map<Paciente>(entity);
                await _pacienteRepository.AddAsync(paciente);
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }
            return response;
        }

        public async Task<ServiceResponse> ModificarAsync(PacienteDto entity)
        {
            var response = new ServiceResponse();

            try
            {
                var pacienteDb = await _pacienteRepository.GetByIdAsync(entity.PacienteId.Value);
                if (pacienteDb == null)
                    throw new InvalidOperationException($"No se encontró el paciente con ID {entity.PacienteId}");

                _mapper.Map(entity, pacienteDb);
                await _pacienteRepository.UpdateAsync(pacienteDb);
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
                if ((await _pacienteRepository.GetByIdAsync(id)) == null)
                    throw new InvalidOperationException($"El Id del Paciente no exite en la base de datos");

                if (await _ordenMedicaRepository.ExistePacienteEnOrdenesMedicasAsync(id))
                    throw new InvalidOperationException($"No se puede realizar la eliminación. El Paciente esta relacionado a una Orden Médica.");

                var pacienteDb = await _pacienteRepository.GetByIdAsync(id);
                await _pacienteRepository.DeleteAsync(pacienteDb);
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }
            return response;
        }
        
        public async Task<Paciente> ExisteDocumentoAsync(int dni)
        {
            var pacienteDb = (await _pacienteRepository.GetAllAsync()).Where(p => p.Documento == dni).FirstOrDefault();
            return pacienteDb;
        }
    }
}
