import type LinkedFhirApiDto from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/LinkedFhirApiDto';
import type LinkedFhirApiActionConfigDto from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/LinkedFhirApiActionConfigDto';

export interface LinkedFhirApiActionConfigApi {
    list: (workspaceId: string) => Promise<LinkedFhirApiActionConfigDto[]>;
    upsert: (workspaceId: string, command: LinkedFhirApiActionConfigDto) => Promise<LinkedFhirApiDto>;
    bulkUpsert: (workspaceId: string, command: LinkedFhirApiActionConfigDto[]) => Promise<LinkedFhirApiDto>;
}
