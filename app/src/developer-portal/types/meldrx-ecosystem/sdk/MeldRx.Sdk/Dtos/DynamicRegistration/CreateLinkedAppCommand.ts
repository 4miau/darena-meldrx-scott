import type { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod'
import type { SecretType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/SecretType'

export default interface CreateLinkedAppCommand {
    meldRxClientId: string;
    fhirApiProviderMeldRxIdentifier: string;
    isSharedCredentialType: boolean;
    clientName: string;
    clientId: string;
    soFAppTokenAuthMethod: SoFAppTokenAuthMethod;
    secretType: SecretType;
    clientSecret?: string;
    jwksAlg?: string;
    jwksKid?: string;
    scopes: string;
};
