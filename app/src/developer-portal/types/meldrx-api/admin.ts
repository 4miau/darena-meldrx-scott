import type { ApiFilter } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter';
import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult';
import type { WorkspaceDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto';

import type SubscriptionDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/SubscriptionDto';
import type { AppDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/App';
import type { OrganizationDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/OrganizationDto';
import type {
    CreateWorkspaceForExistingOrganization,
    WorkspaceOrganizationCommand
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/AdminWorkspaceCommands';
import type { DeleteOrganizationResourcesMessage } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DeleteOrganizationResourcesMessage';
import type {
    AdminAppUpdateCommand
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/AdminAppUpdateCommand";

export interface AdminApi {
  workspaces: AdminWorkspacesApi;
  subscriptions: AdminSubscriptionsApi;
  apps: AdminAppsApi;
  organizations: AdminOrganizationsApi;
}

export interface AdminWorkspacesApi {
  search: (apiFilter: ApiFilter, orgId?: string) => Promise<PagedResult<WorkspaceDto>>;
  delete: (workspaceId: string) => Promise<boolean>;
  createForExistingOrg: (command: CreateWorkspaceForExistingOrganization) => Promise<WorkspaceDto>;
  updateWorkspaceOrg: (command: WorkspaceOrganizationCommand) => Promise<void>;
  updateAnonymousReadAccess: (workspaceId: string, value: boolean) => Promise<void>;
  updateSandboxStatus: (workspaceId: string, value: boolean) => Promise<void>;
  changeLiteStatus: (workspaceId: string, value: boolean ) => Promise<void>;
  migrateDb: (workspaceId: string) => Promise<void>;
}

export interface AdminSubscriptionsApi {
  search: (apiFilter: ApiFilter) => Promise<PagedResult<SubscriptionDto>>;
  update: (id: string, value: SubscriptionDto) => Promise<void>;
}

export interface AdminAppsApi {
  search: (apiFilter: ApiFilter, orgId?: string) => Promise<PagedResult<AppDto>>;
  updateOrganizationId: (appId: string, ogrId: string) => Promise<AppDto>;
  delete: (appId: string, ogrId: string) => Promise<AppDto>;
  appDetails: (appId: string) => Promise<AdminAppUpdateCommand>;
  appDetailsUpdate: (command: AdminAppUpdateCommand) => Promise<void>;
}

export interface AdminOrganizationsApi {
  search: (apiFilter: ApiFilter) => Promise<PagedResult<OrganizationDto>>;
  previewDelete: (id: string) => Promise<DeleteOrganizationResourcesMessage>;
  delete: (id: string) => Promise<void>;
}
