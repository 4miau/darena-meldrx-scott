import type { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';
import type { CdsAppType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/CdsAppType';

type Organization = {
  id: string;
  name: string;
}

type Subscription = {
  id: string;
  type: string;
}

type Workspace = {
  id: string;
  organizationId: string;
  name: string;
}

type App = {
  id: string;
  clientId: string;
  name: string;
  sofAppUserType: SoFAppUserType;
  cdsAppType: CdsAppType;
}

type User = {
  id: string;
  email: string;
}

export type DeleteOrganizationResourcesMessage = {
  organizations: Organization[];
  organizationsWithMipsReport: Organization[];
  subscriptions: Subscription[];
  workspaces: Workspace[];
  apps: App[];
  users: User[];
  mipsUsers: User[];
}
