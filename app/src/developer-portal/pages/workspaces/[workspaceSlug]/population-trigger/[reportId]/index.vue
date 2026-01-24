<script setup lang="ts">
import type {PopulationTriggerReportData} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PopulationTriggerReportData";
import type {Card} from "~/types/cds-hooks/CDSCards";
import {Colors} from "~/types/ui/colors";

definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    allowLinked: true,
})
useHead({title: 'Population Trigger Report | MeldRx'})

const {$api} = useNuxtApp()
const route = useRoute()
const workspaceSlug = ref<string>(route.params.workspaceSlug as string)
const reportId = ref<string>(route.params.reportId as string)
const reportDataHeaders = ['Patient Name/Id', 'Gender', 'DoB', "Included Indicators" , 'Cards'];

const state = reactive<{
  reportData?:PopulationTriggerReportData[];
  showCards: boolean;
  selectedCards?: Card[];
  loading: boolean;
}>({
    loading: false,
    showCards: false,
})

async function loadReportData(){
    state.loading = true;
    try {
        state.reportData = await $api.workspaces.populationTriggers.getPopulationTriggerReportData(workspaceSlug.value, reportId.value)
    } catch (error) {
        handleApiError(error, 'Unable to load execution data')
    }finally {
        state.loading = false;
    }
}

function viewCards(cards: Card[]) {
    state.showCards = true
    state.selectedCards = cards
}

loadReportData()
</script>

<template>
  <DsContainer>

      <!-- Card Viewer -->
      <DsViewer
          v-if="state.showCards"
          @close="() => state.showCards = false"
      >
        <div class="p-5 overflow-auto min-w-[400px] space-y-5">
          <div v-for="item in state.selectedCards" :key="item.uuid">
            <DsCard
                v-if="item"
                display-only
                :show-card="state.showCards"
                :card="item"
                :workspace-slug="route.params.workspaceSlug.toString()"
            />
          </div>
        </div>
      </DsViewer>
    
    <div class="space-y-5">
      <DsText size="2xl" weight="light" class="block">
        Population Trigger Report
      </DsText>

      <DsText size="sm" weight="light" class="block">
        View the data returned by the extension service.
      </DsText>

      <!-- Execution Data Table -->
      <DsTable
          v-if="state.reportData"
          :headers="reportDataHeaders"
          :items="state.reportData"
          :id-selector="item => item.patientId"
      >
        <template #default="{ item }">
          <div class="flex flex-col">
            <div class="flex flex-col items-start">
              <DsLink underline="always" new-tab :href="`/workspaces/${route.params.workspaceSlug}/patient/${item.patientId}`">
                <DsText size="md" weight="light">
                  {{ item.patientName }}
                </DsText>
              </DsLink>
              <DsTextWithCopyButton
                  size="xs"
                  :text="`Patient Id - ${ item.patientId }`"
                  :text-to-copy="item.patientId"
                  :show-toast-on-copy="true"
                  toast-message-on-copy="Copied Patient Id to clipboard."
              />
            </div>
            <DsText size="md" weight="light">
              {{ item.gender }}
            </DsText>
            <DsText size="md" weight="light">
              {{ item.doB }}
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
              <DsButton @click.stop="viewCards(item.cards)">
                View Cards
              </DsButton>
            </div>
          </div>
        </template>
      </DsTable>
    </div>
  </DsContainer>
</template>
