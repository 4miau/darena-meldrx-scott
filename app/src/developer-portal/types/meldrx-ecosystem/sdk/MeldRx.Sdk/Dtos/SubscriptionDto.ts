import type {Guid} from "~/types/common/Guid";
import type {MeldRxSubscription} from "./../Enums/MeldRxSubscription";

export default interface SubscriptionDto {
    id: Guid;
    organizationName: string;
    resellerId: Guid;
    status: string;
    subscriptionType: MeldRxSubscription;
    includedWorkspaces: number;
    includedVirtualWorkspaces: number;
    includedApiCalls: number;
    includedProviders: number;
    includedDataStorage: number;
    allowOverage: boolean;
    allowLiteWorkspaces: boolean;
    allowNpiOverride: boolean;
    validateCcdaProvider: boolean;
    autoAddProvider: boolean;
    allowPopulationTrigger: boolean;
}
