import type { NitroFetchOptions } from 'nitropack';
import type {
    Bundle,
    Patient,
    Group,
    DocumentReference,
    FhirResource,
} from 'fhir/r4';
import type { FetchError } from 'ofetch';
import type UsageGraphPoint from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/UsageGraphPoint';
import type SubscriptionDetails from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/SubscriptionDetails';
import type { MeldRxApi } from '~/types/meldrx-api';
import type SmartConfigurationResponse from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Fhir/SmartOnFhir/SmartConfigurationResponse';
import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult';
import type {
    WorkspaceDto,
    PersonWorkspaceDto,
    CreateNewWorkspaceWithOrgResult,
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto';
import type { CreateDeveloperAppCommandResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CreateDeveloperApp';
import type LinkedAppDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/LinkedAppDto';
import type FhirApiProviderDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';
import type LinkedFhirApiActionConfigDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/LinkedFhirApiActionConfigDto';
import type LinkedFhirApiDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/LinkedFhirApiDto';
import type SharedEhrCredentialView from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SharedEhrCredentialDto';
import type {
    ApiUsageTimeFilter,
    StorageUsageTimeFilter,
} from '~/types/meldrx-api/subscriptions';
import type { OrganizationUserRelationDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/OrganizationUserRelationDto';
import type { WorkspaceUserPermissionDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/UserPermissionDto';
import type {
    FhirRecordGrantDto,
    WorkspaceAppPermissionDto,
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/FhirRecordGrantDto';
import type { ApplicationUserDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApplicationUserDto';
import type { UpdateSubscription } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/UpdateSubscription';
import type { SynapsePackageDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/SynapsePackageDto';
import type { BackgroundJobType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/BackgroundJobType';
import type { BackgroundJobStatus } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/BackgroundJobStatus';
import type { BaseBackgroundJobDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/BackgroundJobs/BaseBackgroundJobDto';
import type {
    CreateInviteDto,
    SendInviteDto,
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Invites';
import type { FhirServerSettings } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/FhirServerSettings';
import type { DirectoryListingDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DirectoryListingDto';
import type { Guid } from '~/types/common/Guid';
import type { WorkspaceDescriptionView } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDescriptionView';
import type { WorkspaceProviderDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceProviderDto';
import type { DirectoryTableView } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Directories/DirectoryTableView';
import type { DirectoryProviderTableView } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Directories/DirectoryProviderTableView';
import type { ApplicationUserInfo } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApplicationUserInfo';
import type SubscriptionDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/SubscriptionDto';
import type DynamicRegistrationDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/DynamicRegistrationDto';
import type { AppDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/App';
import type { OrganizationDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/OrganizationDto';
import type { DirectorySelectorView } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDirectory/DirectorySelectorView';
import type { UploadDocumentDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/UploadDocumentDto';
import type ResourceType from '~/types/fhir/ResourceType';
import type { WorkspaceEhrAppDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceEhrAppDto';
import type { EhrLaunchContextCreateResponse } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/EhrLaunchContextCreateResponse';
import type { PublishedAppDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/PublishedAppDto';
import type { SourceAttributeGroup } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/SourceAttribute';
import type { DsiOption } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/DsiOption';
import type { CdsEventCommand } from '~/types/cds-hooks/CDSCards';
import type {
    CdsFeedbackDto,
    FeedbackPayload,
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CdsHooks/Feedback';
import type { RegisteredAppWithWorkspaceDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/RegisteredAppWithWorkspaceDto';
import QueryUtils from '~/utils/QueryUtils';
import type { CqlCompilationResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CqlCompilationResult';
import type { CdsHookDetails } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CdsHookDetails';
import type { WorkspaceExtensionForm } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceExtensionForm';
import type {
    CreateWorkspaceForExistingOrganization,
    WorkspaceOrganizationCommand,
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/AdminWorkspaceCommands';
import type { ChaiModelCardGroup } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/ChaiModelCardForm';
import type { DeleteOrganizationResourcesMessage } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DeleteOrganizationResourcesMessage';
import externalNavigation from '~/utils/externalNavigation';
import type { BrandingConfigurationDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/BrandingConfigurationDto';
import type { VirtualWorkspaceCreatedResponse } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Fhir/VirtualWorkspaceCreatedResponse';
import type { WorkspaceExternalApp } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceExternalApp';
import type { WorkspaceSynapseOrganization } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceSynapseOrganization';
import type {
    ChatPayload,
    UpdateGithubModelsSettingsCommand,
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/AiInference';
import type {
    AddWorkspaceModel,
    UpdateWorkspaceModel,
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/AiModelDto';
import type { McpServerDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/McpServerDto';
import type {MarketplaceApiFilter} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter";
import type {
    PublishedAppDetails,
    PublishedAppDetailsForMarketplace
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/PublishedAppDetails";
import type {
    AdminAppUpdateCommand
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/AdminAppUpdateCommand";

const fetchConfig: NitroFetchOptions<string> = {
    baseURL: '',
    headers: {
        'X-Requested-With': 'XMLHttpRequest',
    },
};

export default defineNuxtPlugin(() => {
    function createCatchError(redirectWhenNoAuth: boolean) {
        return async function catchError(error: FetchError) {
            const { isLinked, isLaunch, loadUser, authenticated } = useAuth();

            if (error.statusCode === 403) {
                if (isLinked()) {
                    if (isLaunch()) {
                        throw showError({ statusCode: 440 });
                    }

                    const { workspace } = useWorkspace();
                    if (workspace.value) {
                        throw showError({
                            statusCode: 440,
                            data: {
                                ehrLaunchUrl: workspace.value.ehrLaunchUrl,
                            },
                        });
                    }

                    throw showError({ statusCode: 440 });
                } else {
                    // when forbidden: double-check the user account is still active.
                    await loadUser();

                    if (!authenticated.value) {
                        await externalNavigation.signIn(location.href);
                        return;
                    }

                    throw error;
                }
            } else if (error.statusCode === 404) {
                throw showError({ statusCode: 404 });
            } else if (redirectWhenNoAuth && error.statusCode === 401) {
                externalNavigation.signIn();
                return undefined;
            }

            throw error;
        };
    }

    function get<TResponse>(
        url: string,
        redirectWhenNoAuth: boolean = true
    ): Promise<TResponse> {
        return $fetch<TResponse>(url, fetchConfig).catch(
            createCatchError(redirectWhenNoAuth)
        ) as Promise<TResponse>;
    }

    function postStream<ReadableStream>(
        url: string,
        body?: Record<string, any> | string,
        headers?: HeadersInit
    ): Promise<ReadableStream> {
        return $fetch<ReadableStream>(url, {
            ...fetchConfig,
            method: 'post',
            body,
            headers,
            responseType: 'stream',
        });
    }

    function post<TResponse>(
        url: string,
        body?: Record<string, any> | string,
        headers?: HeadersInit
    ): Promise<TResponse> {
        return $fetch<TResponse>(url, {
            ...fetchConfig,
            method: 'post',
            body,
            headers,
        }).catch(createCatchError(true)) as Promise<TResponse>;
    }

    function put<TResponse>(
        url: string,
        body?: Record<string, any>
    ): Promise<TResponse> {
        return $fetch<TResponse>(url, {
            ...fetchConfig,
            method: 'put',
            body,
        }).catch(createCatchError(true)) as Promise<TResponse>;
    }

    function del<TResponse>(url: string): Promise<TResponse> {
        return $fetch<TResponse>(url, {
            ...fetchConfig,
            method: 'delete',
        }).catch(createCatchError(true)) as Promise<TResponse>;
    }

    const api: MeldRxApi = {
        auth: {
            me: () => get<ApplicationUserInfo>('/Account/Me', false),
        },
        users: {
            people: () => get<PersonWorkspaceDto[]>('/api/users/people'),
            workspaces: async () => {
                const workspaces = await get<WorkspaceDto[]>(
                    '/api/users/workspaces?includeAllLinkedWorkspaces=true'
                );
                return workspaces.sort((a, b) => a.name.localeCompare(b.name));
            },
        },
        workspaces: {
            list: (apiFilter) =>
                get<PagedResult<WorkspaceDto>>(
                    `/api/workspaces${QueryUtils.toQueryParams(
                        apiFilter,
                        true
                    )}`
                ),
            get: (workspaceSlug) =>
                get<WorkspaceDto>(`/api/workspaces/slug/${workspaceSlug}`),
            create: (command) => post<WorkspaceDto>('/api/workspaces', command),
            createWithOrganization: (command) =>
                post<CreateNewWorkspaceWithOrgResult>(
                    '/api/workspaces/organization',
                    command
                ),
            update: (workspace) =>
                put<WorkspaceDto>('/api/workspaces/developer', workspace),
            updateLinked: (workspace) =>
                put<WorkspaceDto>(
                    '/api/workspaces/developer/linked',
                    workspace
                ),
            delete: (id: string) => del(`/api/workspaces/${id}`),
            listUsers: (workspaceSlug) =>
                get<OrganizationUserRelationDto[]>(
                    `/api/workspaces/${workspaceSlug}/users`
                ),
            createUsers: (workspaceSlug, command) =>
                post<ApplicationUserDto>(
                    `/api/workspaces/${workspaceSlug}/users`,
                    command
                ),
            updateUsers: (workspaceSlug, command) =>
                put<void>(`/api/workspaces/${workspaceSlug}/users`, command),
            listAppsAccess: (workspaceSlug) =>
                get<WorkspaceAppPermissionDto[]>(
                    `/api/workspaces/${workspaceSlug}/apps`
                ),
            createAppAccess: (workspaceSlug, command) =>
                post<FhirRecordGrantDto>(
                    `/api/workspaces/${workspaceSlug}/apps`,
                    command
                ),
            deleteAppAccess: (workspaceSlug, permissionId) =>
                del(`/api/workspaces/${workspaceSlug}/apps/${permissionId}`),
            checkSlug: (workspaceSlug) =>
                get<boolean>(`/api/workspaces/checkSlug/${workspaceSlug}`),
            permissions: (workspaceSlug) =>
                get<WorkspaceUserPermissionDto>(
                    `/api/workspaces/${workspaceSlug}/permissions`
                ),
            getFeedback: (
                workspaceSlug: string,
                startDate: string,
                endDate: string,
                indicator?: string,
                patientId?: string,
                page?: number,
                size?: number
            ) =>
                get<PagedResult<CdsFeedbackDto>>(
                    `/api/workspaces/${workspaceSlug}/cds-feedback${QueryUtils.objectToQuery(
                        {
                            startDate,
                            endDate,
                            indicator,
                            patientId,
                            page,
                            size,
                        },
                        true
                    )}`
                ),
            getSettings: (workspaceSlug) =>
                get<FhirServerSettings>(`/api/settings/${workspaceSlug}`),
            updateSettings: (
                workspaceSlug: string,
                settings: FhirServerSettings
            ) =>
                post<FhirServerSettings>(
                    `/api/settings/${workspaceSlug}`,
                    settings
                ),
            getAvailableSampleData: () =>
                get<Record<string, string[]>>(`/api/available-sample-data`),

            directories: {
                list: (workspaceSlug, apiFilter) =>
                    get<PagedResult<DirectoryListingDto>>(
                        `/api/workspaces/${workspaceSlug}/directories${QueryUtils.toQueryParams(
                            apiFilter,
                            true
                        )}`
                    ),
                listAll: (workspaceSlug) =>
                    get<DirectorySelectorView[]>(
                        `/api/workspaces/${workspaceSlug}/directories/all`
                    ),
                create: (workspaceSlug, model) =>
                    post<DirectoryListingDto>(
                        `/api/workspaces/${workspaceSlug}/directories`,
                        model
                    ),
                update: (workspaceSlug, model) =>
                    put<DirectoryListingDto>(
                        `/api/workspaces/${workspaceSlug}/directories`,
                        model
                    ),
                updateStatus: (workspaceSlug, directoryListingId, isActive) =>
                    put<void>(
                        `/api/workspaces/${workspaceSlug}/directories/${directoryListingId}/status?isActive=${isActive}`
                    ),
            },

            providers: {
                list: (workspaceSlug, apiFilter) =>
                    get<PagedResult<WorkspaceProviderDto>>(
                        `/api/workspaces/${workspaceSlug}/providers${QueryUtils.toQueryParams(
                            apiFilter,
                            true
                        )}`
                    ),
                updateStatus: (workspaceSlug, command) =>
                    put<void>(
                        `/api/workspaces/${workspaceSlug}/providers/status`,
                        command
                    ),
                delete: (workspaceSlug, command) =>
                    del<void>(
                        `/api/workspaces/${workspaceSlug}/providers/${command}`
                    ),
                updateInfo: (workspaceSlug, command) =>
                    put<void>(
                        `/api/workspaces/${workspaceSlug}/providers/info`,
                        command
                    ),
                createFromNpis: (workspaceSlug, command) =>
                    post<WorkspaceProviderDto>(
                        `/api/workspaces/${workspaceSlug}/providers/npis`,
                        command
                    ),
                createFromCommand: (workspaceSlug, command) =>
                    post<WorkspaceProviderDto>(
                        `/api/workspaces/${workspaceSlug}/providers`,
                        command
                    ),
            },
            extensions: {
                getWorkspaceExtension: (appId, workspaceSlug) =>
                    get<WorkspaceExtensionForm>(
                        `/api/workspaceExtensions/${appId}/${workspaceSlug}`
                    ),
                createWorkspaceExtension: (workspaceSlug, form) =>
                    post<CreateDeveloperAppCommandResult>(
                        `/api/workspaceExtensions/${workspaceSlug}`,
                        form
                    ),
                updateWorkspaceExtension: (workspaceSlug, appId, form) =>
                    put<WorkspaceExtensionForm>(
                        `/api/workspaceExtensions/${workspaceSlug}/${appId}`,
                        form
                    ),
                deleteWorkspaceExtension: (workspaceSlug, appId) =>
                    del(`/api/workspaceExtensions/${workspaceSlug}/${appId}`),
            },
            populationTriggers: {
                getPopulationTriggers: (workspaceSlug) =>
                    get(`/api/population-triggers/${workspaceSlug}`),
                createPopulationTrigger: (workspaceSlug, form) =>
                    post(`/api/population-triggers/${workspaceSlug}`, form),
                updatePopulationTrigger: (
                    workspaceSlug,
                    form,
                    populationTriggerId
                ) =>
                    put(
                        `/api/population-triggers/${workspaceSlug}/${populationTriggerId}`,
                        form
                    ),
                deletePopulationTrigger: (workspaceSlug, populationTriggerId) =>
                    del(
                        `/api/population-triggers/${workspaceSlug}/${populationTriggerId}`
                    ),
                runPopulationTrigger: (workspaceSlug, populationTriggerId) =>
                    get(
                        `/api/population-triggers/${workspaceSlug}/${populationTriggerId}`
                    ),
                getPopulationTriggerReports: (
                    workspaceSlug,
                    populationTriggerId
                ) =>
                    get(
                        `/api/population-triggers/${workspaceSlug}/reports/${populationTriggerId}`
                    ),
                getPopulationTriggerReportData: (workspaceSlug, reportId) =>
                    get(
                        `/api/population-triggers/${workspaceSlug}/reportData/${reportId}`
                    ),
            },
            externalApps: {
                getWorkspaceExternalApps: (workspaceSlug) =>
                    get<WorkspaceExternalApp[]>(
                        `/api/workspaceExternalApps/${workspaceSlug}`
                    ),
                createWorkspaceExternalApp: (workspaceSlug, externalApp) =>
                    post<WorkspaceExternalApp>(
                        `/api/workspaceExternalApps/${workspaceSlug}`,
                        externalApp
                    ),
                updateWorkspaceExternalApp: (workspaceSlug, externalApp) =>
                    put<WorkspaceExternalApp>(
                        `/api/workspaceExternalApps/${workspaceSlug}`,
                        externalApp
                    ),
                deleteWorkspaceExternalApp: (workspaceSlug, externalAppId) =>
                    del(
                        `/api/workspaceExternalApps/${workspaceSlug}/${externalAppId}`
                    ),
            },
            synapseOrganizations: {
                getWorkspaceSynapseOrganizations: (workspaceSlug, apiFilter) =>
                    get<PagedResult<WorkspaceSynapseOrganization>>(
                        `/api/workspaceSynapseOrganizations/${workspaceSlug}${QueryUtils.toQueryParams(
                            apiFilter,
                            true
                        )}`
                    ),
                createWorkspaceSynapseOrganization: (
                    workspaceSlug,
                    synapseOrganization
                ) =>
                    post<WorkspaceSynapseOrganization>(
                        `/api/workspaceSynapseOrganizations/${workspaceSlug}`,
                        synapseOrganization
                    ),
                updateWorkspaceSynapseOrganization: (
                    workspaceSlug,
                    synapseOrganization
                ) =>
                    put<WorkspaceSynapseOrganization>(
                        `/api/workspaceSynapseOrganizations/${workspaceSlug}`,
                        synapseOrganization
                    ),
                deleteWorkspaceSynapseOrganization: (
                    workspaceSlug,
                    synapseOrganizationId
                ) =>
                    del(
                        `/api/workspaceSynapseOrganizations/${workspaceSlug}/${synapseOrganizationId}`
                    ),
            },
        },
        admin: {
            workspaces: {
                search: (apiFilter, orgId?) =>
                    get<PagedResult<WorkspaceDto>>(
                        `/api/admin/workspaces${QueryUtils.toQueryParams(
                            apiFilter,
                            true
                        )}&orgId=${orgId ? orgId : ''}`
                    ),
                delete: (workspaceId: string) =>
                    del(`/api/admin/workspaces/${workspaceId}`),
                createForExistingOrg: (
                    command: CreateWorkspaceForExistingOrganization
                ) =>
                    post<WorkspaceDto>(
                        `/api/admin/workspaces/organization/${command.organizationId}`,
                        command
                    ),
                updateWorkspaceOrg: (command: WorkspaceOrganizationCommand) =>
                    put<void>(`/api/workspaces/organizations`, command),
                updateAnonymousReadAccess: (
                    workspaceId: string,
                    value: boolean
                ) =>
                    post<void>(
                        `/api/admin/workspaces/${workspaceId}/anonymous/status?mode=${value}`
                    ),
                updateSandboxStatus: (workspaceId: string, value: boolean) =>
                    post<void>(
                        `/api/admin/workspaces/${workspaceId}/sandbox/status?mode=${value}`
                    ),
                changeLiteStatus: (workspaceId: string, value: boolean) =>
                    post<void>(
                        `/api/admin/workspaces/${workspaceId}/lite/status?mode=${value}`
                    ),
                migrateDb: (workspaceId: string) =>
                    put<void>(`/api/admin/workspaces/${workspaceId}/migrate`),
            },
            apps: {
                search: (apiFilter, orgId?) =>
                    get<PagedResult<AppDto>>(
                        `/api/admin/apps${QueryUtils.toQueryParams(
                            apiFilter,
                            true
                        )}&orgId=${orgId ? orgId : ''}`
                    ),
                updateOrganizationId: (appId: string, orgId: string) =>
                    post<AppDto>(`/api/admin/apps/${appId}?orgId=${orgId}`),
                delete: (appId: string, orgId: string) =>
                    del<AppDto>(`/api/admin/apps/${appId}?orgId=${orgId}`),
                appDetails: (appId: string) => get(`/api/admin/apps/app-details/${appId}`),
                appDetailsUpdate: (command: AdminAppUpdateCommand) => post(`/api/admin/apps/app-details`, command),
            },
            subscriptions: {
                search: (apiFilter) =>
                    get<PagedResult<SubscriptionDto>>(
                        `/api/admin/subscriptions${QueryUtils.toQueryParams(
                            apiFilter,
                            true
                        )}`
                    ),
                update: (id: string, command: SubscriptionDto) =>
                    post<void>(`/api/admin/subscriptions/${id}`, command),
            },
            organizations: {
                search: (apiFilter) =>
                    get<PagedResult<OrganizationDto>>(
                        `/api/admin/organizations${QueryUtils.toQueryParams(
                            apiFilter,
                            true
                        )}`
                    ),
                previewDelete: (id) =>
                    get<DeleteOrganizationResourcesMessage>(
                        `/api/admin/organizations/${id}/delete`
                    ),
                delete: (id) => del<void>(`/api/admin/organizations/${id}`),
            },
        },
        directories: {
            list: (apiFilter) =>
                get<PagedResult<DirectoryTableView>>(
                    `/api/directories/view${QueryUtils.toQueryParams(
                        apiFilter,
                        true
                    )}`
                ),
            get: (id) =>
                get<DirectoryListingDto>(`/api/directories/view/${id}`),
            listProviders: (id, apiFilter) =>
                get<PagedResult<DirectoryProviderTableView>>(
                    `/api/directories/view/${id}/providers${QueryUtils.toQueryParams(
                        apiFilter,
                        true
                    )}`
                ),
        },
        ehr: {
            list: (workspaceSlug, apiFilter) =>
                get<PagedResult<WorkspaceEhrAppDto>>(
                    `/api/${workspaceSlug}/ehr/apps${QueryUtils.toQueryParams(
                        apiFilter,
                        true
                    )}`
                ),
            listAvailableApps: (workspaceSlug) =>
                get<AppDto[]>(`/api/${workspaceSlug}/ehr/available-apps`),
            listOrgAccessibleApps: (workspaceSlug, apiFilter) =>
                get<PagedResult<RegisteredAppWithWorkspaceDto>>(
                    `/api/${workspaceSlug}/ehr/org-access-apps${QueryUtils.toQueryParams(
                        apiFilter,
                        true
                    )}`
                ),
            listOrgOnlyApps: (workspaceSlug, apiFilter) =>
                get<PagedResult<RegisteredAppWithWorkspaceDto>>(
                    `/api/${workspaceSlug}/ehr/org-only-apps${QueryUtils.toQueryParams(
                        apiFilter,
                        true
                    )}`
                ),
            listWorkspaceApps: (workspaceSlug, apiFilter, activeOnly?) =>
                get<PagedResult<RegisteredAppWithWorkspaceDto>>(
                    `/api/${workspaceSlug}/ehr/workspace-apps${QueryUtils.toQueryParams(
                        apiFilter,
                        true
                    )}&activeOnly=${activeOnly}`
                ),
            getDsiSourceAttributes: (workspaceSlug, ehrAppId) =>
                get<SourceAttributeGroup[]>(
                    `/api/${workspaceSlug}/ehr/apps/${ehrAppId}/dsi-source-attributes`
                ),
            putDsiSourceAttributes: (workspaceSlug, ehrAppId, command) =>
                put(
                    `/api/${workspaceSlug}/ehr/apps/${ehrAppId}/dsi-source-attributes`,
                    command
                ),
            listEhrLaunchApps: (workspaceSlug, apiFilter) =>
                get<PagedResult<WorkspaceEhrAppDto>>(
                    `/api/${workspaceSlug}/ehr/launch-apps${QueryUtils.toQueryParams(
                        apiFilter,
                        true
                    )}`
                ),
            create: (workspaceSlug, command) =>
                post<WorkspaceEhrAppDto>(
                    `/api/${workspaceSlug}/ehr/apps`,
                    command
                ),
            delete: (workspaceSlug, ehrAppId) =>
                del(`/api/${workspaceSlug}/ehr/apps/${ehrAppId}`),
            createContext: (workspaceSlug, command) =>
                post<EhrLaunchContextCreateResponse>(
                    `/api/${workspaceSlug}/ehr/launch-context`,
                    command
                ),
        },
        apps: {
            list: () => get<DynamicRegistrationDto[]>('/api/apps'),
            get: (appId) => get<DynamicRegistrationDto>(`/api/apps/${appId}`),
            listSharedCredentials: () =>
                get<SharedEhrCredentialView[]>('/api/apps/shared-credentials'),
            create: (command) =>
                post<CreateDeveloperAppCommandResult>(
                    '/api/apps/batch',
                    command
                ),
            getCdsHookDetails: (appId) =>
                get<CdsHookDetails>(`/api/apps/cds-hook/${appId}`),
            createCdsHookApp: (command) =>
                post<CreateDeveloperAppCommandResult>(
                    '/api/apps/cds-hook',
                    command
                ),
            updateCdsHookApp: (command) =>
                put<void>('/api/apps/cds-hook', command),
            compileCql: (artifact) =>
                post<CqlCompilationResult[]>('/api/apps/cql-compile', artifact),
            update: (command) => put('/api/apps', command),
            delete: (appId) => del(`/api/apps/${appId}`),
            createSecret: (appId) => post<string>(`/api/apps/${appId}/secrets`),
            deleteSecret: (appId, secretId) =>
                del<string>(`/api/apps/${appId}/secrets/${secretId}`),
            getPublishedAppForEdit: (appId) =>
                get<PublishedAppDto>(`/api/apps/${appId}/publish`),
            updatePublishedApp: (appId: string, command: PublishedAppDto) =>
                put<PublishedAppDto>(`/api/apps/${appId}/publish`, command),
            getDsiSourceAttributes: (dsiOption: DsiOption) =>
                get<SourceAttributeGroup[]>(
                    `/api/apps/dsi-source-attributes/${dsiOption}`
                ),
            getChaiModelCard: () =>
                get<ChaiModelCardGroup[]>(`/api/apps/chai-model-card`),
        },
        marketplace:{
            getPublishedApps: (apiFilter: MarketplaceApiFilter) => get<PagedResult<PublishedAppDetailsForMarketplace>>(`/api/marketplace/published-apps${QueryUtils.marketplaceApiFilterToQueryParams(
                apiFilter,
                true
            )}`),
            getPublishedAppDetails: (appId: string) => get<PublishedAppDetails>(`/api/marketplace/published-app/${appId}`),
            getWorkspacesForActivation: (appId: string) => get<WorkspaceDto[]>(`/api/marketplace/list-workspaces/${appId}?includeAllLinkedWorkspaces=true`),
        },
        linkedApps: {
            list: (appId) =>
                get<PagedResult<LinkedAppDto>>(
                    `/api/apps/${appId}/linkedApp`
                ).then((r) => r.resources),
            create: (command) =>
                post<LinkedAppDto>(
                    `/api/apps/${command.meldRxClientId}/linkedApp`,
                    command
                ),
            update: (command) =>
                post<LinkedAppDto>(
                    `/api/apps/${command.meldRxClientId}/linkedapp/${command.id}`,
                    command
                ),
            delete: (linkedAppId) => del(`/api/apps/linkedapp/${linkedAppId}`),
        },
        linkedFhirApiAction: {
            list: (workspaceSlug) =>
                get<LinkedFhirApiActionConfigDto[]>(
                    `/api/workspaces/${workspaceSlug}/rules`
                ),
            upsert: (workspaceSlug, command) =>
                post<LinkedFhirApiDto>(
                    `/api/workspaces/${workspaceSlug}/rules`,
                    command
                ),
            bulkUpsert: (workspaceSlug, command) =>
                post<LinkedFhirApiDto>(
                    `/api/workspaces/${workspaceSlug}/rules/bulk`,
                    command
                ),
        },
        fhirProviders: {
            list: () => get<FhirApiProviderDto[]>('/api/fhirapiprovider'),
            get: (id) => get<FhirApiProviderDto>(`/api/fhirapiprovider/${id}`),
            validate: (url: string) =>
                get<SmartConfigurationResponse>(
                    `/api/fhirapiprovider/validate?externalFhirUrl=${url}`
                ),
        },
        organizations: {
            listUsers: (organizationId) =>
                get<OrganizationUserRelationDto[]>(
                    `/api/organizations/${organizationId}/users`
                ),
            createUsers: (organizationId, command) =>
                post<OrganizationUserRelationDto[]>(
                    `/api/organizations/${organizationId}/users`,
                    command
                ),
            updateUsers: (organizationId, command) =>
                put<OrganizationUserRelationDto[]>(
                    `/api/organizations/${organizationId}/users`,
                    command
                ),
        },
        subscriptions: {
            getSubscriptionDetails: () => {
                return get<SubscriptionDetails>('/api/subscriptions');
            },

            getApiUsage: (
                workspaceSlug: string | 'all',
                _filter: ApiUsageTimeFilter
            ) => {
                const endpoint =
                    workspaceSlug === 'all'
                        ? `/api/subscriptions/api-usage?filter=${_filter}`
                        : `/api/subscriptions/${workspaceSlug}/api-usage?filter=${_filter}`;
                return get<UsageGraphPoint[]>(endpoint);
            },

            getStorageUsage: (
                workspaceSlug: string | 'all',
                _filter: StorageUsageTimeFilter
            ) => {
                const endpoint =
                    workspaceSlug === 'all'
                        ? `/api/subscriptions/storage-usage?filter=${_filter}`
                        : `/api/subscriptions/${workspaceSlug}/storage-usage?filter=${_filter}`;
                return get<UsageGraphPoint[]>(endpoint);
            },
            checkout: () => post('/api/subscriptions/checkout'),
            getStripeCustomerPortal: () =>
                post('/api/subscriptions/CustomerPortal'),
            updateSubscription: (command: UpdateSubscription) =>
                put('/api/subscriptions', command),
            workspaces: (subscriptionId: Guid) =>
                get<WorkspaceDescriptionView[]>(
                    `/api/subscriptions/${subscriptionId}/workspaces`
                ),
            saveCustomization: (form: FormData) =>
                post<boolean>(`/api/subscriptions/branding`, form),
            deleteCustomization: () =>
                del<boolean>(`/api/subscriptions/branding`),
            getCustomization: async (storageUrl, subscriptionId: Guid) => {
                const response = await fetch(
                    `${storageUrl}/branding/${subscriptionId}/customization_config.json`
                );
                return (await response.json()) as BrandingConfigurationDto;
            },
        },
        fhir: {
            uploadBundle: (workspaceSlug: string, bundle: Bundle) =>
                post<void>(`/api/fhir/${workspaceSlug}`, bundle),

            search: <T>(
                workspaceSlug: string,
                resourceType: ResourceType,
                query: { [key: string]: string }
            ) =>
                get<Bundle<T>>(
                    `/api/fhir/${workspaceSlug}/${resourceType}${QueryUtils.objectToQuery(
                        query,
                        true
                    )}`
                ),

            searchPatients: (workspaceSlug: string) =>
                get<Bundle<Patient>>(`/api/fhir/${workspaceSlug}/Patient`),
            searchPatientsByName: (
                workspaceSlug: string,
                patientName: string
            ) =>
                get<Bundle<Patient>>(
                    `/api/fhir/${workspaceSlug}/Patient?name=${patientName}`
                ),
            searchPatientsByIds: (
                workspaceSlug: string,
                patientIds: string[]
            ) =>
                get<Bundle<Patient>>(
                    `/api/fhir/${workspaceSlug}/Patient?_id=${patientIds.join(
                        ','
                    )}`
                ),
            searchPatientsByPage: (
                workspaceSlug: string,
                page: number,
                token: string
            ) =>
                get<Bundle<Patient>>(
                    `/api/fhir/${workspaceSlug}/bundle/page/${page}?token=${token}`
                ),
            deletePatient: (workspaceSlug: string, patientId: string) =>
                del<void>(`/api/fhir/${workspaceSlug}/Patient/${patientId}`),
            createPatient: (workspaceSlug: string, patient: Patient) =>
                post<Patient>(`/api/fhir/${workspaceSlug}/Patient`, patient),
            updatePatient: (workspaceSlug: string, patient: Patient) =>
                put<Patient>(
                    `/api/fhir/${workspaceSlug}/Patient/${patient.id ?? ''}`,
                    patient
                ),

            getGroups: (workspaceSlug: string) =>
                get<Bundle<Group>>(`/api/fhir/${workspaceSlug}/Group`),
            getGroupById: (workspaceSlug: string, groupId: string) =>
                get<Group>(`/api/fhir/${workspaceSlug}/Group/${groupId}`),
            createGroup: (workspaceSlug: string, group: Group) =>
                post<void>(`/api/fhir/${workspaceSlug}/Group`, group),
            updateGroup: (
                workspaceSlug: string,
                groupId: string,
                group: Group
            ) =>
                put<void>(`/api/fhir/${workspaceSlug}/Group/${groupId}`, group),
            deleteGroup: (workspaceSlug: string, groupId: string) =>
                del<void>(`/api/fhir/${workspaceSlug}/Group/${groupId}`),

            getResourceById: <T>(
                workspaceSlug: string,
                resourceType: string,
                resourceId: string
            ) =>
                get<T>(
                    `/api/fhir/${workspaceSlug}/${resourceType}/${resourceId}`
                ),
            getResourcesByPatientId: <T>(
                workspaceSlug: string,
                resourceType: string,
                patientId: string
            ) =>
                get<Bundle<T>>(
                    `/api/fhir/${workspaceSlug}/${resourceType}?patient=${patientId}`
                ),
            getEverythingByPatientId: (
                workspaceSlug: string,
                patientId: string
            ) =>
                get<Bundle<FhirResource>>(
                    `/api/fhir/${workspaceSlug}/Patient/${patientId}/$Everything`
                ),
            deleteDocument: (workspaceSlug: string, documentId: string) =>
                del<void>(
                    `/api/fhir/${workspaceSlug}/DocumentReference/${documentId}`
                ),

            getLink: <T>(link: string) => get<Bundle<T>>(link),
            getBinaryDocument: (
                workspaceSlug: string,
                id: string,
                contentType: string
            ) =>
                $fetch<Blob>(`/api/fhir/${workspaceSlug}/Binary/${id}`, {
                    baseURL: '',
                    headers: {
                        Accept: contentType,
                        'X-Requested-With': 'XMLHttpRequest',
                    },
                    responseType: 'blob',
                }).catch(createCatchError(true)) as Promise<Blob>,

            operations: {
                virtualWorkspace: (workspaceSlug, payload) =>
                    post<VirtualWorkspaceCreatedResponse>(
                        `/api/fhir/${workspaceSlug}/$virtual-workspace`,
                        payload
                    ),
            },
        },
        synapse: {
            getPackages: (workspaceSlug) =>
                get<SynapsePackageDto[]>(
                    `/api/synapse/${workspaceSlug}/received`
                ),
            create: (pkg) => post<SynapsePackageDto>('/api/synapse', pkg),
            deletePackage: (workspaceSlug, packageId) =>
                del<void>(`/api/synapse/${workspaceSlug}/${packageId}`),
            importPackage: (workspaceSlug, downloadSynapsePackageModel) =>
                post<void>(
                    `/api/synapse/${workspaceSlug}/importBundle`,
                    downloadSynapsePackageModel
                ),
        },
        dataImport: {
            importCcda: (workspaceSlug: string, fileContents: string) =>
                post<void>(
                    `/api/import/${workspaceSlug}/ccdaxml`,
                    fileContents,
                    {
                        'content-type': 'application/xml',
                    }
                ),
            listJobs: (
                workspaceSlug: string,
                jobType: BackgroundJobType,
                jobStatus: BackgroundJobStatus,
                startDate: string,
                endDate: string,
                page?: number | null | undefined,
                size?: number | null | undefined
            ) => {
                let url = `/api/backgroundjobs/${workspaceSlug}?jobType=${jobType}`;
                if (startDate) {
                    url += `&startDate=${startDate}`;
                }
                if (endDate) {
                    url += `&endDate=${endDate}`;
                }
                if (jobStatus) {
                    url += `&status=${jobStatus}`;
                }
                if (page) {
                    url += `&page=${page}`;
                }
                if (size) {
                    url += `&size=${size}`;
                }
                return get<PagedResult<BaseBackgroundJobDto>>(url);
            },
        },
        invites: {
            create: (workspaceSlug: string, model: CreateInviteDto) =>
                post(`/api/${workspaceSlug}/invites/create`, model),
            send: (
                workspaceSlug: string,
                id: string,
                sendInviteDto: SendInviteDto
            ) =>
                post(`/api/${workspaceSlug}/invites/${id}/send`, sendInviteDto),
            delete: (workspaceSlug: string, id: string) =>
                del<void>(`/api/${workspaceSlug}/invites/${id}`),
            findSent: (workspaceSlug: string) =>
                get(`/api/${workspaceSlug}/invites/sent`),
            search: (workspaceSlug: string, patientIds: string[]) =>
                get(
                    `/api/${workspaceSlug}/invites/search?patientIdsAsString=${patientIds.join(
                        ','
                    )}`
                ),
        },
        cdsServices: {
            getCards: (workspaceSlug: string, command: CdsEventCommand) =>
                post(`/api/${workspaceSlug}/cds-services`, command),
            postFeedback: (
                workspaceSlug: string,
                serviceId: string,
                feedbackPayload: FeedbackPayload
            ) =>
                post(
                    `/api/${workspaceSlug}/cds-services/${serviceId}/feedback`,
                    feedbackPayload
                ),
        },
        documents: {
            uploadDocument: (
                workspaceSlug: string,
                uploadmodel: UploadDocumentDto,
                fileContent: ArrayBuffer,
                fileName: string
            ) => {
                const url = `/api/documents/${workspaceSlug}`;
                const formData = new FormData();
                formData.append(
                    'fileContent',
                    new Blob([fileContent]),
                    fileName
                );
                formData.append('Date', uploadmodel.date);
                formData.append('PatientId', uploadmodel.patientId!);
                formData.append('Description', uploadmodel.description!);
                return post<DocumentReference>(url, formData);
            },
        },
        ai: {
            updateGithubSettings: (
                workspaceSlug: string,
                command: UpdateGithubModelsSettingsCommand
            ) => post(`/api/${workspaceSlug}/ai/github-settings`, command),
            getGitHubToken: (workspaceSlug: string) =>
                get(`/api/${workspaceSlug}/ai/github-token`),
            getModels: (workspaceSlug: string) =>
                get(`/api/${workspaceSlug}/ai/models`),
            inference: (workspaceSlug: string, form: ChatPayload) =>
                postStream(`/api/${workspaceSlug}/ai/stream`, form),
            getWorkspaceModels: (workspaceSlug: string) =>
                get(`/api/${workspaceSlug}/ai-models`),
            addWorkspaceModel: (
                workspaceSlug: string,
                form: AddWorkspaceModel
            ) => post(`/api/${workspaceSlug}/ai-models`, form),
            updateWorkspaceModel: (
                workspaceSlug: string,
                form: UpdateWorkspaceModel
            ) => put(`/api/${workspaceSlug}/ai-models`, form),
            deleteWorkspaceModel: (workspaceSlug: string, modelId: string) =>
                del(`/api/${workspaceSlug}/ai-models/${modelId}`),
            listMcpServers: (workspaceSlug: string) =>
                get(`/api/${workspaceSlug}/ai/mcp-servers/list`),
            createMcpServer: (workspaceSlug: string, mcpServer: McpServerDto) =>
                post(`/api/${workspaceSlug}/ai/mcp-servers`, mcpServer),
            updateMcpServer: (workspaceSlug: string, mcpServer: McpServerDto) =>
                post(
                    `/api/${workspaceSlug}/ai/mcp-servers/${mcpServer.id}`,
                    mcpServer
                ),
            deleteMcpServer: (workspaceSlug: string, id: string) =>
                del(`/api/${workspaceSlug}/ai/mcp-servers/${id}`),
            testMcpServer: (workspaceSlug: string, mcpServer: McpServerDto) =>
                post(`/api/${workspaceSlug}/ai/mcp-servers/test`, mcpServer),
            toggleDefaultMcpTools: (workspaceSlug: string, value: boolean) =>
                post(`/api/${workspaceSlug}/ai/toggle-default-mcp-tools`, {
                    value,
                }),
        },
    };
    return {
        provide: { api },
    };
});
