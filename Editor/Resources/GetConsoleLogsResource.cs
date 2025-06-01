using Newtonsoft.Json.Linq;
using McpUnity.Services;

namespace McpUnity.Resources
{
    /// <summary>
    /// Resource for retrieving all logs from the Unity console
    /// </summary>
    public class GetConsoleLogsResource : McpResourceBase
    {
        private readonly IConsoleLogsService _consoleLogsService;

        public GetConsoleLogsResource(IConsoleLogsService consoleLogsService)
        {
            Name = "get_console_logs";
            Description = "Retrieves logs from the Unity console (newest first), optionally filtered by type (error, warning, info). Use pagination parameters (offset, limit) to avoid LLM token limits. Recommended: limit=20-50 for optimal performance.";
            Uri = "unity://logs/{logType}";
            
            _consoleLogsService = consoleLogsService;
        }

        /// <summary>
        /// Fetch logs from the Unity console, optionally filtered by type with pagination support
        /// </summary>
        /// <param name="parameters">Resource parameters as a JObject (may include 'logType', 'offset', 'limit')</param>
        /// <returns>A JObject containing the list of logs with pagination info</returns>
        public override JObject Fetch(JObject parameters)
        {
            string logType = parameters?["logType"]?.ToString();
            if (string.IsNullOrWhiteSpace(logType)) logType = null;
            
            int offset = Math.Max(0, GetIntParameter(parameters, "offset", 0));
            int limit = Math.Max(1, Math.Min(1000, GetIntParameter(parameters, "limit", 100)));

            // Use the new paginated method
            JObject result = _consoleLogsService.GetLogsAsJson(logType, offset, limit);
            
            // Add success info to the response
            result["success"] = true;
            
            var pagination = result["pagination"] as JObject;
            string typeFilter = logType != null ? $" of type '{logType}'" : "";
            int returnedCount = pagination?["returnedCount"]?.Value<int>() ?? 0;
            int filteredCount = pagination?["filteredCount"]?.Value<int>() ?? 0;
            result["message"] = $"Retrieved {returnedCount} of {filteredCount} log entries{typeFilter} (offset: {offset}, limit: {limit})";

            return result;
        }

        /// <summary>
        /// Helper method to safely extract integer parameters with default values
        /// </summary>
        /// <param name="parameters">JObject containing parameters</param>
        /// <param name="key">Parameter key to extract</param>
        /// <param name="defaultValue">Default value if parameter is missing or invalid</param>
        /// <returns>Extracted integer value or default</returns>
        private static int GetIntParameter(JObject parameters, string key, int defaultValue)
        {
            if (parameters?[key] != null && int.TryParse(parameters[key].ToString(), out int value))
                return value;
            return defaultValue;
        }


    }
}
