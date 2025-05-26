import * as z from 'zod';
import { McpUnityError, ErrorType } from '../utils/errors.js';
const toolName = 'get_text_asset';
const toolDescription = 'Reads the contents of a text file from the Unity project';
const paramsSchema = z.object({
    filePath: z
        .string()
        .describe('Path of the text file relative to the Unity project (e.g., Assets/MyFolder/file.txt)')
});
export function registerGetTextAssetTool(server, mcpUnity, logger) {
    logger.info(`Registering tool: ${toolName}`);
    server.tool(toolName, toolDescription, paramsSchema.shape, async (params) => {
        try {
            logger.info(`Executing tool: ${toolName}`, params);
            const result = await toolHandler(mcpUnity, params);
            logger.info(`Tool execution successful: ${toolName}`);
            return result;
        }
        catch (error) {
            logger.error(`Tool execution failed: ${toolName}`, error);
            throw error;
        }
    });
}
async function toolHandler(mcpUnity, params) {
    const { filePath } = params;
    const response = await mcpUnity.sendRequest({
        method: toolName,
        params: { filePath }
    });
    if (!response.success) {
        throw new McpUnityError(ErrorType.TOOL_EXECUTION, response.message || `Failed to read text asset`);
    }
    return {
        content: [
            {
                type: 'text',
                text: response.data || ''
            }
        ]
    };
}
