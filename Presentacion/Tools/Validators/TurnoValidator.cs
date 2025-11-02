using FluentValidation;
using Presentacion.Models;

namespace Presentacion.Tools.Validators
{
    public class TurnoValidator : AbstractValidator<GestionarTurnoViewModel>
    {
        public TurnoValidator()
        {
            // Solo validar los campos del Turno, no las listas de apoyo
            RuleFor(model => model.Turno)
                .NotNull().WithMessage("El turno es requerido.");

            RuleFor(model => model.Turno.PacienteId)
                .NotEmpty().WithMessage("El campo Paciente es requerido.")
                .GreaterThan(0).WithMessage("Debe seleccionar un paciente válido.");

            RuleFor(model => model.Turno.MedicoId)
                .NotEmpty().WithMessage("El campo Médico es requerido.")
                .GreaterThan(0).WithMessage("Debe seleccionar un médico válido.");

            RuleFor(model => model.Turno.FechaTurno)
                .NotEmpty().WithMessage("El campo Fecha del Turno es requerido.")
                .Must(fecha => fecha.HasValue && fecha.Value > DateTime.Now)
                .WithMessage("La fecha del turno debe ser posterior a la fecha actual.");
        }
    }
}
