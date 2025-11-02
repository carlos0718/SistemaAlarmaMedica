using Dominio.Application.DTOs;
using Dominio.Entidades;
using Dominio.Servicios.Utils;
using Newtonsoft.Json;

namespace Dominio.Servicios.Farmacos
{
    public class FarmacosService : IFarmacosService
    {
        private readonly ICimaHttpClient _cimaHttpClient;

        public FarmacosService(ICimaHttpClient cimaHttpClient)
        {
            _cimaHttpClient = cimaHttpClient;
        }

        public async Task<FarmacoDto> BuscarPorNumeroRegistro(string numeroRegistro)
        {
            var farmacos = await _cimaHttpClient.ObtenerMedicamentosPorNumeroRegistro(numeroRegistro);
            return farmacos.FirstOrDefault();
        }

        public async Task<List<FarmacoDto>> BuscarMedicamentosPorNombre(string nombre)
        {
            return await _cimaHttpClient.BuscarMedicamentosPorNombre(nombre);
        }

        public async Task<List<FarmacoDto>> ObtenerMedicamentosPorPagina(int numeroPagina)
        {
            return await _cimaHttpClient.ObtenerMedicamentosPorPagina(numeroPagina);
        }
    }
}
