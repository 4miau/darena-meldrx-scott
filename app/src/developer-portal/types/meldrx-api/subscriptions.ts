import type { UpdateSubscription } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/UpdateSubscription';
import type UsageGraphPoint from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/UsageGraphPoint';
import type SubscriptionDetails from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/SubscriptionDetails';
import type { StripeRedirectResponse } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/StripeRedirectResponse'
import type { WorkspaceDescriptionView } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDescriptionView'
import type { Guid } from '~/types/common/Guid'
import type { BrandingConfigurationDto } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/BrandingConfigurationDto';

export type ApiUsageTimeFilter = 'current-month' | 'year';
export type StorageUsageTimeFilter = 'current-month' | 'year';

export interface SubscriptionsApi {
    getSubscriptionDetails: () => Promise<SubscriptionDetails>;
    getApiUsage: (workspaceId: string | 'all', filter: ApiUsageTimeFilter) => Promise<UsageGraphPoint[]>;
    getStorageUsage: (workspaceId: string | 'all', filter: StorageUsageTimeFilter) => Promise<UsageGraphPoint[]>;
    checkout: () => Promise<StripeRedirectResponse>;
    getStripeCustomerPortal: () => Promise<StripeRedirectResponse>;
    updateSubscription: (updateSubscriptionData: UpdateSubscription) => Promise<StripeRedirectResponse>;
    workspaces: (subscriptionId: Guid) => Promise<WorkspaceDescriptionView[]>;
    saveCustomization(model: FormData): Promise<boolean>;
    deleteCustomization(): Promise<boolean>;
    getCustomization(storageUrl: string, subscriptionId: Guid): Promise<BrandingConfigurationDto>;
}
