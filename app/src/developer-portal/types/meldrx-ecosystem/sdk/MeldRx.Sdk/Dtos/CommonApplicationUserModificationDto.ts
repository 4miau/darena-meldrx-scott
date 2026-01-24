import type { CreateModifyOrganizationUserRelationDto } from './Organization/CreateModifyOrganizationUserRelationDto';

export interface CommonApplicationUserModificationDto {
    firstName: string;
    middleName: string;
    lastName: string;
    npi: string;
    externalUserId: string;
    active: boolean;
    isOrganizationModerator: boolean;
    isUserModerator: boolean;
    organizationUserRelations: CreateModifyOrganizationUserRelationDto[];
}
