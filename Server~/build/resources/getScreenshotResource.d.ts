import { McpUnity } from '../unity/mcpUnity.js';
import { Logger } from '../utils/logger.js';
import { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
export declare const resourceName = "get_screenshot";
export declare function registerGetScreenshotResource(server: McpServer, mcpUnity: McpUnity, logger: Logger): void;
