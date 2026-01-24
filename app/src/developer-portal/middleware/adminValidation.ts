export default defineNuxtRouteMiddleware(async () => {
    const isAdmin: boolean = useAuth().isAdmin()

    // Ignore if not an admin
    if (!isAdmin) { return }

    return navigateTo('/administrator/workspaces')
})