using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json.Linq;

namespace McpUnity.Tools
{
    /// <summary>
    /// Tool for retrieving the first screenshot image found in Assets/Screenshots
    /// </summary>
    public class GetScreenshotFunctionTool : McpToolBase
    {
        public GetScreenshotFunctionTool()
        {
            Name = "get_screenshot_function";
            Description = "Retrieves the first screenshot image found in Assets/Screenshots";
        }

        /// <summary>
        /// Execute the GetScreenshotFunction tool synchronously
        /// </summary>
        /// <param name="parameters">Tool parameters as a JObject</param>
        public override JObject Execute(JObject parameters)
        {
            string screenshotsFolder = Path.Combine(Application.dataPath, "Screenshots");
            if (!Directory.Exists(screenshotsFolder))
            {
                return new JObject
                {
                    ["success"] = false,
                    ["message"] = "Screenshots folder not found"
                };
            }

            string[] files = Directory.GetFiles(screenshotsFolder);
            string imagePath = null;
            foreach (string file in files)
            {
                string ext = Path.GetExtension(file).ToLowerInvariant();
                if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                {
                    imagePath = file;
                    break;
                }
            }

            if (string.IsNullOrEmpty(imagePath))
            {
                return new JObject
                {
                    ["success"] = false,
                    ["message"] = "No screenshot image found"
                };
            }

            byte[] bytes = File.ReadAllBytes(imagePath);
            string base64 = Convert.ToBase64String(bytes);
            string extLower = Path.GetExtension(imagePath).ToLowerInvariant();
            string mimeType = extLower == ".jpg" || extLower == ".jpeg" ? "image/jpeg" : "image/png";
            string assetPath = $"Assets/Screenshots/{Path.GetFileName(imagePath)}";

            return new JObject
            {
                ["success"] = true,
                ["type"] = "image",
                ["mimeType"] = mimeType,
                ["data"] = base64,
                ["path"] = assetPath
            };
        }
    }
}
