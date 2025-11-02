using Dominio.Application.Mappings;
using Dominio.Core.Genericos;
using Dominio.Servicios.Farmacos;
using Dominio.Servicios.Medicos;
using Dominio.Servicios.OrdenesMedicas;
using Dominio.Servicios.Pacientes;
using Dominio.Servicios.Turnos;
using Dominio.Servicios.Usuarios;
using Dominio.Servicios.Utils;
using Infraestructura.ContextoBD;
using Infraestructura.Genericos;
using Infraestructura.Repositorios;
using Microsoft.Extensions.DependencyInjection;

namespace Aplicacion
{
    public static class AddIoC
    {
        public static IServiceCollection AddInversionOfControl(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));

            services.AddScoped<IAplicacionBDContexto, AplicacionBDContexto>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddTransient<IFarmacosService, FarmacosService>();

            services.AddTransient<IMedicoService, MedicoService>();
            services.AddTransient<IMedicoRepository, MedicoRepository>();

            services.AddTransient<IPacienteService, PacienteService>();
            services.AddTransient<IPacienteRepository, PacienteRepository>();

            services.AddTransient<IUsuarioService, UsuarioService>();
            services.AddTransient<IUsuarioRepository, UsuarioRepository>();

            services.AddTransient<IOrdenMedicaService, OrdenMedicaService>();
            services.AddTransient<IOrdenMedicaRepository, OrdenMedicaRepository>();

            services.AddTransient<ILineaOrdenMedicaRepository, LineaOrdenMedicaRepository>();

            services.AddTransient<ITurnoService, TurnoService>();
            services.AddTransient<ITurnoRepository, TurnoRepository>();

            services.AddHttpClient<ICimaHttpClient, CimaHttpClient>(client =>
            {
                client.BaseAddress = new Uri("https://cima.aemps.es/cima/rest/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            return services;
        }
    }
}
