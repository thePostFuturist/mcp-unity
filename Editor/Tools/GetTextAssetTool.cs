using System;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using McpUnity.Unity;
using McpUnity.Utils;

namespace McpUnity.Tools
{
    /// <summary>
    /// Tool for reading the contents of a text asset inside the Unity project.
    /// </summary>
    public class GetTextAssetTool : McpToolBase
    {
        public GetTextAssetTool()
        {
            Name = "get_text_asset";
            Description = "Reads the contents of a text file from the Unity project";
        }

        /// <summary>
        /// Execute the GetTextAsset tool with the provided parameters.
        /// </summary>
        /// <param name="parameters">Tool parameters as a JObject</param>
        public override JObject Execute(JObject parameters)
        {
            string filePath = parameters["filePath"]?.ToObject<string>();

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
                if (!File.Exists(fullPath))
                {
                    return McpUnitySocketHandler.CreateErrorResponse(
                        $"File not found at path '{filePath}'",
                        "file_not_found"
                    );
                }

                string contents = File.ReadAllText(fullPath);

                return new JObject
                {
                    ["success"] = true,
                    ["type"] = "text",
                    ["data"] = contents,
                    ["path"] = filePath.Replace("\\", "/")
                };
            }
            catch (Exception ex)
            {
                return McpUnitySocketHandler.CreateErrorResponse(
                    $"Failed to read text asset: {ex.Message}",
                    "file_read_error"
                );
            }
        }
    }
}
