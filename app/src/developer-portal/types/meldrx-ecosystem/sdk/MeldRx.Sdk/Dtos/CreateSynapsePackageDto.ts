import type { Bundle } from 'fhir/r4'

export type CreateSynapsePackageDto = {
    fhirServerUrl?: string;
    description: string;
    workspaceId: string;
    patientId: string;
    expiresAt?: string;
    bundle?: Bundle;
    downloadReturnUrl: string;
    scheme: string;
    synapseOrganizationId?: string;
}
