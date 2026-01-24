import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'
import type { WorkspaceProviderDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceProviderDto'
import type { UpdateWorkspaceProviderInfoCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceProviders/UpdateWorkspaceProviderInfoCommand'
import type { UpdateWorkspaceProviderStatusCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceProviders/UpdateWorkspaceProviderStatusCommand'
import type { ApiFilter } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter'
import type { CreateWorkspaceProvidersFromNpisCommand } from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceProviders/CreateWorkspaceProvidersFromNpisCommand";
import type { CreateWorkspaceProviderCommand } from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceProviders/CreateWorkspaceProviderCommand";

export interface WorkspacesProvidersApi {

    list: (workspaceSlug: string, filter: ApiFilter) => Promise<PagedResult<WorkspaceProviderDto>>;
    updateStatus: (workspaceSlug: string, command: UpdateWorkspaceProviderStatusCommand) => Promise<void>;
    delete: (workspaceSlug: string, command: string) => Promise<void>;
    updateInfo: (workspaceSlug: string, command: UpdateWorkspaceProviderInfoCommand) => Promise<void>;
    createFromNpis: (workspaceSlug: string, command: CreateWorkspaceProvidersFromNpisCommand) => Promise<WorkspaceProviderDto>;
    createFromCommand: (workspaceSlug: string, command: CreateWorkspaceProviderCommand) => Promise<WorkspaceProviderDto>;
}
