import type {PagedResult} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult";
import type {
    PublishedAppDetails,
    PublishedAppDetailsForMarketplace
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/PublishedAppDetails";
import type {WorkspaceDto} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto";
import type {MarketplaceApiFilter} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter";

export interface MarketplaceApi {
    getPublishedApps: (apiFilter: MarketplaceApiFilter) => Promise<PagedResult<PublishedAppDetailsForMarketplace>>,
    getPublishedAppDetails: (appId:string) => Promise<PublishedAppDetails>;
    getWorkspacesForActivation: (appId: string) => Promise<WorkspaceDto[]>;
}