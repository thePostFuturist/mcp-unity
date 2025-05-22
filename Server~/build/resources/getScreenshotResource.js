import { McpUnityError, ErrorType } from '../utils/errors.js';
export const resourceName = 'get_screenshot';
const resourceUri = 'unity://screenshot';
const defaultMimeType = 'image/png';
export function registerGetScreenshotResource(server, mcpUnity, logger) {
    logger.info(`Registering resource: ${resourceName}`);
    server.resource(resourceName, resourceUri, {
        description: 'Retrieve the first screenshot image from Assets/Screenshots',
        mimeType: defaultMimeType
    }, async () => {
        try {
            return await resourceHandler(mcpUnity);
        }
        catch (error) {
            logger.error(`Error handling resource ${resourceName}: ${error}`);
            throw error;
        }
    });
}
async function resourceHandler(mcpUnity) {
    const response = await mcpUnity.sendRequest({
        method: resourceName,
        params: {}
    });
    if (!response.success) {
        throw new McpUnityError(ErrorType.RESOURCE_FETCH, response.message || 'Failed to fetch screenshot from Unity');
    }
    const mimeType = response.mimeType || defaultMimeType;
    return {
        contents: [
            {
                uri: resourceUri,
                mimeType,
                blob: response.data
            }
        ]
    };
}
