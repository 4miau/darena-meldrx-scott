<script setup lang="ts">
import {Colors} from '~/types/ui/colors';
import type {ApiFilter} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter';
import type {PagedResult} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult';
import type {
    RegisteredAppWithWorkspaceDto
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/RegisteredAppWithWorkspaceDto';
import {DsiOptionMap} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/DsiOption";

definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    allowLinked: true,
})
useHead({title: 'Workspace Extensions | MeldRx'})

const {$api} = useNuxtApp()

const route = useRoute()
const {permissions} = useAuth()
const workspaceSlug = ref<string>(route.params.workspaceSlug as string)

const headersExtensions = ['Extension Name', 'Status', 'Type', 'DSI Type', 'Actions'];
const state = reactive<{
  displayApps?: PagedResult<RegisteredAppWithWorkspaceDto>;
  apiFilter: ApiFilter,
  isLoading: boolean,
}>({
    apiFilter: {
        page: 1,
        filter: '',
    },
    isLoading: false,
})

async function loadAccessibleApps(filter: ApiFilter) {
    state.isLoading = true;
    try {
        state.displayApps = await $api.ehr.listWorkspaceApps(workspaceSlug.value, filter, true);
    } catch (error) {
        handleApiError(error, 'Unable to get apps for activation');
    } finally {
        state.isLoading = false;
    }
}

async function onManage(appId: string) {
    await navigateTo(`/workspaces/${route.params.workspaceSlug}/workspace-extensions/${appId}`)
}

async function onRegisterExtension() {
    await navigateTo(`/workspaces/${route.params.workspaceSlug}/workspace-extensions/register`)
}

await loadAccessibleApps(state.apiFilter);
</script>

<template>
  <DsContainer>
    <!-- Header Section -->
    <div class="space-y-5">
      <DsText size="2xl" weight="light" class="block">
        Workspace Extensions
      </DsText>

      <DsText size="sm" weight="light" class="block">
        Register and manage extensions specific to this workspace. Workspace extensions can only be activated for this
        workspace and will not appear in the published extension directory.
        Only the workspace administrator can manage the extension details and source attributes.
      </DsText>


      <!-- Search bar -->
      <div class="flex items-center">
        <DsTextInput
            v-model="state.apiFilter.filter"
            class="w-full"
            placeholder="Search extensions"
            @enter-pressed="() => loadAccessibleApps(state.apiFilter)"
        >
          <template #right>
            <DsButton :color="Colors.white" :text-color='Colors.gray' variant="outline" @click="loadAccessibleApps(state.apiFilter)">
              Search
              <DsIcon name='heroicons:arrow-right'/>
            </DsButton>
          </template>
        </DsTextInput>
      </div>

      <DsButton
          :color="Colors.primary"
          variant="filled"
          :disabled="!permissions.developerPermissionsDto?.canCreateApps && !permissions.workspaceUserPermissionDto?.find(x=> x.slug == workspaceSlug)?.canAddExtensions"
          @click="onRegisterExtension"
      >
        <DsIcon name='heroicons:plus'/>
        Register New Extension
      </DsButton>

      <DsTable
          v-if="state.displayApps"
          :headers="headersExtensions"
          :items="state.displayApps.resources"
          :id-selector="item => item.appId">
        <template #default="{ item }">
          <div class="flex flex-col">
            <div class="flex flex-col items-start">
              <DsText size="md" weight="light">
                {{ item.appName }}
              </DsText>
            </div>
            <DsText size="sm" weight="light">
              {{ item.fhirServerId ? 'Activated' : 'Not Activated' }}
            </DsText>
            <DsText size="sm" weight="light">
              {{ item.ehrIntegration }}
            </DsText>
            <DsText size="sm" weight="light">
              {{ DsiOptionMap(item.dsiType) }}
            </DsText>
            <div class="space-x-2">
              <DsButton
                  :id="`Manage-${item.appName.replaceAll(' ', '-').toLowerCase()}-button`"
                  :disabled="!permissions.developerPermissionsDto?.canCreateApps && !permissions.workspaceUserPermissionDto?.find(x=> x.slug == workspaceSlug)?.canAddExtensions"
                  :color="Colors.secondary"
                  :text-color='Colors.secondary'
                  variant="outline"
                  size="sm"
                  @click="onManage(item.appId)"
              >
                Manage
                <DsIcon name='heroicons:arrow-right'/>
              </DsButton>
            </div>
          </div>
        </template>
        <template #footer>
          <DsTablePager
              class="pl-6" :paged-result-info="state.displayApps"
              @go-to-page="loadAccessibleApps({ ...state.apiFilter, page: $event })"/>
        </template>
      </DsTable>
    </div>
  </DsContainer>
</template>
