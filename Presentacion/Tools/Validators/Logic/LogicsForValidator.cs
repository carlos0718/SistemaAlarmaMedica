using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Presentacion.Tools.Validators.Logic
{
    public static class LogicsForValidator
    {
        public static ModelStateDictionary GetAllErrorsInView(ModelStateDictionary ModelState, ValidationResult result)
        {
            foreach (var failure in result.Errors)
            {
                ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
            }

            return ModelState;
        }
    }
}
