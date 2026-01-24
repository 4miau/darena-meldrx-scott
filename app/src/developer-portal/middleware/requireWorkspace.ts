import useWorkspace from '~/composables/useWorkspace'

export default defineNuxtRouteMiddleware(async (to) => {
    if (!to.params.workspaceSlug) {
        return navigateTo('/workspaces')
    }

    const { workspace, loadWorkspace } = useWorkspace()
    const result = await loadWorkspace(to.params.workspaceSlug as string, !!to.meta.refreshWorkspace)

    if (!result) {
        return navigateTo('/workspaces')
    }

    const isLinked = !!workspace.value?.linkedFhirApiDto;
    if(isLinked && !to.meta.allowLinked){
        return navigateTo(`/workspaces/${workspace.value?.fhirDatabaseDisplayName}`)
    }
})
