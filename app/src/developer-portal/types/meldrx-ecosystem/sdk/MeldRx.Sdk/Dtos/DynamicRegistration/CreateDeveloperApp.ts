import type { SecretType } from '../../Enums/SecretType';
import type { DynamicAuthMethods } from './DynamicAuthMethods';
import type { SoFAppTokenAuthMethod } from './SoFAppTokenAuthMethod';
import type { SoFAppUserType } from './SoFAppUserType';
import type { Guid } from '~/types/common/Guid';
import type { LinkedWorkspacePatientStrategy } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/LinkedWorkspacePatientStrategy'

export interface LinkedAppForm {
    fhirApiProviderMeldRxIdentifier: string;
    isSharedCredentialType?: boolean;
    clientName: string;
    clientId: string;
    soFAppTokenAuthMethod: SoFAppTokenAuthMethod;
    secretType: SecretType;
    clientSecret?: string;
    scopes: string;
    jwksAlg?: string;
    jwksKid?: string;
}

export interface LinkedFhirApiForm {
    fhirApiProviderMeldRxIdentifier: string;
    baseUrl: string;
    patientStrategy: LinkedWorkspacePatientStrategy;
}

export interface WorkspaceForm {
    name: string;
    linkedApi?: LinkedFhirApiForm;
}

export interface CreateDeveloperAppCommand {
    clientName: string;
    publisherUrl: string;
    soFAppUserType: SoFAppUserType;
    tokenEndpointAuthMethod: DynamicAuthMethods;
    ehrLaunchUrl:string;
    secretType: SecretType;

    jwksUri: string;

    scope: string;
    redirectUris: string[];
    postLogoutRedirectUris: string[];

    linkedApps: LinkedAppForm[];
    workspace?: WorkspaceForm;
}

export interface CreateDeveloperAppCommandResult {
    clientId: string,
    clientSecret?: string,
    workspaceId?: Guid,
    fhirDatabaseDisplayName: string,
    fhirServerEndpoint?: string,
}
