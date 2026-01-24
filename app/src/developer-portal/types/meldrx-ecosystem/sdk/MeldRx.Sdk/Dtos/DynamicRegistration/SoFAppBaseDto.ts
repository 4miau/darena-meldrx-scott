import type { SecretType } from '../../Enums/SecretType';
import type { SoFAppTokenAuthMethod } from './SoFAppTokenAuthMethod';
import type { SoFAppUserType } from './SoFAppUserType';

export default interface SoFAppBaseDto {
    soFAppUserType?: SoFAppUserType;
    soFAppTokenAuthMethod: SoFAppTokenAuthMethod;
    fhirApiProviderMeldRxIdentifier: string;
    clientName: string;
    clientId: string;
    clientSecret?: string;
    scopes: string;
    secretType: SecretType;
    jwksAlg?: string;
    jwksKid?: string;
};
