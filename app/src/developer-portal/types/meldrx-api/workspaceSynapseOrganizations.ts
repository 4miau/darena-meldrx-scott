import type {
    CreateWorkspaceSynapseOrganization, UpdateWorkspaceSynapseOrganization,
    WorkspaceSynapseOrganization
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceSynapseOrganization";
import type {PagedResult} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult";
import type {ApiFilter} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter";

export interface WorkspaceSynapseOrganizationApi {
    getWorkspaceSynapseOrganizations:  (workspaceSlug: string, apiFilter: ApiFilter) => Promise<PagedResult<WorkspaceSynapseOrganization>>;
    createWorkspaceSynapseOrganization: (workspaceSlug: string, synapseOrganization: CreateWorkspaceSynapseOrganization) => Promise<WorkspaceSynapseOrganization>;
    updateWorkspaceSynapseOrganization: (workspaceSlug: string, synapseOrganization: UpdateWorkspaceSynapseOrganization) => Promise<WorkspaceSynapseOrganization>;
    deleteWorkspaceSynapseOrganization: (workspaceSlug: string, synapseOrganizationId: string) => Promise<boolean>;
}
    