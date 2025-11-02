using Newtonsoft.Json;
using Presentacion.Core.Responses;

namespace Presentacion.Tools.Serializations
{
    public static class Serialization
    {
        public static string SerializeResponse(ServiceResponse response)
        {
            return JsonConvert.SerializeObject(response);
        }

        public static ServiceResponse? DeserializeResponse(string? textResponse)
        {
            ServiceResponse? response = null;

            if (!string.IsNullOrEmpty(textResponse))
            {
                response = JsonConvert.DeserializeObject<ServiceResponse>(textResponse);
            }

            return response;
        }
    }
}
