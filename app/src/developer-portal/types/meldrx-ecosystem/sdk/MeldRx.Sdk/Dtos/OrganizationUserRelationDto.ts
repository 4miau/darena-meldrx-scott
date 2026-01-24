import type { OrganizationRoles } from '../Enums/OrganizationRoles';
import type { OrganizationType } from '../Enums/OrganizationType';

export interface OrganizationUserRelationDto {
    organizationId: string;
    organizationName: string;
    userId: string;
    firstName: string;
    lastName: string;
    email: string;
    organizationRole: OrganizationRoles;
    organizationType: OrganizationType;
    organizationTin: string;
    createdAt: string;
}
