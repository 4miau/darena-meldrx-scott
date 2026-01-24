import type { NavigateToOptions } from '#app/composables/router'

const options: NavigateToOptions = {
    external: true
}

const newTab: NavigateToOptions = {
    external: true,
    open: {
        target: '_blank'
    }
}

function ensureStartsWithBaseUrl(path: string) {
    const config = useRuntimeConfig()
    if (path.startsWith(config.app.baseURL)) {
        return path;
    }

    return urls.createUrlPath(config.app.baseURL, path)
}

function signIn(returnUrl?: string) {
    return navigateTo(
        {
            path: location.origin + '/Account/Login',
            query: {
                returnUrl: ensureStartsWithBaseUrl(returnUrl || location.pathname)
            }
        },
        options
    )
}

function signUp() {
    return navigateTo({ path: location.origin + '/OrganizationRequest' }, options);
}

function signOut() {
    return navigateTo({ path: location.origin + '/Account/Logout' }, options);
}

function manageAccount() {
    return navigateTo({ path: location.origin + '/Manage/AccountSettings' }, options);
}

function synapsePackage(packageId: string, token: string, scheme: string) {
    return navigateTo(
        {
            path: location.origin + '/api/synapse/login',
            query: { id: packageId, token, scheme, }
        },
        options
    )
}

function mips() {
    return navigateTo({ path: location.origin + '/mymipsscore', }, options)
}

function home() {
    return navigateTo({ path: location.origin }, options);
}

function changeContext() {
    return navigateTo(
        {
            path: '/',
            query: {
                select: 'true'
            }
        }
    )
}

function downloadFhirDirectoryBundle() {
    return navigateTo(
        { path: location.origin + '/api/directories/fhir/endpoints' },
        newTab
    )
}

export default {
    signIn,
    signUp,
    signOut,
    manageAccount,
    synapsePackage,
    mips,
    home,
    changeContext,
    downloadFhirDirectoryBundle,
}
