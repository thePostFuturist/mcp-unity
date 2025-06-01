using System;
using System.IO;
using System.Collections.Generic;
using McpUnity.Unity;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace McpUnity.Utils
{
    /// <summary>
    /// Utility class for MCP configuration operations
    /// </summary>
    /// <summary>
    /// Utility class for MCP configuration and system operations
    /// </summary>
    public static class McpUtils
    {
        /// <summary>
        /// Generates the MCP configuration JSON to setup the Unity MCP server in different AI Clients
        /// </summary>
        public static string GenerateMcpConfigJson(bool useTabsIndentation)
        {
            var config = new Dictionary<string, object>
            {
                { "mcpServers", new Dictionary<string, object>
                    {
                        { "mcp-unity", new Dictionary<string, object>
                            {
                                { "command", "node" },
                                { "args", new[] { Path.Combine(GetServerPath(), "build", "index.js") } }
                            }
                        }
                    }
                }
            };
            
            // Initialize string writer with proper indentation
            var stringWriter = new StringWriter();
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                
                // Set indentation character and count
                if (useTabsIndentation)
                {
                    jsonWriter.IndentChar = '\t';
                    jsonWriter.Indentation = 1;
                }
                else
                {
                    jsonWriter.IndentChar = ' ';
                    jsonWriter.Indentation = 2;
                }
                
                // Serialize directly to the JsonTextWriter
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, config);
            }
            
            return stringWriter.ToString().Replace("\\", "/").Replace("//", "/");
        }

        /// <summary>
        /// Gets the absolute path to the Server directory containing package.json (root server dir).
        /// Works whether MCP Unity is installed via Package Manager or directly in the Assets folder
        /// </summary>
        public static string GetServerPath()
        {
            // First, try to find the package info via Package Manager
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath($"Packages/{McpUnitySettings.PackageName}");
                
            if (packageInfo != null && !string.IsNullOrEmpty(packageInfo.resolvedPath))
            {
                return Path.Combine(packageInfo.resolvedPath, "Server~");
            }
            
            var assets = AssetDatabase.FindAssets("tsconfig");

            if(assets.Length == 1)
            {
                // Convert relative path to absolute path
                var relativePath = AssetDatabase.GUIDToAssetPath(assets[0]);
                return Path.GetFullPath(Path.Combine(Application.dataPath, "..", relativePath));
            }
            if (assets.Length > 0)
            {
                foreach (var assetJson in assets)
                {
                    string relativePath = AssetDatabase.GUIDToAssetPath(assetJson);
                    string fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", relativePath));
                    
                    if(Path.GetFileName(Path.GetDirectoryName(fullPath)) == "Server~")
                    {
                        return Path.GetDirectoryName(fullPath);
                    }
                }
            }
            
            // If we get here, we couldn't find the server path
            var errorString = "[MCP Unity] Could not locate Server directory. Please check the installation of the MCP Unity package.";

            Debug.LogError(errorString);

            return errorString;
        }

        /// <summary>
        /// Adds the MCP configuration to the Windsurf MCP config file
        /// </summary>
        public static bool AddToWindsurfIdeConfig(bool useTabsIndentation)
        {
            string configFilePath = GetWindsurfMcpConfigPath();
            return AddToConfigFile(configFilePath, useTabsIndentation, "Windsurf");
        }
        
        /// <summary>
        /// Adds the MCP configuration to the Claude Desktop config file
        /// </summary>
        public static bool AddToClaudeDesktopConfig(bool useTabsIndentation)
        {
            string configFilePath = GetClaudeDesktopConfigPath();
            return AddToConfigFile(configFilePath, useTabsIndentation, "Claude Desktop");
        }
        
        /// <summary>
        /// Adds the MCP configuration to the Cursor config file
        /// </summary>
        public static bool AddToCursorConfig(bool useTabsIndentation)
        {
            string configFilePath = GetCursorConfigPath();
            return AddToConfigFile(configFilePath, useTabsIndentation, "Cursor");
        }

        /// <summary>
        /// Common method to add MCP configuration to a specified config file
        /// </summary>
        /// <param name="configFilePath">Path to the config file</param>
        /// <param name="useTabsIndentation">Whether to use tabs for indentation</param>
        /// <param name="productName">Name of the product (for error messages)</param>
        /// <returns>True if successfuly added the config, false otherwise</returns>
        private static bool AddToConfigFile(string configFilePath, bool useTabsIndentation, string productName)
        {
            if (string.IsNullOrEmpty(configFilePath))
            {
                Debug.LogError($"{productName} config file not found. Please make sure {productName} is installed.");
                return false;
            }
                
            // Generate fresh MCP config JSON
            string mcpConfigJson = GenerateMcpConfigJson(useTabsIndentation);
                
            // Parse the MCP config JSON
            JObject mcpConfig = JObject.Parse(mcpConfigJson);
            
            try
            {
                // Check if the file exists
                if (File.Exists(configFilePath))
                {
                    // Read the existing config
                    string existingConfigJson = File.ReadAllText(configFilePath);
                    JObject existingConfig = string.IsNullOrEmpty(existingConfigJson) ? new JObject() : JObject.Parse(existingConfigJson);
                    
                    // Merge the mcpServers from our config into the existing config
                    if (mcpConfig["mcpServers"] != null && mcpConfig["mcpServers"] is JObject mcpServers)
                    {
                        // Create mcpServers object if it doesn't exist
                        if (existingConfig["mcpServers"] == null)
                        {
                            existingConfig["mcpServers"] = new JObject();
                        }
                        
                        // Add or update the mcp-unity server config
                        if (mcpServers["mcp-unity"] != null)
                        {
                            ((JObject)existingConfig["mcpServers"])["mcp-unity"] = mcpServers["mcp-unity"];
                        }
                        
                        // Write the updated config back to the file
                        File.WriteAllText(configFilePath, existingConfig.ToString(Formatting.Indented));
                        return true;
                    }
                }
                else if(Directory.Exists(Path.GetDirectoryName(configFilePath)))
                {
                    // Create a new config file with just our config
                    File.WriteAllText(configFilePath, mcpConfigJson);
                    return true;
                }
                else
                {
                    Debug.LogError($"Cannot find {productName} config file or {productName} is currently not installed. Expecting {productName} to be installed in the {configFilePath} path");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to add MCP configuration to {productName}: {ex}");
            }

            return false;
        }
        
        /// <summary>
        /// Gets the path to the Windsurf MCP config file based on the current OS
        /// </summary>
        /// <returns>The path to the Windsurf MCP config file</returns>
        private static string GetWindsurfMcpConfigPath()
        {
            // Base path depends on the OS
            string basePath;
            
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                // Windows: %USERPROFILE%/.codeium/windsurf
                basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".codeium/windsurf");
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                // macOS: ~/Library/Application Support/.codeium/windsurf
                string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                basePath = Path.Combine(homeDir, ".codeium/windsurf");
            }
            else
            {
                // Unsupported platform
                Debug.LogError("Unsupported platform for Windsurf MCP config");
                return null;
            }
            
            // Return the path to the mcp_config.json file
            return Path.Combine(basePath, "mcp_config.json");
        }
        
        /// <summary>
        /// Gets the path to the Claude Desktop config file based on the current OS
        /// </summary>
        /// <returns>The path to the Claude Desktop config file</returns>
        private static string GetClaudeDesktopConfigPath()
        {
            // Base path depends on the OS
            string basePath;
            
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                // Windows: %USERPROFILE%/AppData/Roaming/Claude
                basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Claude");
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                // macOS: ~/Library/Application Support/Claude
                string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                basePath = Path.Combine(homeDir, "Library", "Application Support", "Claude");
            }
            else
            {
                // Unsupported platform
                Debug.LogError("Unsupported platform for Claude Desktop config");
                return null;
            }
            
            // Return the path to the claude_desktop_config.json file
            return Path.Combine(basePath, "claude_desktop_config.json");
        }
        
        
        
        /// <summary>
        /// Gets the path to the Cursor config file based on the current OS
        /// </summary>
        /// <returns>The path to the Cursor config file</returns>
        private static string GetCursorConfigPath()
        {
            // Base path depends on the OS
            string basePath;
            
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                // Windows: %USERPROFILE%/.cursor
                basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cursor");
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                // macOS: ~/.cursor
                string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                basePath = Path.Combine(homeDir, ".cursor");
            }
            else
            {
                // Unsupported platform
                Debug.LogError("Unsupported platform for Cursor MCP config");
                return null;
            }
            
            // Return the path to the mcp_config.json file
            return Path.Combine(basePath, "mcp.json");
        }

        /// <summary>
        /// Runs an npm command (such as install or build) in the specified working directory.
        /// Handles cross-platform compatibility (Windows/macOS/Linux) for invoking npm.
        /// Logs output and errors to the Unity console.
        /// </summary>
        /// <param name="arguments">Arguments to pass to npm (e.g., "install" or "run build").</param>
        /// <param name="workingDirectory">The working directory where the npm command should be executed.</param>
        public static void RunNpmCommand(string arguments, string workingDirectory)
        {
            string npmExecutable = McpUnitySettings.Instance.NpmExecutablePath;
            bool useCustomNpmPath = !string.IsNullOrWhiteSpace(npmExecutable);

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
            {
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false, // Important for redirection and direct execution
                CreateNoWindow = true
            };

            if (useCustomNpmPath)
            {
                // Use the custom path directly
                startInfo.FileName = npmExecutable;
                startInfo.Arguments = arguments;
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                // Fallback to cmd.exe to find 'npm' in PATH
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = $"/c npm {arguments}";
            }
            else // macOS / Linux
            {
                // Fallback to /bin/bash to find 'npm' in PATH
                startInfo.FileName = "/bin/bash";
                startInfo.Arguments = $"-c \"npm {arguments}\"";
            }

            try
            {
                using (var process = System.Diagnostics.Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        Debug.LogError($"[MCP Unity] Failed to start npm process with arguments: {arguments} in {workingDirectory}. Process object is null.");
                        return;
                    }

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        Debug.Log($"[MCP Unity] npm {arguments} completed successfully in {workingDirectory}.\n{output}");
                    }
                    else
                    {
                        Debug.LogError($"[MCP Unity] npm {arguments} failed in {workingDirectory}. Exit Code: {process.ExitCode}. Error: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Use commandToLog here
                Debug.LogError($"[MCP Unity] Exception while running npm {arguments} in {workingDirectory}. Error: {ex.Message}");
            }
        }
    }
}
