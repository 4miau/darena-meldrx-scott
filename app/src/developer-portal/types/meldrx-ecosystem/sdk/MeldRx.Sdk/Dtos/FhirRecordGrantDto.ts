import type { AppRole } from '../Enums/AppRole';
import type { FhirServerRole } from '../Enums/FhirServerRole';
import type { GrantEntityType } from '../Enums/GrantEntityType';
import type { UserPersonRelationshipType } from '../Enums/UserPersonRelationshipType';
import type { WorkspaceDto } from './WorkspaceDto';

export interface WorkspaceAppPermissionDto {
    id: string;
    slug: string;
    workspaceId: string;
    appId: string;
    role: AppRole;
    name: string;
    createdAt:Date;
}

export interface FhirRecordGrantDto {
    workspaceDto: WorkspaceDto;
    grantId: string | null;
    entityType: GrantEntityType;
    entityId: string;
    relationship: UserPersonRelationshipType | null;
    role: FhirServerRole;
    roleIsInherited: boolean;
    accessiblePatientId: string;
}
