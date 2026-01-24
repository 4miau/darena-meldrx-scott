import type LinkedFhirApiDto from './DynamicRegistration/LinkedFhirApiDto';
import type { FhirServerValidationOption } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/FhirServerValidationOption'

export interface CreateWorkspaceAndNewOrgCommand {
    linkedFhirApiDto?: LinkedFhirApiDto;
    organizationName: string;
    workspaceName: string;
    slug: string;
    description: string;
    validationOption: FhirServerValidationOption;
    organizationIdentifier: string;
    firstName: string | null;
    lastName: string | null;
    email: string | null;
    password: string | null;
    confirmPassword: string | null;
    isLiteWorkspace: boolean;
}
