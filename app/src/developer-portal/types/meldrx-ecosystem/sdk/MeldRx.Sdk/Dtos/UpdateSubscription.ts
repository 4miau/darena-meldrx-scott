import type {MeldRxSubscription} from "./../Enums/MeldRxSubscription";

export interface UpdateSubscription {
    workspaces?: number;
    subscriptionType?: MeldRxSubscription
}
