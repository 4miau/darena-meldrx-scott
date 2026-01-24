<script setup lang="ts">
import type {populationTriggerReportDto} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PopulationTriggerReportDto";
import type {PagedResult} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult";
import type {WorkspaceEhrAppDto} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceEhrAppDto";
import {Colors} from "~/types/ui/colors";
import type {
    CreatePopulationTrigger,
    PopulationTrigger,
    UpdatePopulationTrigger
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PopulationTrigger";

definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    allowLinked: true,
})
useHead({title: 'Population Triggers | MeldRx'})

const {$api} = useNuxtApp()
const route = useRoute()
const {permissions} = useAuth()
const workspaceSlug = ref<string>(route.params.workspaceSlug as string)
const populationTriggersHeaders = ['Extension Name', 'Population', 'Included Indicators', 'Actions'];
const reportsHeaders = ['Report Id', 'Report Date', 'Actions'];
const populationTriggerActions = [
    { value: "runTrigger", title: 'Run Population Trigger' },
    { value: "loadReports", title: 'Load Reports' },
    { value: "update", title: 'Update Population Trigger' },
    { value: "delete", title: 'Delete Population Trigger' },
]

const state = reactive<{
  extensions?: PagedResult<WorkspaceEhrAppDto>;
  populationTriggers: PopulationTrigger[];
  reports?: populationTriggerReportDto[];
  showPopulationTriggerModal: boolean;
  selectedPopulationTrigger?: PopulationTrigger;
  loading: boolean;
}>({
    loading: false,
    populationTriggers: [],
    showPopulationTriggerModal: false
})

async function loadExtensions() {
    state.loading = true;
    try {
        state.extensions = await $api.ehr.list(
            workspaceSlug.value,
            {
                page: 1,
                filter: '',
            }
        );
    } catch (error) {
        handleApiError(error, 'Unable to load extensions')
    } finally {
        state.loading = false;
    }
}

async function loadPopulationTriggers(){
    state.loading = true;
    try {
        state.populationTriggers = await $api.workspaces.populationTriggers.getPopulationTriggers(workspaceSlug.value)
    } catch (error) {
        handleApiError(error, 'Unable to load population triggers')
    } finally {
        state.loading = false;
    }
}

async function loadPopulationTriggerReports(populationTriggerId:string){
    state.loading = true;
    state.reports = undefined
    try {
        state.reports = await $api.workspaces.populationTriggers.getPopulationTriggerReports(workspaceSlug.value, populationTriggerId)
    } catch (error) {
        handleApiError(error, 'Unable to load reports')
    } finally {
        state.loading = false;
    }
}

async function viewReportData(reportId:string){
    navigateTo(
        `/workspaces/${workspaceSlug.value}/population-trigger/${reportId}`,
        {
            external: true,
            open: {
                target: '_blank'
            }
        }
    )
}

function selectPopulationTrigger(extensionId?: string){
    if (!extensionId){
        state.selectedPopulationTrigger = undefined;
        state.showPopulationTriggerModal = true
        return;
    }
    
    state.selectedPopulationTrigger = state.populationTriggers?.find(x => x.id === extensionId)
    state.showPopulationTriggerModal = true

}

async function createPopulationTrigger(populationTrigger: CreatePopulationTrigger)
{
    state.loading = true;

    try {
        await $api.workspaces.populationTriggers.createPopulationTrigger(workspaceSlug.value, populationTrigger)
        state.showPopulationTriggerModal = false;
    }
    catch (error) {
        handleApiError(error)
    } finally {
        state.loading = false;
    }
    await loadPopulationTriggers()
}

async function updatePopulationTrigger(populationTrigger: UpdatePopulationTrigger)
{
    state.loading = true;

    try {
        await $api.workspaces.populationTriggers.updatePopulationTrigger(workspaceSlug.value, populationTrigger, populationTrigger.id)
        state.showPopulationTriggerModal = false;
    }
    catch (error) {
        handleApiError(error)
    } finally {
        state.loading = false;
    }
    await loadPopulationTriggers()
}

async function deletePopulationTrigger(populationTriggerId: string)
{
    state.loading = true;

    try {
        await $api.workspaces.populationTriggers.deletePopulationTrigger(workspaceSlug.value, populationTriggerId)
    }
    catch (error) {
        handleApiError(error)
    } finally {
        state.loading = false;
    }
    await loadPopulationTriggers()
}

async function runPopulationTrigger(populationTriggerId: string)
{
    state.loading = true;

    try {
        await $api.workspaces.populationTriggers.runPopulationTrigger(workspaceSlug.value, populationTriggerId)
    }
    catch (error) {
        handleApiError(error)
        return
    } finally {
        state.loading = false;
    }
    await loadPopulationTriggerReports(populationTriggerId)
}

const onActionSelected = (action: string, populationTrigger: PopulationTrigger) => {
    if (action === "runTrigger") {
        runPopulationTrigger(populationTrigger.id)
    }
    else if (action === "loadReports") {
        state.selectedPopulationTrigger = populationTrigger
        loadPopulationTriggerReports(populationTrigger.id)
    }
    else if (action === "update") {
        selectPopulationTrigger(populationTrigger.id)
    }
    else if (action === "delete") {
        deletePopulationTrigger(populationTrigger.id)
    }
}
loadPopulationTriggers()
loadExtensions();
</script>

<template>
  <DsContainer>
    <div class="space-y-5">
      <DsText size="2xl" weight="light" class="block">
        Population Triggers
      </DsText>

      <DsText size="sm" weight="light" class="block">
        Population Triggers let you run Clinical Decision Support(CDS) on a population level.
        Create and run your workspace extensions on a group of patients or the whole workspace.
      </DsText>

      <DsButton
          v-if="state.populationTriggers.length > 0"
          :color="Colors.primary"
          variant="filled"
          :disabled="!permissions.developerPermissionsDto?.canCreateApps && !permissions.workspaceUserPermissionDto?.find(x=> x.slug == workspaceSlug)?.canAddExtensions"
          @click="selectPopulationTrigger()"
      >
        <DsIcon name="heroicons:plus"/>
        Create Population Trigger
      </DsButton>

      <!-- Population Triggers Table -->
      <DsTable
          v-if="state.populationTriggers.length > 0 && state.extensions"
          :headers="populationTriggersHeaders"
          :items="state.populationTriggers"
          :id-selector="item => item.id"
      >
        <template #default="{ item }">
          <div class="flex flex-col">
            <DsText size="md" weight="light">
              {{ state.extensions.resources.find(x => x.appId === item.cdsServiceId)?.appName }}
            </DsText>
            <DsText size="sm" weight="light">
              {{ item.populationType }}
            </DsText>
            <DsText size="sm" weight="light" class="space-x-1.5">
              <DsBadge v-if="item.includedIndicators.includes('critical')" size="sm" :color="Colors.fire" :text-color="Colors.white">
                Critical
              </DsBadge>
              <DsBadge v-if="item.includedIndicators.includes('warning')" size="sm" :color="Colors.marigold" :text-color="Colors.white">
                Warning
              </DsBadge>
              <DsBadge v-if="item.includedIndicators.includes('info')" size="sm" :color="Colors.secondary" :text-color="Colors.white">
                Info
              </DsBadge>
            </DsText>
            <div class="space-x-2 flex flex-row">
              <DsDropdown
                  :id="item.id.replaceAll(' ', '-').toLowerCase() + '-actions'"
                  :options="populationTriggerActions"
                  label="Actions"
                  @select="(v) => { onActionSelected(v, item) }"
              />
            </div>
          </div>
        </template>
      </DsTable>

      <!-- No Triggers -->
      <div v-if="state.populationTriggers?.length === 0">
        <div class="flex flex-col items-center justify-center h-full py-20 border-2 border-silver space-y-5">
          <DsText size="2xl" weight="light">
            No Population Triggers
          </DsText>
          <DsText size="sm" weight="light">
            There are no population triggers for workspace. Click the button below to create one.
          </DsText>
          <DsButton
              :color="Colors.primary"
              variant="filled"
              :disabled="!permissions.developerPermissionsDto?.canCreateApps && !permissions.workspaceUserPermissionDto?.find(x=> x.slug == workspaceSlug)?.canAddExtensions"
              @click="selectPopulationTrigger()"
          >
            <DsIcon name="heroicons:plus"/>
            Create Population Trigger
          </DsButton>
        </div>
      </div>

      <div  v-if="state.reports && state.selectedPopulationTrigger">
        <DsText size="md" weight="light" class="block">
          These reports are for extension <strong>{{
            state.extensions?.resources.find(x => x.appId == state.selectedPopulationTrigger?.cdsServiceId)?.appName
          }}</strong> with indicators: <strong>{{ state.selectedPopulationTrigger.includedIndicators.join(", ") }}</strong>
        </DsText>
        <!-- Population Trigger Reports Table -->
        <DsTable
            v-if="state.reports"
            :headers="reportsHeaders"
            :items="state.reports"
            :id-selector="item => item.id"
        >
          <template #default="{ item }">
            <div class="flex flex-col">
              <DsText size="md" weight="light">
                {{ item.id }}
              </DsText>
              <DsText size="md" weight="light">
                {{ item.reportTime }}
              </DsText>
              <div class="space-x-2 flex flex-row">
                <DsButton @click="viewReportData(item.id)">
                  View Report
                </DsButton>
              </div>
            </div>
          </template>
        </DsTable>
      </div>

    </div>

    <PopulationTriggerModal
        :show="state.showPopulationTriggerModal"
        :workspace-slug="workspaceSlug"
        :population-trigger="state.selectedPopulationTrigger"
        @create-population-trigger="(v) => createPopulationTrigger(v)"
        @edit-population-trigger="(v) => updatePopulationTrigger(v)"
        @close="state.showPopulationTriggerModal = false;"
    />
  </DsContainer>
</template>
