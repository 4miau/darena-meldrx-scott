import type SubscriptionDetails from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/SubscriptionDetails'

const UNLIMITED_WORKSPACES = -1

export default function () {
    const subscription = useState<SubscriptionDetails>('subscription-details')

    async function loadSubscription () {
        const { $api } = useNuxtApp()

        try {
            subscription.value = await $api.subscriptions.getSubscriptionDetails()
            return subscription.value !== undefined
        } catch (error) {
            handleApiError(error, 'Could not get subscription details')
            return false
        }
    }

    return {
        subscription: readonly(subscription),
        subscriptionWorkspaceLimitReached: computed(() =>
            !subscription.value
            || (subscription.value.includedWorkspaces !== UNLIMITED_WORKSPACES
                && subscription.value.workspaceCount >= subscription.value.includedWorkspaces)
        ),
        subscriptionProvidersLimitReached: computed(() =>
            !subscription.value
            || (subscription.value.includedProviders !== UNLIMITED_WORKSPACES
                && subscription.value.providersCount >= subscription.value.includedProviders)
        ),
        loadSubscription
    }
}
