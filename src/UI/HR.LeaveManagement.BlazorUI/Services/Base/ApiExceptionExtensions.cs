using System.Text.Json;

namespace HR.LeaveManagement.BlazorUI.Services.Base
{
    public static class ApiExceptionExtensions
    {
        /// <summary>
        /// Pulls the human readable message out of the ProblemDetails body returned by the API,
        /// falling back to a generic message when the body cannot be read.
        /// </summary>
        public static string GetErrorMessage(this ApiException exception)
        {
            try
            {
                using var doc = JsonDocument.Parse(exception.Response);
                var root = doc.RootElement;
                // Try camelCase first, then PascalCase — WriteAsJsonAsync may produce either
                if (!root.TryGetProperty("title", out var titleElement))
                    root.TryGetProperty("Title", out titleElement);

                if (titleElement.ValueKind == JsonValueKind.String)
                {
                    var title = titleElement.GetString();
                    if (!string.IsNullOrWhiteSpace(title))
                        return title;
                }
            }
            catch { }

            return "Something went wrong, please try again later";
        }
    }
}
