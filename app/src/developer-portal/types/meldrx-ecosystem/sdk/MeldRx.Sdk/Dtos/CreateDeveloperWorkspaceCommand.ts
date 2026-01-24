import type { LinkedWorkspacePatientStrategy } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/LinkedWorkspacePatientStrategy'
import type { FhirServerValidationOption } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/FhirServerValidationOption'

export interface LinkedFhirApiForm {
    fhirApiProviderMeldRxIdentifier: string;
    baseUrl: string;
    patientStrategy: LinkedWorkspacePatientStrategy;
}

export interface CreateDeveloperWorkspaceCommand {
    linkedFhirApi?: LinkedFhirApiForm;
    name: string;
    validationOption: FhirServerValidationOption;
}
