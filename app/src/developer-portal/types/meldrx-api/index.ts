import type { AppsApi, LinkedAppsApi } from './apps'
import type { AuthApi } from './auth'
import type { FhirProvidersApi } from './fhir-providers'
import type { LinkedFhirApiActionConfigApi } from './LinkedFhirApiActionConfigApi'
import type { WorkspacesApi } from './workspaces'
import type { SubscriptionsApi } from './subscriptions';
import type { OrganizationsApi } from './organizations';
import type { UsersApi } from './users';
import type { FhirApi } from './fhir'
import type { SynapseApi } from './synapse'
import type { DataImportApi } from './data-import'
import type { InviteApi } from '~/types/meldrx-api/invites';
import type { DirectoriesApi } from '~/types/meldrx-api/directories'
import type { AdminApi } from '~/types/meldrx-api/admin'
import type { DocumentsApi } from './documentsApi'
import type { EhrApi } from '~/types/meldrx-api/ehr'
import type {CdsServicesApi} from "~/types/meldrx-api/cds-services";
import type {AiApi} from "~/types/meldrx-api/ai";
import type {MarketplaceApi} from "~/types/meldrx-api/marketplace";
export interface MeldRxApi {
    auth: AuthApi,
    workspaces: WorkspacesApi,
    directories: DirectoriesApi,
    apps: AppsApi,
    marketplace: MarketplaceApi,
    linkedApps: LinkedAppsApi,
    linkedFhirApiAction: LinkedFhirApiActionConfigApi,
    fhirProviders: FhirProvidersApi,
    subscriptions: SubscriptionsApi,
    organizations: OrganizationsApi,
    users: UsersApi,
    ehr: EhrApi,
    cdsServices: CdsServicesApi,
    fhir: FhirApi,
    synapse: SynapseApi,
    dataImport: DataImportApi,
    invites: InviteApi,
    admin: AdminApi,
    documents: DocumentsApi,
    ai: AiApi
}
