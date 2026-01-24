import type { Guid } from '~/types/common/Guid'

export interface WorkspaceProviderDto {
    id: string;
    npi: string;
    displayName: string;
    workspaceSlug: string;
    directoryIds: Guid[];
    active: boolean;
    createdOn: string;
    dateOfActivation: string;
}
