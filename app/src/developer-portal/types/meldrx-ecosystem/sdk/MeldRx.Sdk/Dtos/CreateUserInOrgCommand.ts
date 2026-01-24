import type { OrganizationRoles } from '../Enums/OrganizationRoles';

export interface CreateUserInOrgCommand {
    email: string;
    password?: string;
    confirmPassword?: string;
    firstName: string;
    middleName?: string;
    lastName: string;
    organizationId: string;
    organizationRole: OrganizationRoles;
}
