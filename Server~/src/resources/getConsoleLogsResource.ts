import { Logger } from '../utils/logger.js';
import { ResourceTemplate, McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { ReadResourceResult } from '@modelcontextprotocol/sdk/types.js';
import { McpUnity } from '../unity/mcpUnity.js';
import { McpUnityError, ErrorType } from '../utils/errors.js';
import { Variables } from '@modelcontextprotocol/sdk/shared/uriTemplate.js';

// Constants for the resource
const resourceName = 'get_console_logs';
const resourceMimeType = 'application/json';
const resourceUri = 'unity://logs/{logType}?offset={offset}&limit={limit}';
const resourceTemplate = new ResourceTemplate(resourceUri, {
  list: () => listLogTypes(resourceMimeType)
});

function listLogTypes(resourceMimeType: string) {
  return {
    resources: [
      {
        uri: `unity://logs/`,
        name: "All logs",
        description: "Retrieve Unity console logs (newest first). Use pagination to avoid token limits: ?offset=0&limit=50 for recent logs. Default limit=100 may be too large for LLM context.",
        mimeType: resourceMimeType
      },
      {
        uri: `unity://logs/error`,
        name: "Error logs",
        description: "Retrieve only error logs from Unity console (newest first). Use ?offset=0&limit=20 to avoid token limits. Large log sets may exceed LLM context window.",
        mimeType: resourceMimeType
      },
      {
        uri: `unity://logs/warning`,
        name: "Warning logs", 
        description: "Retrieve only warning logs from Unity console (newest first). Use pagination ?offset=0&limit=30 to manage token usage effectively.",
        mimeType: resourceMimeType
      },
      {
        uri: `unity://logs/info`,
        name: "Info logs",
        description: "Retrieve only info logs from Unity console (newest first). Use smaller limits like ?limit=25 to prevent token overflow in LLM responses.",
        mimeType: resourceMimeType
      }
    ]
  };
}

/**
 * Registers the get_console_logs resource with the MCP server
 */
export function registerGetConsoleLogsResource(server: McpServer, mcpUnity: McpUnity, logger: Logger) {
  logger.info(`Registering resource: ${resourceName}`);

  server.resource(
    resourceName,
    resourceTemplate,
    {
      description: 'Retrieve Unity console logs by type (newest first). IMPORTANT: Use pagination parameters ?offset=0&limit=50 to avoid LLM token limits. Default limit=100 may exceed context window.',
      mimeType: resourceMimeType
    },
    async (uri, variables) => {
      try {
        return await resourceHandler(mcpUnity, uri, variables, logger);
      } catch (error) {
        logger.error(`Error handling resource ${resourceName}: ${error}`);
        throw error;
      }
    }
  );
}

/**
 * Handles requests for Unity console logs by log type
 */
async function resourceHandler(mcpUnity: McpUnity, uri: URL, variables: Variables, logger: Logger): Promise<ReadResourceResult> {
  // Extract and convert the parameter from the template variables
  let logType = variables["logType"] ? decodeURIComponent(variables["logType"] as string) : undefined;
  if (logType === '') logType = undefined;
  
  // Extract pagination parameters
  const offset = variables["offset"] ? parseInt(variables["offset"] as string, 10) : 0;
  const limit = variables["limit"] ? parseInt(variables["limit"] as string, 10) : 100;

  // Send request to Unity
  const response = await mcpUnity.sendRequest({
    method: resourceName,
    params: {
      logType: logType,
      offset: offset,
      limit: limit
    }
  });

  if (!response.success) {
    throw new McpUnityError(
      ErrorType.RESOURCE_FETCH,
      response.message || 'Failed to fetch logs from Unity'
    );
  }

  return {
    contents: [{
      uri: `unity://logs/${logType ?? ''}?offset=${offset}&limit=${limit}`,
      mimeType: resourceMimeType,
      text: JSON.stringify(response, null, 2)
    }]
  };
}
