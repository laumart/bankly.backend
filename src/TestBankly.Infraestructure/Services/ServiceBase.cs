using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestBankly.Infraestructure.Services
{
    public abstract class ServiceBase
    {
        protected StringContent GetContent(object content)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };

            return new StringContent(JsonSerializer.Serialize(content, options), Encoding.UTF8, "application/json");
        }

        protected async Task<T> DesserializerObject<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                };
                return JsonSerializer.Deserialize<T>(content, options);
            }
            else
            {
                throw new HttpRequestException(content);
                //Exception? inner, HttpStatusCode? statusCode
            }
        }
    }
}
