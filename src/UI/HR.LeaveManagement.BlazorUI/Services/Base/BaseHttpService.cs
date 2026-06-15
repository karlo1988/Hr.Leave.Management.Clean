using System.Linq;
using System.Text.Json;

namespace HR.LeaveManagement.BlazorUI.Services.Base
{
    public class BaseHttpService
    {
        protected IClient _client;
        public BaseHttpService(IClient client)
        {
            _client = client;
        }

        protected Response<Guid> ConvertApiExceptions<Guid>(ApiException exception)
        {
            if (exception.StatusCode == 400)
            {
                string validationErrors = string.Empty;
                try
                {
                    using var doc = JsonDocument.Parse(exception.Response);
                    var root = doc.RootElement;
                    // Try camelCase first, then PascalCase — WriteAsJsonAsync may produce either
                    if (!root.TryGetProperty("errors", out var errorsElement))
                        root.TryGetProperty("Errors", out errorsElement);

                    if (errorsElement.ValueKind == JsonValueKind.Object)
                    {
                        validationErrors = string.Join("; ", errorsElement
                            .EnumerateObject()
                            .SelectMany(p => p.Value.EnumerateArray().Select(v => v.GetString()))
                            .Where(s => s != null));
                    }
                }
                catch { }

               return new Response<Guid>
                {
                    Success = false,
                    Message = "Invalid data was submitted",
                    ValidationErrors = validationErrors
                };
            }
            else if (exception.StatusCode == 404)
            {
                return new Response<Guid>
                {
                    Success = false,
                    Message ="The record was not found"
                };
            }
            else
            {
                return new Response<Guid>
                {
                    Success = false,
                    Message ="Something went wrong, please try again later"
                };
            }
        }
    }
}