using AutoMapper;
using Dominio.Application.DTOs;
using Dominio.Entidades;

namespace Dominio.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Medico, MedicoDto>().ReverseMap();
            CreateMap<Especialidad, EspecialidadDto>().ReverseMap();
            CreateMap<Paciente, PacienteDto>().ReverseMap();
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<OrdenMedica, OrdenMedicaDto>().ReverseMap();
            CreateMap<LineaOrdenMedica, LineaOrdenMedicaDto>().ReverseMap();
            CreateMap<Turno, TurnoDto>().ReverseMap();
        }
    }
}
