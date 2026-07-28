using System.Text;
using Newtonsoft.Json;

namespace Core.Networking
{
    public class ApiErrorPayload
    {
        [JsonProperty("statusCode")] public int StatusCode;
        [JsonProperty("code")] public string Code;
        [JsonProperty("error")] public string Error;
        [JsonProperty("message")] public string Message;

        public static ApiErrorPayload TryParse(byte[] data)
        {
            if (data is not { Length: > 0 }) return null;
            try
            {
                var json = Encoding.UTF8.GetString(data);
                return JsonConvert.DeserializeObject<ApiErrorPayload>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}