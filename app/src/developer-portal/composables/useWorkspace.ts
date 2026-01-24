import type { WorkspaceDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto'

export default function () {
    const { $api } = useNuxtApp()

    const state = useState<{
        workspace?: WorkspaceDto,
        loading: boolean
    }>('workspace', () => ({ workspace: undefined, loading: false }))

    async function loadWorkspace (workspaceSlug: string, force: boolean = false) {
        if (!force && workspaceSlug === state.value.workspace?.fhirDatabaseDisplayName) {
            return true
        }

        try {
            state.value.loading = true
            state.value.workspace = await $api.workspaces.get(workspaceSlug)

            return state.value.workspace !== undefined
        } catch (error) {
            handleApiError(error, 'Unable to load workspace')
            notification({
                title: 'Error',
                description: 'Unable to load workspace',
                displayTime: 3000,
                variant: 'error'
            })

            return false
        } finally {
            state.value.loading = false
        }
    }

    return {
        workspace: computed(() => state.value.workspace),
        loadingWorkspace: computed(() => state.value.loading),
        loadWorkspace,
        refreshWorkspace: () => loadWorkspace(state.value.workspace!.fhirDatabaseDisplayName, true)
    }
}
