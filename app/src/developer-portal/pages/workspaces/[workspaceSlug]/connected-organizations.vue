<script setup lang="ts">

import type {
    CreateWorkspaceSynapseOrganization, UpdateWorkspaceSynapseOrganization,
    WorkspaceSynapseOrganization
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceSynapseOrganization";
import type {PagedResult} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult";
import type {ApiFilter} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter";
import type {WorkspaceExternalApp} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceExternalApp";
import {Colors} from "~/types/ui/colors";

definePageMeta({layout: 'workspace', middleware: ['require-workspace']});
useHead({title: 'Connected Organizations | MeldRx'});

const {$api} = useNuxtApp();
const confirmation = useConfirmation()
const route = useRoute();
const {workspace} = useWorkspace();
const workspaceSlug = ref<string>(route.params.workspaceSlug as string);

const state = reactive<{
  selectedSynapseOrganization?: WorkspaceSynapseOrganization;
  synapseOrganizations?: PagedResult<WorkspaceSynapseOrganization>;
  externalApps?: WorkspaceExternalApp[];
  loading: boolean;
  showSynapseOrganizationModal: boolean;
  apiFilter: ApiFilter;
}>({
    loading: false,
    showSynapseOrganizationModal: false,
    apiFilter: {
        page: 1,
        orderBy: 'name'
    }
})

useQuerySync(() => state.apiFilter)
const synapseTableHeaders = [
    'Organization Name',
    'External App',
    'Actions'
];


async function loadExternalApps(workspaceSlug: string) {
    state.loading = true;
    try {
        state.externalApps = await $api.workspaces.externalApps.getWorkspaceExternalApps(workspaceSlug)
    } catch (error) {
        handleApiError(error)
    } finally {
        state.loading = false;
    }
}

async function loadSynapseOrganizations(workspaceSlug: string, apiFilter: ApiFilter) {
    state.loading = true;
    try {
        state.synapseOrganizations = await $api.workspaces.synapseOrganizations.getWorkspaceSynapseOrganizations(workspaceSlug, apiFilter)
    } catch (error) {
        handleApiError(error)
    } finally {
        state.loading = false;
    }
}

function onAddSynapseOrganization() {
    state.selectedSynapseOrganization = undefined;
    state.showSynapseOrganizationModal = true
}

async function addSynapseOrganization(synapseOrganization: CreateWorkspaceSynapseOrganization) {
    if (!state.synapseOrganizations) {
        return;
    }

    state.loading = true;

    try {
        await $api.workspaces.synapseOrganizations.createWorkspaceSynapseOrganization(workspaceSlug.value, synapseOrganization)
        state.showSynapseOrganizationModal = false;
    } catch (error) {
        handleApiError(error)
    } finally {
        state.loading = false;
    }
    await loadSynapseOrganizations(workspaceSlug.value, state.apiFilter)
}

async function editSynapseOrganization(synapseOrganization: UpdateWorkspaceSynapseOrganization) {
    if (!state.synapseOrganizations) {
        return;
    }

    state.loading = true;

    try {
        await $api.workspaces.synapseOrganizations.updateWorkspaceSynapseOrganization(workspaceSlug.value, synapseOrganization)
        state.showSynapseOrganizationModal = false;
    } catch (error) {
        handleApiError(error)
    } finally {
        state.loading = false;
    }
    await loadSynapseOrganizations(workspaceSlug.value, state.apiFilter)
}

async function deleteSynapseOrganization(id: string) {
    if (!state.synapseOrganizations) {
        return;
    }
    const {isCancelled} = await confirmation(
        'Are you sure you want to delete this Organization? This action cannot be undone.',
        'Delete Organization'
    )
    if (isCancelled) {
        return
    }

    state.loading = true;

    try {
        await $api.workspaces.synapseOrganizations.deleteWorkspaceSynapseOrganization(workspaceSlug.value, id)
    } catch (error) {
        handleApiError(error)
    } finally {
        state.loading = false;
    }
    await loadSynapseOrganizations(workspaceSlug.value, state.apiFilter)
}

function selectSynapseOrganization(id: string) {
    if (state.synapseOrganizations == null) {
        return
    }

    state.selectedSynapseOrganization = state.synapseOrganizations.resources.find(x => x.id === id)
    state.showSynapseOrganizationModal = true
}

loadExternalApps(workspaceSlug.value)
loadSynapseOrganizations(workspaceSlug.value, state.apiFilter)
</script>

<template>
  <DsContainer v-if="workspace">

    <div class="grid grid-cols-12 gap-5">
      <div class="col-span-8 flex flex-col">
        <DsText size="2xl" weight="light">
          Patient Connect - Connected Organizations
        </DsText>
        <DsText size="xs" weight="light">
          {{ workspace.name }}
        </DsText>

        <DsText size="sm" weight="light" class="mt-5">
          Set up connected organizations for which your external apps will be used.<br>
          Existing/default organizations in the patient data request flow will use the MeldRx apps.
        </DsText>
      </div>

      <!-- Connected Organizations (formerly Synapse Organizations) -->
      <div class="col-span-12">
        <DsTextInput
            v-model="state.apiFilter.filter"
            placeholder="Search organizations"
            @enter-pressed="() => loadSynapseOrganizations(workspaceSlug, state.apiFilter)"
        >
          <template #right>
            <DsButton
                :color="Colors.white"
                :text-color='Colors.gray'
                variant="outline"
                @click="() => loadSynapseOrganizations(workspaceSlug, state.apiFilter)"
            >
              Search
              <DsIcon name='heroicons:magnifying-glass'/>
            </DsButton>
          </template>
        </DsTextInput>
      </div>

      <div class="flex col-span-12">
        <DsButton
            :disabled="state.externalApps?.length === 0"
            :color="Colors.primary"
            variant="filled"
            size="md"
            @click="onAddSynapseOrganization"
        >
          <DsIcon name="heroicons:plus" size='sm' />
          Add Connected Organization
        </DsButton>
        <DsBanner v-if="state.externalApps?.length === 0" icon='heroicons:information-circle' :color='Colors.fire' class='pl-5'>
          <DsLink :href='`/workspaces/${workspaceSlug}/patient-connect`' underline='hover'>
            You need an External App to get started.
          </DsLink>
        </DsBanner>
      </div>

      <div class="col-span-12">

        <DsTable
            v-if="state.synapseOrganizations"
            :headers="synapseTableHeaders"
            :items="state.synapseOrganizations.resources"
            :id-selector="item => item.id"
            :api-filter="state.apiFilter"
            @update:api-filter="loadSynapseOrganizations(workspaceSlug, state.apiFilter)"
        >
          <template #default="{item}">

            <div>
              <DsText size="lg" weight="light" class="inline-flex items-center">
                {{ item.organizationName }}
              </DsText>
              <DsTextWithCopyButton
                  size="xs"
                  :text="`Organization Id: ${item.id}`"
                  :text-to-copy="item.id"
                  :show-toast-on-copy="true"
                  toast-message-on-copy="Copied Organization ID to clipboard."
              />
            </div>

            <DsText size="md" weight="light">
              {{ state.externalApps?.find(x => x.id === item.externalAppId)?.clientName }}
            </DsText>

            <div class="flex flex-row space-x-3">
              <DsButton
                  :id="`edit-${item.organizationName.replaceAll(' ', '-').toLowerCase()}-button`"
                  :color="Colors.secondary"
                  :text-color='Colors.secondary'
                  size="sm"
                  variant="outline"
                  @click="selectSynapseOrganization(item.id)"
              >
                Edit
              </DsButton>
              <DsButton
                :id="`delete-${item.organizationName.replaceAll(' ', '-').toLowerCase()}-button`"
                :color="Colors.fire"
                :text-color='Colors.fire'
                size="sm"
                variant="outline"
                @click="deleteSynapseOrganization(item.id)"
              >
                <DsIcon name="heroicons:x-mark" />
                Delete
              </DsButton>
            </div>

          </template>
          <template #footer>
            <DsTablePager
                class="pl-6"
                :paged-result-info="state.synapseOrganizations"
                @go-to-page="(page) => loadSynapseOrganizations(workspaceSlug,{...state.apiFilter, page})"
            />
          </template>
        </DsTable>
      </div>
    </div>

    <SynapseOrganizationModal
        v-if="state.externalApps"
        :show="state.showSynapseOrganizationModal"
        :synapse-organization="state.selectedSynapseOrganization"
        :external-apps="state.externalApps"
        @add-synapse-organization="(v) => addSynapseOrganization(v)"
        @edit-synapse-organization="(v) => editSynapseOrganization(v)"
        @close="state.showSynapseOrganizationModal = false;"
    />
  </DsContainer>
</template>
