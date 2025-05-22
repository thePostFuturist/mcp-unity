using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json.Linq;

namespace McpUnity.Resources
{
    /// <summary>
    /// Resource for retrieving the first screenshot image found in Assets/Screenshots
    /// </summary>
    public class GetScreenshotResource : McpResourceBase
    {
        public GetScreenshotResource()
        {
            Name = "get_screenshot";
            Description = "Retrieves the first screenshot image found in Assets/Screenshots";
            Uri = "unity://screenshot";
        }

        /// <summary>
        /// Fetch the first screenshot image and return it as base64 encoded data
        /// </summary>
        public override JObject Fetch(JObject parameters)
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
