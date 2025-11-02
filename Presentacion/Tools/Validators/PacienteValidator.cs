using FluentValidation;
using Presentacion.Models;

namespace Presentacion.Tools.Validators
{
    public class PacienteValidator : AbstractValidator<GestionarPacienteViewModel>
    {
        public PacienteValidator()
        {
            RuleFor(pac => pac.Paciente.Documento)
                .NotEmpty().WithMessage("El campo Documento es requerido.");

            RuleFor(pac => pac.Paciente.Apellido)
                .NotNull().WithMessage("El campo no debe ser nulo.")
                .NotEmpty().WithMessage("El campo Apellido es requerido.")
                .Matches("^[a-zA-Z\\s/]*$").WithMessage("El campo Nombre solo debe contener letras y espacios.");

            RuleFor(pac => pac.Paciente.FechaNacimiento)
                .NotEmpty().WithMessage("El campo Fecha de Nacimiento es requerido.")
                .Must(fecha => fecha <= DateTime.Today).WithMessage("La Fecha de Nacimiento no puede ser superior a la actual.")
                .Must(fecha => fecha >= DateTime.Today.AddYears(-120)).WithMessage("La Fecha de Nacimiento no puede ser mayor a 110 años en el pasado.");
        }
    }
}
