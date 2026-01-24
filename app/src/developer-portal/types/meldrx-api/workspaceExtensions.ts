import type {
    CreateDeveloperAppCommandResult
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CreateDeveloperApp";
import type {WorkspaceExtensionForm} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceExtensionForm";


export interface WorkspaceExtensionsApi {
    getWorkspaceExtension: ( workspaceSlug: string, appId: string) => Promise<WorkspaceExtensionForm>;
    createWorkspaceExtension: (workspaceSlug:string, form: FormData) => Promise<CreateDeveloperAppCommandResult>;
    updateWorkspaceExtension: (workspaceSlug:string, appId:string, form: FormData) => Promise<WorkspaceExtensionForm>;
    deleteWorkspaceExtension: (workspaceSlug:string, appId: string) => Promise<boolean>;
}
