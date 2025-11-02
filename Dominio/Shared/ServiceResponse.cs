namespace Dominio.Shared
{
    public class ServiceResponse<T>
    {
        public int ReturnValue { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnName { get; set; }

        public List<ServiceError> Errors { get; set; }
        public T Data { get; set; }

        public string MessageTitle { get; set; }
        public string MessageText { get; set; }

        public bool Status => !Errors.Any();

        public bool IsSuccess => !IsFailure;

        public bool IsFailure => Errors.Any();

        public ServiceResponse()
        {
            Errors = new List<ServiceError>();
        }

        public void AddError(Exception ex)
        {
            Errors.Add(new ServiceError(ex));
        }

        public void AddError(string errorMessage)
        {
            Errors.Add(new ServiceError(errorMessage));
        }

        public void AddError(string errorCode, string errorMessage)
        {
            Errors.Add(new ServiceError(errorCode, errorMessage));
        }

        public void AddError(string errorCode, string errorMessage, ServiceErrorLevel errorLevel)
        {
            Errors.Add(new ServiceError(errorCode, errorMessage, errorLevel));
        }

        public void AddError(ServiceError serviceError)
        {
            Errors.Add(serviceError);
        }

        public void AddErrors(List<ServiceError> serviceErrorList)
        {
            foreach (var e in serviceErrorList)
                Errors.Add(e);
        }

        public void AddErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors)
                Errors.Add(new ServiceError(error));
        }

        public string GetErrorsAsString() => string.Join("; ", Errors.Select(e => e.ToString()));
    }

    public class ServiceResponse : ServiceResponse<object>
    {
        #region Service Response Factory Methods

        // Success
        public static ServiceResponse Success()
            => new ServiceResponse();

        public static ServiceResponse<T> Success<T>(T data, int value = 0)
            => new ServiceResponse<T> { Data = data, ReturnValue = value };

        // Failure
        public static ServiceResponse Failure(List<ServiceError> errors)
            => new ServiceResponse { Errors = errors, };

        #endregion Service Response Factory Methods
    }
}
