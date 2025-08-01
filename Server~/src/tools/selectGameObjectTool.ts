import * as z from 'zod';
import { Logger } from '../utils/logger.js';
import { McpUnity } from '../unity/mcpUnity.js';
import { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { McpUnityError, ErrorType } from '../utils/errors.js';
import { CallToolResult } from '@modelcontextprotocol/sdk/types.js';

// Constants for the tool
const toolName = 'select_gameobject';
const toolDescription = 'Sets the selected GameObject in the Unity editor by path, name or instance ID';
const paramsSchema = z.object({
  objectPath: z.string().optional().describe('The path or name of the GameObject to select (e.g. "Main Camera")'),
  objectName: z.string().optional().describe('The name of the GameObject to select'),
  instanceId: z.number().optional().describe('The instance ID of the GameObject to select')
});

/**
 * Creates and registers the Select GameObject tool with the MCP server
 * This tool allows selecting a GameObject in the Unity Editor
 * 
 * @param server The MCP server instance to register with
 * @param mcpUnity The McpUnity instance to communicate with Unity
 * @param logger The logger instance for diagnostic information
 */
export function registerSelectGameObjectTool(server: McpServer, mcpUnity: McpUnity, logger: Logger) {
  logger.info(`Registering tool: ${toolName}`);
      
  // Register this tool with the MCP server
  server.tool(
    toolName,
    toolDescription,
    paramsSchema.shape,
    async (params: any) => {
      try {
        logger.info(`Executing tool: ${toolName}`, params);
        const result = await toolHandler(mcpUnity, params);
        logger.info(`Tool execution successful: ${toolName}`);
        return result;
      } catch (error) {
        logger.error(`Tool execution failed: ${toolName}`, error);
        throw error;
      }
    }
  );
}

/**
 * Handles selecting a GameObject in Unity
 * 
 * @param mcpUnity The McpUnity instance to communicate with Unity
 * @param params The parameters for the tool
 * @returns A promise that resolves to the tool execution result
 * @throws McpUnityError if the request to Unity fails
 */
async function toolHandler(mcpUnity: McpUnity, params: any): Promise<CallToolResult> {
  // Custom validation since we can't use refine/superRefine while maintaining ZodObject type
  if (params.objectPath === undefined && params.objectName === undefined && params.instanceId === undefined) {
    throw new McpUnityError(
      ErrorType.VALIDATION,
      "Either 'objectPath', 'objectName' or 'instanceId' must be provided"
    );
  }
  
  const response = await mcpUnity.sendRequest({
    method: toolName,
    params
  });
  
  if (!response.success) {
    throw new McpUnityError(
      ErrorType.TOOL_EXECUTION,
      response.message || `Failed to select GameObject`
    );
  }
  
  return {
    content: [{
      type: response.type,
      text: response.message || `Successfully selected GameObject`
    }]
  };
}
