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
            string logType = null;
            int offset = 0;
            int limit = 100;
            
            if (parameters != null)
            {
                // Extract logType
                if (parameters.ContainsKey("logType") && parameters["logType"] != null)
                {
                    logType = parameters["logType"].ToString()?.ToLowerInvariant();
                    if (string.IsNullOrWhiteSpace(logType))
                    {
                        logType = null;
                    }
                }
                
                // Extract pagination parameters
                if (parameters.ContainsKey("offset") && parameters["offset"] != null)
                {
                    int.TryParse(parameters["offset"].ToString(), out offset);
                }
                
                if (parameters.ContainsKey("limit") && parameters["limit"] != null)
                {
                    int.TryParse(parameters["limit"].ToString(), out limit);
                }
            }

            // Use the new paginated method
            JObject result = _consoleLogsService.GetLogsAsJson(logType, offset, limit);
            
            // Add success info to the response
            result["success"] = true;
            
            var pagination = result["pagination"] as JObject;
            string typeFilter = logType != null ? $" of type '{logType}'" : "";
            result["message"] = $"Retrieved {pagination["returnedCount"]} of {pagination["filteredCount"]} log entries{typeFilter} (offset: {offset}, limit: {limit})";

            return result;
        }


    }
}
