import type { ExternalIdp } from '../Enums/ExternalIdp';
import type { OrganizationRoles } from '../Enums/OrganizationRoles';
import type { CommonApplicationUserModificationDto } from './CommonApplicationUserModificationDto';

export interface CreateUserInOrgOrWorkspaceBaseCommand {
    email: string;
    password?: string;
    confirmPassword?: string;
    firstName: string;
    middleName?: string;
    lastName: string;
    externalIdp?: ExternalIdp;
    externalId?: string;
    organizationRole: OrganizationRoles;
}

export interface CreateUserInOrgCommand extends CreateUserInOrgOrWorkspaceBaseCommand {
    organizationId: string;
}

export interface CreateUserInWorkspaceCommand extends CreateUserInOrgOrWorkspaceBaseCommand {
    workspaceSlug: string;
}

export interface CreateApplicationUserDto extends CommonApplicationUserModificationDto {
    sendRegistrationEmail: boolean;
    email: string;
    password: string;
    confirmPassword: string;
    clientContext: string;
}
