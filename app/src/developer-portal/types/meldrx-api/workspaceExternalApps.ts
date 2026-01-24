import type {
    CreateWorkspaceExternalApp, UpdateWorkspaceExternalApp,
    WorkspaceExternalApp
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceExternalApp";

export interface WorkspaceExternalAppsApi {
    getWorkspaceExternalApps:  (workspaceSlug: string) => Promise<WorkspaceExternalApp[]>;
    createWorkspaceExternalApp: (workspaceSlug: string, externalApp: CreateWorkspaceExternalApp) => Promise<WorkspaceExternalApp>;
    updateWorkspaceExternalApp: (workspaceSlug: string, externalApp: UpdateWorkspaceExternalApp) => Promise<WorkspaceExternalApp>;
    deleteWorkspaceExternalApp: (workspaceSlug: string, externalAppId: string) => Promise<boolean>;
}
    