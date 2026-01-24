import type { Guid } from '~/types/common/Guid';
import type { EHRs } from '~/types/ehrs';
import type { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod';
import type { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';
import type { SecretType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/SecretType';

/**
 * Interface used when buildng a new linked app
 */
export interface INewLinkedApp {
    soFAppUserType?: SoFAppUserType;
    soFAppTokenAuthMethod?: SoFAppTokenAuthMethod;
    fhirApiProviderMeldRxIdentifier?: string;
    clientName: string;
    clientId: string;
    clientSecret?: string;
    redirecUrls?: string[];
    postLogoutRedirecUris?: string[];
    scopes: string;
    jwksAlg?: string;
    jwksKid?: string;
    secretType?: SecretType;
    ehr: EHRs;
    isSharedCredentialType?: boolean;
}

export interface LinkedApp extends INewLinkedApp {
    id: Guid;
}
