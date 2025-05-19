# MCP Unity Architecture

This document provides a high level overview of how the Node.js server and the Unity side communicate, and how new tools can be added.

## Node.js server startup and WebSocket bridge

The entry point is [`Server~/src/index.ts`](Server~/src/index.ts). The server creates an `McpServer`, registers all tools and resources, then starts communicating with Unity over WebSockets through the `McpUnity` bridge.

```ts
const server = new McpServer({
  name: "MCP Unity Server",
  version: "1.0.0"
}, { capabilities: { tools: {}, resources: {}, prompts: {} } });

const mcpUnity = new McpUnity(unityLogger);

registerMenuItemTool(server, mcpUnity, toolLogger);
registerSelectGameObjectTool(server, mcpUnity, toolLogger);
// ...more tools and resources...
```

The `startServer` function connects to the MCP client via STDIO and then opens the WebSocket connection to Unity:

```ts
async function startServer() {
  const stdioTransport = new StdioServerTransport();
  await server.connect(stdioTransport);
  const clientName = server.server.getClientVersion()?.name || 'Unknown MCP Client';
  await mcpUnity.start(clientName);
}
```

`McpUnity` handles the WebSocket protocol and exposes `sendRequest` for tools and resources to invoke Unity operations.

## Unity WebSocket server and registration

`Editor/UnityBridge/McpUnityServer.cs` hosts the Unity side WebSocket server. When Unity loads the static constructor starts the server if **AutoStartServer** is enabled. `StartServer` creates a `WebSocketServer` and adds a handler for `/McpUnity`:

```csharp
_webSocketServer = new WebSocketServer($"ws://localhost:{McpUnitySettings.Instance.Port}");
_webSocketServer.AddWebSocketService("/McpUnity", () => new McpUnitySocketHandler(this));
_webSocketServer.Start();
```

`RegisterTools` and `RegisterResources` populate dictionaries used by `McpUnitySocketHandler` to dispatch incoming requests:

```csharp
MenuItemTool menuItemTool = new MenuItemTool();
_tools.Add(menuItemTool.Name, menuItemTool);
// ...other tool registrations...
GetMenuItemsResource getMenuItemsResource = new GetMenuItemsResource();
_resources.Add(getMenuItemsResource.Name, getMenuItemsResource);
```

## Adding a new tool – `add_rounded_graphic`

1. **Create the Unity tool**
   - File: `Editor/Tools/AddRoundedGraphicTool.cs`
   - Inherit from `McpToolBase` and set `Name` to `"add_rounded_graphic"`.
   - Implement `Execute` to create or modify the selected UI Graphic with rounded corners.

2. **Register the tool in Unity**
   - Update `McpUnityServer.RegisterTools()` to instantiate `AddRoundedGraphicTool` and add it to the `_tools` dictionary.

3. **Create the Node.js wrapper**
   - File: `Server~/src/tools/addRoundedGraphicTool.ts`.
   - Follow the pattern of `addAssetToSceneTool.ts` to define the schema, call `mcpUnity.sendRequest`, and return a `CallToolResult`.

4. **Register the tool on the Node.js server**
   - Import and call `registerAddRoundedGraphicTool(server, mcpUnity, toolLogger);` in `Server~/src/index.ts` alongside the other tools.

5. **Build the server**
   - Run `npm run build` inside `Server~` to compile the TypeScript files.

After these steps the new tool can be invoked from an MCP client and will forward requests to Unity via the WebSocket bridge.
