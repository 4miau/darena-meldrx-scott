export interface ChatPayload {
    model: string;
    systemMessage: string;
    chatMessage: string;
    base64BinaryData?: string;
    base64BinaryDataName?: string;
    patientId?: string;
}

export interface AiInferenceResponse {
    content?: string;
    promptToken: number;
    completionToken: number;
    totalTokens: number;
    toolCalls: string[];
}

export interface UpdateGithubModelsSettingsCommand {
    isActive: boolean;
    githubToken?: string;
}
