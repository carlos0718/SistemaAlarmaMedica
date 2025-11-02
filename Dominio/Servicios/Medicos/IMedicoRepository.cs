using Dominio.Core.Genericos;
using Dominio.Entidades;

namespace Dominio.Servicios.Medicos
{
    public interface IMedicoRepository : IRepository<Medico>
    {
        Task<List<Especialidad>> ObtenerEspecialidadesAsync();
    }
}
