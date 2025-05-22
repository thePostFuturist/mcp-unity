using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json.Linq;
using McpUnity.Unity;
using McpUnity.Utils;

namespace McpUnity.Tools
{
    /// <summary>
    /// Tool for taking a screenshot of the Game View and saving it to Assets/Screenshots
    /// </summary>
    public class TakeScreenshotTool : McpToolBase
    {
        public TakeScreenshotTool()
        {
            Name = "take_screenshot";
            Description = "Takes a screenshot of the Game View and saves it to Assets/Screenshots";
        }

        /// <summary>
        /// Execute the TakeScreenshot tool synchronously
        /// </summary>
        /// <param name="parameters">Tool parameters as a JObject</param>
        public override JObject Execute(JObject parameters)
        {
            // Ensure the Screenshots folder exists
            string screenshotsFolder = Path.Combine(Application.dataPath, "Screenshots");
            if (!Directory.Exists(screenshotsFolder))
            {
                Directory.CreateDirectory(screenshotsFolder);
            }
            string fileName = $"screenshot.png";
            // Determine file name

            // string fileName = parameters?["fileName"]?.ToObject<string>();
            // if (string.IsNullOrEmpty(fileName))
            // {
            //     fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            // }

            string fullPath = Path.Combine(screenshotsFolder, fileName);

            // Capture the screenshot
            ScreenCapture.CaptureScreenshot(fullPath);

            // Refresh AssetDatabase so screenshot appears in Project window
            AssetDatabase.Refresh();

            string assetPath = $"Assets/Screenshots/{fileName}";

            McpLogger.LogInfo($"Screenshot captured: {assetPath}");

            return new JObject
            {
                ["success"] = true,
                ["type"] = "text",
                ["message"] = $"Screenshot saved to {assetPath}",
                ["path"] = assetPath
            };
        }
    }
}
