import type { FhirServerType } from '../Enums/FhirServerType';
import type { FhirServerDatabase } from '../Models/MeldRxFhirServer';
import type LinkedFhirApiDto from './DynamicRegistration/LinkedFhirApiDto';
import type { PersonDto } from './PersonDto';
import type { UserPersonRelationshipDto } from './UserPersonRelationshipDto';
import type { Guid } from '~/types/common/Guid';
import type { FhirServerValidationOption } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/FhirServerValidationOption';

export interface WorkspaceDto {
    linkedFhirApiDto: LinkedFhirApiDto | null;
    fhirServerDatabase: FhirServerDatabase;
    resellerId: Guid | null;
    organizationId: Guid | null;
    organizationName: string;
    resellerName: string;
    id: Guid;
    sharedFhirServerId: Guid | null;
    name: string;
    description: string;
    fhirDatabaseDisplayName: string;
    ehrLaunchUrl?: string;
    fhirServerEndpoint: string;
    pfrPatientId: string;
    subscriptionType?: string;
    type: FhirServerType;
    validationOption: FhirServerValidationOption;
    isSandbox: boolean;
    isHiddenFromDirectoryListing: boolean;
    allowAnonymousReadAccess: boolean;
    isLiteWorkspace: boolean;
    workspaceIdentifier: string;
    isGithubModelsActive: boolean;
    githubModelsToken: string;
    enableDefaultMcpTools: boolean;
}

export interface CreateNewWorkspaceWithOrgResult {
    workspaceDto: WorkspaceDto;
    organizationId: Guid;
    userId: Guid;
}

export interface PersonWorkspaceDto {
    workspaceDto: WorkspaceDto;
    personDto: PersonDto;
    userPersonRelationshipRoleDto: UserPersonRelationshipDto;
    accessiblePatientId: string;
}
