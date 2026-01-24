export enum McpServerTransportType {
    Sse = 'Sse',
    StreamableHttp = 'StreamableHttp',
}

export enum McpServerAuthType {
    ApiKey = 'ApiKey',
    BasicAuth = 'BasicAuth',
    Open = 'Open',
}

export type McpServerDto = {
    id: string;
    endpoint: string;
    transportType: McpServerTransportType;
    authType: McpServerAuthType;
    apiKeyHeaderName?: string;
    apiKeyHeaderValue?: string;
    basicAuthUsername?: string;
    basicAuthPassword?: string;
};
