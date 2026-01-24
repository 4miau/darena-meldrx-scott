import type { CreateModifyOrganizationUserRelationDto } from './CreateModifyOrganizationUserRelationDto';

export interface OrganizationUserModificationModel {
    organizationId: string;
    usersToAddOrModify: CreateModifyOrganizationUserRelationDto[];
    usersToRemove: string[];
}
export interface OrganizationOrWorkspaceUserModificationModelBase {
    usersToAddOrModify: CreateModifyOrganizationUserRelationDto[];
    usersToRemove: string[];
}
export interface WorkspaceUserModificationModel extends OrganizationOrWorkspaceUserModificationModelBase {
    workspaceSlug: string;
}
