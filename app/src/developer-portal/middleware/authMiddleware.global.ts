import type { RouteLocationNormalized } from 'vue-router'
import externalNavigation from '~/utils/externalNavigation'

function allowAnonymous(to: RouteLocationNormalized) {
    return to.path.includes('/component-gallery') ||
        to.meta.anonymous
}

export default defineNuxtRouteMiddleware(async (to) => {
    const { initialized, authenticated, loadUser, permissions, isAdmin } = useAuth()

    if (!initialized()) {
        await loadUser()
    }

    if (!authenticated.value) {
        if (allowAnonymous(to)) {
            return;
        }

        if (import.meta.browser) {
            await externalNavigation.signIn(to.fullPath)
            return
        }

        throw new Error('server side not implemented')
    }

    if (permissions.value.isDeveloper || isAdmin()) {
        const { subscription, loadSubscription } = useSubscription()
        if (!subscription.value) {
            // if we fail to load the subscription, user isn't part of a developer organization
            const subscriptionLoaded = await loadSubscription();
            if (!subscriptionLoaded) {
                await externalNavigation.home()
            }
        }
    }

    const onlyMips = permissions.value.hasMipsReports && !permissions.value.isDeveloper && !permissions.value.hasPeople && !permissions.value.hasWorkspaces;
    if (onlyMips) {
        externalNavigation.mips();
        return;
    }

    const { brandingConfiguration, loadBranding } = useBranding();
    if(!brandingConfiguration.value) {
        await loadBranding();
    }

})
