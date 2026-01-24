import type { UpdateDeveloperLinkedWorkspaceDto, UpdateDeveloperWorkspaceDto } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/UpdateDeveloperWorkspaceDto';
import type { CreateWorkspaceAndNewOrgCommand } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CreateWorkspaceAndNewOrgCommand';
import type { CreateUserInWorkspaceCommand } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CreateApplicationUserDto';
import type { OrganizationUserRelationDto } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/OrganizationUserRelationDto';
import type { WorkspaceUserModificationModel } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Organization/OrganizationUserModificationModel';
import type { CreateAppPermissionCommand } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CreateFhirServerGrantDto';
import type { FhirRecordGrantDto, WorkspaceAppPermissionDto } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/FhirRecordGrantDto';
import type { WorkspaceUserPermissionDto } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/UserPermissionDto';
import type { ApplicationUserDto } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApplicationUserDto';
import type { WorkspaceDto, CreateNewWorkspaceWithOrgResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto';
import type { CreateDeveloperWorkspaceCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CreateDeveloperWorkspaceCommand';
import type { LinkedWorkspacePatientStrategy } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/LinkedWorkspacePatientStrategy'
import type { FhirServerSettings } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/FhirServerSettings';
import type { FhirServerValidationOption } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/FhirServerValidationOption'
import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'
import type { WorkspacesProvidersApi } from '~/types/meldrx-api/workspacesProviders'
import type { WorkspacesDirectoriesApi } from '~/types/meldrx-api/workspacesDirectoriesApi'
import type { ApiFilter } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter'
import type {CdsFeedbackDto} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CdsHooks/Feedback";
import type {WorkspaceExtensionsApi} from "~/types/meldrx-api/workspaceExtensions";
import type {WorkspaceExternalAppsApi} from "~/types/meldrx-api/workspaceExternalApps";
import type {WorkspaceSynapseOrganizationApi} from "~/types/meldrx-api/workspaceSynapseOrganizations";
import type {PopulationTriggersApi} from "~/types/meldrx-api/populationTriggers";

export type WorkspaceType = 'linked' | 'standalone';

// This interface is used when constructing a new workspace.
export interface INewWorkspace {
    workspaceType: WorkspaceType;
    validationOption: FhirServerValidationOption;
    patientStrategy: LinkedWorkspacePatientStrategy;
    name: string;
    fhirApiProviderId: string;
    fhirUrl: string;
    workspaceSlug: string;
    isLiteWorkspace: boolean;
    email: string | null;
    firstName: string | null;
    lastName: string | null;
    organizationName: string;
    organizationIdentifier: string;

    appId?: string;
}

export interface WorkspacesApi {
    list: (apiFilter:ApiFilter) => Promise<PagedResult<WorkspaceDto>>;
    get: (workspaceSlug: string) => Promise<WorkspaceDto>;
    create: (command: CreateDeveloperWorkspaceCommand) => Promise<WorkspaceDto>;
    createWithOrganization: (command: CreateWorkspaceAndNewOrgCommand) => Promise<CreateNewWorkspaceWithOrgResult>;
    update: (command: UpdateDeveloperWorkspaceDto) => Promise<WorkspaceDto>;
    updateLinked: (command: UpdateDeveloperLinkedWorkspaceDto) => Promise<WorkspaceDto>;
    delete: (id: string) => Promise<boolean>;
    checkSlug: (workspaceSlug: string) => Promise<boolean>;

    listUsers: (workspaceSlug: string) => Promise<OrganizationUserRelationDto[]>;
    createUsers: (workspaceSlug: string, command: CreateUserInWorkspaceCommand) => Promise<ApplicationUserDto>;
    updateUsers: (workspaceSlug: string, command: WorkspaceUserModificationModel) => Promise<void>;

    listAppsAccess: (workspaceSlug: string) => Promise<WorkspaceAppPermissionDto[]>;
    createAppAccess: (workspaceSlug: string, command: CreateAppPermissionCommand) => Promise<FhirRecordGrantDto>;
    deleteAppAccess: (workspaceSlug: string, permissionId: string) => Promise<void>;
    permissions: (workspaceSlug: string) => Promise<WorkspaceUserPermissionDto>;

    getSettings: (workspaceSlug: string) => Promise<FhirServerSettings>;
    updateSettings: (workspaceSlug: string, settings: FhirServerSettings) => Promise<FhirServerSettings>;

    directories: WorkspacesDirectoriesApi;
    providers: WorkspacesProvidersApi;

    getFeedback:(workspaceId: string,startDate:string,endDate:string,indicator:string, patientId:string,page:number,size:number ) => Promise<PagedResult<CdsFeedbackDto>>;
    getAvailableSampleData: () => Promise<Record<string, string[]>>;
    extensions: WorkspaceExtensionsApi,
    populationTriggers: PopulationTriggersApi,
    externalApps: WorkspaceExternalAppsApi,
    synapseOrganizations: WorkspaceSynapseOrganizationApi,
}
