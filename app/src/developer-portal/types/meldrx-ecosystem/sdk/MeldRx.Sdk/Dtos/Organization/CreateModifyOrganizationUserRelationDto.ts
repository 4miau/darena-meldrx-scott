import type { OrganizationRoles } from '../../Enums/OrganizationRoles';

export interface CreateModifyOrganizationUserRelationDto {
    applicationUserId: string;
    organizationId: string;
    organizationRole: OrganizationRoles;
}
