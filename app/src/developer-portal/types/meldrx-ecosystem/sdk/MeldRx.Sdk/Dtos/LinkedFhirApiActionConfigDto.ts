import type { LinkedApiAction } from '../Enums/LinkedApiAction';
import type { LinkedApiActionTarget } from '../Enums/LinkedApiActionTarget';
import type { Guid } from '~/types/common/Guid';

export default interface LinkedFhirApiActionConfigDto{
        linkedApiAction: LinkedApiAction; // CRUD
        resourceType: string; // ToDo Confirm if string is ok for Fhir Resources?
        actionTarget: LinkedApiActionTarget; // meldrx - external - both
        linkedFhirApiId: Guid;
        id?: Guid;
}
