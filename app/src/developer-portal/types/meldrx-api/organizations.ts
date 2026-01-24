import type { CreateUserInOrgCommand } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CreateUserInOrgCommand';
import type { OrganizationUserModificationModel } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Organization/OrganizationUserModificationModel';
import type { OrganizationUserRelationDto } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/OrganizationUserRelationDto';

export interface OrganizationsApi {
    listUsers: (organizationId: string) => Promise<OrganizationUserRelationDto[]>;
    createUsers: (organizationId: string, command: CreateUserInOrgCommand) => Promise<OrganizationUserRelationDto[]>;
    updateUsers: (organizationId: string, command: OrganizationUserModificationModel) => Promise<OrganizationUserRelationDto[]>;
}
