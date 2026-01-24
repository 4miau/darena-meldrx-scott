<script setup lang="ts">
import { Colors } from "~/types/ui/colors";
import type {ApiFilter} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter";
import type {PagedResult} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult";
import type {WorkspaceEhrAppDto} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceEhrAppDto";


const { $api } = useNuxtApp()

const route = useRoute()
const state = reactive<{
  workspaceEhrApps?: PagedResult<WorkspaceEhrAppDto>;
  isLoading: boolean;
}>({
    workspaceEhrApps: undefined,
    isLoading: false,
})
const workspaceSlug = ref<string>(route.params.workspaceSlug as string)
const patientId = ref<string>(route.params.patientId as string)
async function loadWorkspaceEhrApps( filter: ApiFilter) {
    state.isLoading = true;
    try {
        state.workspaceEhrApps = await $api.ehr.listEhrLaunchApps(workspaceSlug.value, filter);
    } catch (error) {
        handleApiError(error, 'Unable to load ehr apps');
    } finally {
        state.isLoading = false;
    }
}

async function launchApp(app: WorkspaceEhrAppDto) {
    try{
        const response = await $api.ehr.createContext(workspaceSlug.value, {
            patientId: patientId.value,
            workspaceSlug: workspaceSlug.value
        })
        const launchUrl = `${app.launchUrl}?iss=${response.issuerUrl}&launch=${response.launchContext}&client=${app.appId}`
        window.open(launchUrl, '_blank');
    }
    catch (error){
        handleApiError(error, 'Unable to launch app');
    }
}

loadWorkspaceEhrApps( {
    page:1,
    filter:''
});

</script>

<template>
    <div v-if="state.workspaceEhrApps?.resources.length! > 0" class="flex flex-col items-start bg-bliss border border-silver rounded-sm h-full p-2">
      <div class="space-x-2 px-3.5">
        <DsText>
          EHR App Launcher
        </DsText>
      </div>
      <div class="flex flex-row space-x-2 px-3.5 py-1.5">
        <div v-for="app in state.workspaceEhrApps?.resources" :key="app.id">
          <DsButton variant="outline" :color="Colors.secondary" :text-color='Colors.secondary' size="xs" @click="launchApp(app)">
            {{ app.appName }}
          </DsButton>
        </div>
      </div>
    </div>
</template>
