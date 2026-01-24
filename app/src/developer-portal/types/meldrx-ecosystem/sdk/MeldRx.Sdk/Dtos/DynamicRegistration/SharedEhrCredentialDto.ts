import type { SoFAppTokenAuthMethod } from './SoFAppTokenAuthMethod';
import type { SoFAppUserType } from './SoFAppUserType';

export default interface SharedEhrCredentialView {
    chplId: string[];
    soFAppTokenAuthMethod: SoFAppTokenAuthMethod;
    soFAppUserType: SoFAppUserType;
}
