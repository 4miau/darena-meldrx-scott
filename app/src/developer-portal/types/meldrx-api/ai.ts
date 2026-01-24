import type {
    ChatPayload,
    UpdateGithubModelsSettingsCommand,
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/AiInference';
import type { WorkspaceDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto';
import type {
    AddWorkspaceModel,
    AiModelDto,
    WorkspaceModelDto,
    UpdateWorkspaceModel,
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/AiModelDto';
import type { GitHubTokenDto } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/GitHubTokenDto';
import type { McpServerDto } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/McpServerDto';
import type { TestToolDto } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/TestToolDto';

export interface AiApi {
    updateGithubSettings: (
        workspaceSlug: string,
        command: UpdateGithubModelsSettingsCommand
    ) => Promise<WorkspaceDto>;
    getGitHubToken: (workspaceSlug: string) => Promise<GitHubTokenDto>;
    getModels: (workspaceSlug: string) => Promise<AiModelDto[]>;
    inference: (
        workspaceSlug: string,
        form: ChatPayload
    ) => Promise<ReadableStream>;
    getWorkspaceModels: (workspaceSlug: string) => Promise<WorkspaceModelDto[]>;
    addWorkspaceModel: (
        workspaceSlug: string,
        form: AddWorkspaceModel
    ) => Promise<WorkspaceModelDto>;
    updateWorkspaceModel: (
        workspaceSlug: string,
        form: UpdateWorkspaceModel
    ) => Promise<WorkspaceModelDto>;
    deleteWorkspaceModel: (
        workspaceSlug: string,
        modelId: string
    ) => Promise<void>;
    listMcpServers: (workspaceSlug: string) => Promise<McpServerDto[]>;
    createMcpServer: (
        workspaceSlug: string,
        mcpServer: McpServerDto
    ) => Promise<McpServerDto>;
    updateMcpServer: (
        workspaceSlug: string,
        mcpServer: McpServerDto
    ) => Promise<McpServerDto>;
    deleteMcpServer: (workspaceSlug: string, id: string) => Promise<void>;
    testMcpServer: (
        workspaceSlug: string,
        mcpServer: McpServerDto
    ) => Promise<TestToolDto[]>;
    toggleDefaultMcpTools: (
        workspaceSlug: string,
        value: boolean
    ) => Promise<void>;
}
