using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json.Linq;
using McpUnity.Unity;
using McpUnity.Utils;

namespace McpUnity.Tools
{
    /// <summary>
    /// Tool for creating a new text asset inside the Unity project.
    /// </summary>
    public class CreateTextAssetTool : McpToolBase
    {
        public CreateTextAssetTool()
        {
            Name = "create_text_asset";
            Description = "Creates a new text file in the Unity project Assets folder";
        }

        /// <summary>
        /// Execute the CreateTextAsset tool with the provided parameters.
        /// </summary>
        /// <param name="parameters">Tool parameters as a JObject</param>
        public override JObject Execute(JObject parameters)
        {
            string filePath = parameters["filePath"]?.ToObject<string>();
            string contents = parameters["contents"]?.ToObject<string>() ?? string.Empty;
            bool overwrite = parameters["overwrite"]?.ToObject<bool?>() ?? false;

            if (string.IsNullOrEmpty(filePath))
            {
                return McpUnitySocketHandler.CreateErrorResponse(
                    "Required parameter 'filePath' not provided",
                    "validation_error"
                );
            }

            try
            {
                string fullPath = Path.GetFullPath(filePath);
                string directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(fullPath) && !overwrite)
                {
                    return McpUnitySocketHandler.CreateErrorResponse(
                        $"File already exists at path '{filePath}'",
                        "file_exists_error"
                    );
                }

                File.WriteAllText(fullPath, contents);
                AssetDatabase.Refresh();

                McpLogger.LogInfo($"Created text asset at '{filePath}'");

                return new JObject
                {
                    ["success"] = true,
                    ["type"] = "text",
                    ["message"] = $"Text asset created at '{filePath}'"
                };
            }
            catch (Exception ex)
            {
                return McpUnitySocketHandler.CreateErrorResponse(
                    $"Failed to create text asset: {ex.Message}",
                    "file_write_error"
                );
            }
        }
    }
}
