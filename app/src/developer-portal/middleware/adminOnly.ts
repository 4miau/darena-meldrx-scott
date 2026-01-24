import externalNavigation from '~/utils/externalNavigation'

export default defineNuxtRouteMiddleware( async (to) => {
    const { initialized, authenticated, isAdmin } = useAuth()

    if (!initialized() || !authenticated.value) {
        if (import.meta.browser) {
            await externalNavigation.signIn(to.fullPath)
            return
        }

        throw new Error('server side not implemented')
    }

    if(!isAdmin()){
        return navigateTo('/')
    }
})
