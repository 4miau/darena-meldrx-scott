import type {WorkspaceExternalApp} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceExternalApp";


export interface WorkspaceSynapseOrganization {
    id: string,
    organizationName: string,
    externalAppId: string,
    externalApp?: WorkspaceExternalApp,
    workspaceId: string,
    type: SynapseOrganizationType,
    fhirEndpoint: string,
    authorizationUrl?: string,
    tokenUrl?: string,
    addressLine1?: string,
    addressLine2?: string,
    city?: string,
    state?: string,
    postalcode?: string,
}
export interface CreateWorkspaceSynapseOrganization {
    organizationName: string,
    externalAppId: string,
    type: SynapseOrganizationType,
    fhirEndpoint: string,
    addressLine1?: string,
    addressLine2?: string,
    city?: string,
    state?: string,
    postalcode?: string,
}

export interface UpdateWorkspaceSynapseOrganization {
    id: string,
    organizationName: string,
    externalAppId: string,
    type: SynapseOrganizationType,
    fhirEndpoint: string,
    addressLine1?: string,
    addressLine2?: string,
    city?: string,
    state?: string,
    postalcode?: string,
}

export enum SynapseOrganizationType{
    Provider = 'Provider',
    Payer = 'Payer'
}