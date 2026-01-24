import type { CreateDeveloperAppCommand, CreateDeveloperAppCommandResult } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CreateDeveloperApp';
import type DynamicRegistrationDto from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/DynamicRegistrationDto';
import type LinkedAppDto from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/LinkedAppDto';
import type SharedEhrCredentialView from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SharedEhrCredentialDto';
import type { UpdateDeveloperAppInformationCommand } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/UpdateDeveloperAppInformationCommand';
import type CreateLinkedAppCommand from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CreateLinkedAppCommand'
import type { CqlCompilationResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CqlCompilationResult';
import type { CdsHookDetails } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CdsHookDetails';
import type {PublishedAppDto} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/PublishedAppDto";
import type {DsiOption} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/DsiOption";
import type {SourceAttributeGroup} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/SourceAttribute";
import type {ChaiModelCardGroup} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/ChaiModelCardForm";

export interface AppsApi {
    list: () => Promise<DynamicRegistrationDto[]>,
    get: (appId: string) => Promise<DynamicRegistrationDto>,
    listSharedCredentials: () => Promise<SharedEhrCredentialView[]>,
    create: (command: CreateDeveloperAppCommand) => Promise<CreateDeveloperAppCommandResult>,
    getCdsHookDetails: (appId: string) => Promise<CdsHookDetails>,
    createCdsHookApp: (form: FormData) => Promise<CreateDeveloperAppCommandResult>,
    updateCdsHookApp: (form: FormData) => Promise<void>,
    compileCql: (artifactPayload: any) => Promise<CqlCompilationResult[]>,
    update: (command: UpdateDeveloperAppInformationCommand) => Promise<void>,
    delete: (appId: string) => Promise<boolean>,
    createSecret: (appId: string) => Promise<string>
    deleteSecret: (appId: string, secretId: number) => Promise<string>
    getPublishedAppForEdit: (appId:string) => Promise<PublishedAppDto>;
    updatePublishedApp: (appId:string, command: PublishedAppDto) => Promise<PublishedAppDto>;
    getDsiSourceAttributes: (dsiOption:DsiOption) => Promise<SourceAttributeGroup[]>;
    getChaiModelCard: () => Promise<ChaiModelCardGroup[]>;
}

export interface LinkedAppsApi {
    list: (appId: string) => Promise<LinkedAppDto[]>,
    create: (command: CreateLinkedAppCommand) => Promise<LinkedAppDto>,
    update: (command: LinkedAppDto) => Promise<LinkedAppDto>,
    delete: (linkedAppId: string) => Promise<boolean>,
}
