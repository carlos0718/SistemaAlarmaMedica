namespace Presentacion.Core.Responses
{
    public enum ServiceErrorLevel
    {
        VALIDATION_ERROR,
        VALIDATION_WARNING,
        EXCEPTION
    }

    public class ServiceError
    {
        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorDetail { get; set; }

        public ServiceErrorLevel ErrorLevel { get; set; }

        public ServiceError()
        {
        }

        public ServiceError(Exception ex)
        {
            var finalException = ex;

            if (ex.InnerException != null)
                finalException = ex.InnerException;

            ErrorMessage = finalException.Message;
            ErrorDetail = finalException.ToString();
            ErrorLevel = ServiceErrorLevel.EXCEPTION;
        }

        public ServiceError(string errorMessage)
        {
            ErrorMessage = errorMessage;
            ErrorLevel = ServiceErrorLevel.VALIDATION_ERROR;
        }

        public ServiceError(string errorCode, string errorMessage)
        {
            ErrorMessage = errorMessage;
            ErrorLevel = ServiceErrorLevel.VALIDATION_ERROR;
            ErrorCode = errorCode;
        }

        public ServiceError(string errorCode, string errorMessage, ServiceErrorLevel errorLevel)
        {
            ErrorMessage = errorMessage;
            ErrorLevel = errorLevel;
            ErrorCode = errorCode;
        }
    }
}
