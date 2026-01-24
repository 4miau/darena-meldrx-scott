import type { WorkspaceDto, PersonWorkspaceDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto'

export interface UsersApi {
    people: () => Promise<PersonWorkspaceDto[]>;
    workspaces: () => Promise<WorkspaceDto[]>;
}
