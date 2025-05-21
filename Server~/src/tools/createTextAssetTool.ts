import * as z from 'zod';
import { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { McpUnity } from '../unity/mcpUnity.js';
import { Logger } from '../utils/logger.js';
import { McpUnityError, ErrorType } from '../utils/errors.js';
import { CallToolResult } from '@modelcontextprotocol/sdk/types.js';

const toolName = 'create_text_asset';
const toolDescription = 'Creates a new text file in the Unity project Assets folder';

const paramsSchema = z.object({
  filePath: z.string().describe('Path of the text file relative to the Unity project (e.g., Assets/MyFolder/file.txt)'),
  contents: z.string().describe('The text contents of the file'),
  overwrite: z.boolean().optional().describe('Whether to overwrite an existing file (default false)')
});

export function registerCreateTextAssetTool(server: McpServer, mcpUnity: McpUnity, logger: Logger) {
  logger.info(`Registering tool: ${toolName}`);

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

async function toolHandler(mcpUnity: McpUnity, params: any): Promise<CallToolResult> {
  const { filePath, contents, overwrite = false } = params;

  const response = await mcpUnity.sendRequest({
    method: toolName,
    params: {
      filePath,
      contents,
      overwrite,
    },
  });

  if (!response.success) {
    throw new McpUnityError(
      ErrorType.TOOL_EXECUTION,
      response.message || `Failed to create text asset`
    );
  }

  return {
    content: [
      {
        type: response.type,
        text: response.message || `Text asset created at ${filePath}`,
      },
    ],
  };
}
