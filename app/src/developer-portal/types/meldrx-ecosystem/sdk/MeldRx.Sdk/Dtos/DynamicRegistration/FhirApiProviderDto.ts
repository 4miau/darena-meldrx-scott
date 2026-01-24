import type { Guid } from '~/types/common/Guid';

export default interface FhirApiProviderDto
{
    id : Guid;
    hasSandbox : boolean;
    productName : string;
    organizationName : string;
    chplId : string;
    version : string;
    apiDocumentationUrl : string;
    meldRxIdentifier : string;
    portalUrl : string;
    discussionId : string;
}
