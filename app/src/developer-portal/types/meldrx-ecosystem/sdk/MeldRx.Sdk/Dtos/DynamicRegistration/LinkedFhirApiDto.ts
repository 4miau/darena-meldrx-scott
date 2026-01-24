import type LinkedFhirApiCreateDto from './LinkedFhirApiCreateDto';
import type { Guid } from '~/types/common/Guid';

export default interface LinkedFhirApiDto extends LinkedFhirApiCreateDto {
    id?: Guid;
    fhirApiProviderProductName: string;
    fhirApiProviderOrganizationName: string;
    fhirApiProviderVersion: string;
    launchAppClientId?: string;
    launchAppScopes?: string;
}
