using FluentValidation;
using Presentacion.Models;

namespace Presentacion.Tools.Validators
{
    public class MedicoValidator : AbstractValidator<GestionarMedicoViewModel>
    {
        public MedicoValidator()
        {
            RuleFor(pro => pro.Medico.Apellido)
                .NotNull().WithMessage("El campo no debe ser nulo.")
                .NotEmpty().WithMessage("El campo Apellido es requerido.")
                .Matches("^[a-zA-Z\\s/]*$").WithMessage("El campo Nombre solo debe contener letras y espacios.");

            RuleFor(pro => pro.Medico.Nombre)
                .NotNull().WithMessage("El campo no debe ser nulo.")
                .NotEmpty().WithMessage("El campo Nombre es requerido.")
                .Matches("^[a-zA-Z\\s/]*$").WithMessage("El campo Nombre solo debe contener letras y espacios.");

            RuleFor(pro => pro.Medico.Matricula)
                .NotNull().WithMessage("El campo no debe ser nulo.")
                .NotEmpty().WithMessage("El campo Matricula es requerido.")
                .Matches("^[a-zA-Z0-9/]*$").WithMessage("El campo C. Principal solo debe contener letras y números.");

            RuleFor(pro => pro.Medico.EspecialidadId)
                .NotNull().WithMessage("El campo no debe ser nulo.")
                .NotEmpty().WithMessage("El campo Especialidad es requerido.");
        }
    }
}
