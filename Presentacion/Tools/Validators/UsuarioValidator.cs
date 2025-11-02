using FluentValidation;
using Presentacion.Models;

namespace Presentacion.Tools.Validators
{
    public class UsuarioValidator : AbstractValidator<GestionarUsuarioViewModel>
    {
        public UsuarioValidator()
        {
            RuleFor(usu => usu.Usuario.Nombre)
                .NotNull().WithMessage("El campo no debe ser nulo.")
                .NotEmpty().WithMessage("El campo Nombre es requerido.");

            RuleFor(usu => usu.Usuario.Contrasena)
                .NotNull().WithMessage("El campo no debe ser nulo.")
                .NotEmpty().WithMessage("El campo Contrasena es requerido.");

            RuleFor(usu => usu.Usuario.TipoUsuario.ToString())
                .NotNull().WithMessage("El campo no debe ser nulo.")
                .NotEmpty().WithMessage("El campo Matricula es requerido.");
        }
    }
}
