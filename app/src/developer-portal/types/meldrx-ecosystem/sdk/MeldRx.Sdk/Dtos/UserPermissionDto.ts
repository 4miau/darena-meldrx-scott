import type { PersonRole } from '../Enums/PersonRole';

export interface DeveloperPermissionsDto {
    organizationId: string;
    isReseller: boolean;
    canCreateApps: boolean;
    canManageUsers: boolean;
    canCreateDeveloperWorkspaces: boolean;
    canSellWorkspaces: boolean;
}

export interface UserPermissionsDto {
    isDeveloper: boolean;
    hasPeople: boolean;
    hasWorkspaces: boolean;
    developerPermissionsDto: DeveloperPermissionsDto | null;
    workspaceUserPermissionDto: WorkspaceUserPermissionDto[] | null;
    hasMipsReports: boolean;
}

export interface PersonPermissionDto {
    personId: string;
    accessiblePatientId: string;
    personRole: PersonRole;
}
export interface WorkspaceUserPermissionDto {
    notFound: boolean;
    slug: string;
    id: string;
    hasPersonAccess: boolean;
    hasOrgAccess: boolean;
    organizationId: string;
    resellerId: string;
    canUpdateMetadata: boolean;
    isReseller: boolean;
    canManageUsers: boolean;
    canWrite: boolean;
    canRead: boolean;
    canSendInvites: boolean;
    canApproveApps: boolean;
    canAddExtensions: boolean;
    canPerformBulkOperations: boolean;
    personPermissionDtos: PersonPermissionDto[];
}

export interface UserPermissionInOrg {
    hasAccess: boolean;
    canManageUsers: boolean;
}
