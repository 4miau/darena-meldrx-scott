import type { Guid } from '~/types/common/Guid'
import type {MeldRxSubscription} from "./../Enums/MeldRxSubscription";

export default interface SubscriptionDetails {
    id: Guid;
    organizationName: string;
    status: string;
    subscriptionType: MeldRxSubscription
    includedWorkspaces: number;
    includedApiCalls: number;
    includedProviders: number;
    apiCallsUsed: number;
    includedDataStorage: number;
    dataStorageUsed: number;
    allowOverage: boolean;
    workspaceCount: number;
    providersCount: number;
    allowLiteWorkspaces: boolean;
    allowNpiOverride:boolean;
    bannerLogoImageData?: string;
    bannerBackgroundColorHex?: string;
    allowPopulationTrigger:boolean;
}
