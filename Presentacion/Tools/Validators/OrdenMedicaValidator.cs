using FluentValidation;
using Presentacion.Models;

namespace Presentacion.Tools.Validators
{
    public class OrdenMedicaValidator : AbstractValidator<GestionarOrdenMedicaViewModel>
    {
        public OrdenMedicaValidator()
        {
            RuleFor(usu => usu.OrdenMedica.PacienteId)
                .NotNull().WithMessage("El campo Paciente no debe ser nulo.")
                .NotEmpty().WithMessage("El campo Paciente es requerido.");

            RuleFor(usu => usu.OrdenMedica.MedicoId)
                .NotNull().WithMessage("El campo Medico no debe ser nulo.")
                .NotEmpty().WithMessage("El campo Medico es requerido.");

            RuleFor(usu => usu.OrdenMedica.ObraSocial)
                .NotNull().WithMessage("El campo Obra Social no debe ser nulo.")
                .NotEmpty().WithMessage("El campo Obra Social es requerido.");

            RuleFor(usu => usu.OrdenMedica.LineaOrdenMedica)
                .NotNull().WithMessage("El campo Farmaco no debe ser nulo.")
                .NotEmpty().WithMessage("Debe haber al menos un Farmaco para recetar.");
        }
    }
}
