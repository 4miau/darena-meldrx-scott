import type { WorkspaceDto } from './Dtos/WorkspaceDto';
import type { FhirServerRole } from './Enums/FhirServerRole';
import type { GrantEntityType } from './Enums/GrantEntityType';
import type { UserPersonRelationshipType } from './Enums/UserPersonRelationshipType';

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
