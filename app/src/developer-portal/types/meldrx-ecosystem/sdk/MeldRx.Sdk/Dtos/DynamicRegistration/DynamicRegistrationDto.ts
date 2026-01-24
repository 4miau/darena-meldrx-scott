import type { ApprovalStatus } from '../../Enums/ApprovalStatus';
import type { SecretType } from '../../Enums/SecretType';
import type ClientSecretView from './ClientSecretView';
import type { DynamicAuthMethods } from './DynamicAuthMethods';
import type { SoFAppUserType } from './SoFAppUserType';
import type { CdsAppType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/CdsAppType'

export default interface DynamicRegistrationDto {
    client_id: string;
    client_secret: ClientSecretView[];
    client_id_issued_at: number;
    client_secret_expires_at: number;
    token_endpoint_auth_method: DynamicAuthMethods;
    grant_types: string[];
    response_types: string[];
    client_name: string;
    redirect_uris: string[];
    post_logout_redirect_uris: string[];
    logo_uri: string[];
    client_uri: string;
    jwks_uri: string;
    scope: string;
    approval_status: ApprovalStatus;
    denied_reason: string;
    soFAppUserType: SoFAppUserType;
    secretType: SecretType;
    ehrLaunchUrl: string;
    cdsAppType: CdsAppType;
    cdsHookServiceUrl:string;
    cqlEditorArtifact:string;
}
