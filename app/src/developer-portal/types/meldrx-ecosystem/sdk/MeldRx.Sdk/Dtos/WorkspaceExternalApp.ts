import type {SecretType} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/SecretType";
import type {
    SoFAppTokenAuthMethod
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod";
import type {SoFAppUserType} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType";

export interface WorkspaceExternalApp {
    id: string;
    soFAppUserType: SoFAppUserType,
    soFAppTokenAuthMethod: SoFAppTokenAuthMethod,
    clientName: string,
    clientId: string,
    clientSecret: string,
    scopes: string,
    secretType: SecretType,
    jwksAlg: string,
    jwksKid: string,
}
export interface CreateWorkspaceExternalApp {
    soFAppUserType: SoFAppUserType,
    soFAppTokenAuthMethod: SoFAppTokenAuthMethod,
    clientName: string,
    clientId: string,
    clientSecret?: string,
    scopes: string,
    secretType: SecretType,
    jwksAlg?: string,
    jwksKid?: string,
}
export interface UpdateWorkspaceExternalApp {
    id: string;
    soFAppUserType: SoFAppUserType,
    soFAppTokenAuthMethod: SoFAppTokenAuthMethod,
    clientName: string,
    clientId: string,
    clientSecret?: string,
    scopes: string,
    secretType: SecretType,
    jwksAlg?: string,
    jwksKid?: string,
}