import type { LinkedWorkspacePatientStrategy } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/LinkedWorkspacePatientStrategy'

export default interface LinkedFhirApiCreateDto {
    fhirApiProviderMeldRxIdentifier: string;
    patientStrategy: LinkedWorkspacePatientStrategy;
    baseUrl: string;
    authUrl?: string;
    tokenUrl?: string;
}
