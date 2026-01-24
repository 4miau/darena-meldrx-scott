import type { Guid } from '~/types/common/Guid'
import type { FhirServerValidationOption } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/FhirServerValidationOption'

export type UpdateDeveloperWorkspaceDto = {
    workspaceId: Guid,
    workspaceName: string,
    validationOption: FhirServerValidationOption;
    isLiteWorkspace: boolean;
}

export type UpdateDeveloperLinkedWorkspaceDto = {
    workspaceId: Guid,
    workspaceName: string,
    validationOption: FhirServerValidationOption;
    fhirApiProviderMeldRxIdentifier: string,
    baseUrl: string
    launchAppClientId?: string;
    launchAppScopes?: string;
}
