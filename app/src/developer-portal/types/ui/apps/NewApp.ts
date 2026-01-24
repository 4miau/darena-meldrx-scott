import type { INewLinkedApp } from './NewLinkedApp';
import type { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod';
import type { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';

/**
 * Interface used when buildng a new app
 */
export interface INewApp {
    appName: string;
    appPublisherUrl: string;
    appIcon?: string;
    userType: SoFAppUserType;
    authenticationClientType: SoFAppTokenAuthMethod;
    scopes: string;
    redirectUrls: string[];
    ehrLaunchUrl:string;
    linkedApps: INewLinkedApp[];
}
