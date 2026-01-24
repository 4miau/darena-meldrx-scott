<script setup lang="ts">
import {format} from 'date-fns';
import type {CdsFeedbackDto} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CdsHooks/Feedback';
import {Colors} from '~/types/ui/colors';
import type {PagedResult} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'
import {type Card, CdsIndicator} from "~/types/cds-hooks/CDSCards";
import type {IDsSelectItem} from "~/types/ui/DsSelect";
import QueryUtils from "~/utils/QueryUtils";


definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    allowLinked: true,
})
useHead({title: 'View CDS Alerts | MeldRx'});

const {$api} = useNuxtApp()
const route = useRoute()


const state = reactive<{
  cdsFeedbackItems?: PagedResult<CdsFeedbackDto>;
  page: number;
  pageSize: number;
  queryStartDate?: string;
  queryEndDate?: string;
  isLoading: boolean;
  jsonDocument: any;
  showJson: boolean;
  showCard: boolean;
  selectedAlertIndicator: string;
}>({
    cdsFeedbackItems: undefined,
    page: 1,
    pageSize: 10,
    queryStartDate: new Date(new Date().setDate(new Date().getDate() - 7)).toISOString().split('T')[0],
    queryEndDate: new Date().toISOString().split('T')[0],
    isLoading: false,
    jsonDocument: '',
    showJson: false,
    showCard: false,
    selectedAlertIndicator: CdsIndicator.All
});

const tableHeaders = ['Card Summary', 'Indicator', 'Feedback', 'Feedback Time', 'Actions'];
const indicatorOptions = ref<IDsSelectItem<string>[]>([
    { value: CdsIndicator.Critical, title: 'Critical' },
    { value: CdsIndicator.Warning, title: 'Warning' },
    { value: CdsIndicator.Info, title: 'Information' },
    { value: CdsIndicator.All, title: 'All' },
]);

async function onRefreshClick(currentPage: number, pageSize: number, queryStartDate?: string, queryEndDate?: string) {
    state.isLoading = true;
    try {
        state.cdsFeedbackItems = await $api.workspaces.getFeedback(
            route.params.workspaceSlug.toString(),
            queryStartDate ? DateTime.toISODateString(queryStartDate) : '',
            queryEndDate ? `${DateTime.toISODateString(queryEndDate)}T23:59:59` : '',
            state.selectedAlertIndicator === 'All' ? '' : state.selectedAlertIndicator,
            '',
            currentPage,
            pageSize
        );
    } catch (error) {
        handleApiError(error, 'Unable to load feedback');
    } finally {
        state.isLoading = false;
    }
}

function showFhirResource(resourceId: string, resourceType: string) {
    const resource = state.cdsFeedbackItems?.resources.find(x => x.id === resourceId);
    if (resourceType === 'card') {
        state.jsonDocument = resource ? resource.card : 'Resource not found';
        state.showCard = true;
        return
    }
    if (resourceType === 'feedback') {
        state.jsonDocument = resource ? JSON.stringify(resource.feedback, null, 2) : 'Resource not found';
    }
    state.showJson = true;
}

function exportFeedback() {
    const query = QueryUtils.objectToQuery(
        {
            startDate : state.queryStartDate ? DateTime.toISODateString(state.queryStartDate) : '',
            endDate : state.queryEndDate ? `${DateTime.toISODateString(state.queryEndDate)}T23:59:59` : '',
            indicator : state.selectedAlertIndicator === 'All' ? '' : state.selectedAlertIndicator,
            patientId : '',
            page : state.page,
            size : state.pageSize
        },
        true
    )
    window.open(`/api/workspaces/${route.params.workspaceSlug.toString()}/export-feedback${query}`, '_blank');
}

onRefreshClick(state.page, state.pageSize, state.queryStartDate, state.queryEndDate);
</script>

<template>
  <!-- Json Resource Viewer Modal -->
  <DsViewer
      v-if="state.showJson"
      @close="() => state.showJson = false"
  >
    <DsDocumentViewer :content="state.jsonDocument" content-type="json"/>
  </DsViewer>
  <DsViewer
      v-if="state.showCard"
      @close="() => state.showCard = false"
  >
    <div class="p-5 overflow-auto min-w-[400px]">
      <DsCard
        :show-card="state.showCard"
        :card="state.jsonDocument as Card"
        :workspace-slug="route.params.workspaceSlug.toString()"
        display-only
      />
    </div>
  </DsViewer>

  <!-- Main Viewer-->
  <DsContainer>
    <DsLoadingOverlay :loading="state.isLoading"/>
    <div class="space-y-5">
      <!-- Header Text -->
      <DsText size="2xl" weight="light">
        CDS Alert Feedback
      </DsText>
      <DsText size="sm" weight="light" class="block">
        Review CDS alert feedback.
      </DsText>
      <DsDivider/>

      <div class="flex items-start gap-4">
        <!-- Start Date -->
        <div class="flex flex-col">
          <label class="mb-1 text-onyx">Start Date</label>
          <DsDatePicker v-model="state.queryStartDate" past-only />
        </div>

        <!-- End Date -->
        <div class="flex flex-col">
          <label class="mb-1 text-onyx">End Date</label>
          <DsDatePicker v-model="state.queryEndDate" />
        </div>

        <!-- Alerts Category -->
        <div class="flex flex-col">
          <label class="mb-1 text-onyx">Alerts Category</label>
          <DsSelect v-model="state.selectedAlertIndicator" class="w-[120px]" :items="indicatorOptions" />
        </div>

        <!-- Buttons aligned at the bottom (end) -->
        <div class="flex flex-row gap-4 self-end">
          <DsButton :color="Colors.primary" :text-color='Colors.primary' variant="outline" size="md" @click="() => onRefreshClick(state.page, state.pageSize, state.queryStartDate, state.queryEndDate)">
            Refresh
          </DsButton>
          <DsButton :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" size="md" @click="exportFeedback">
            Export Feedback
          </DsButton>
        </div>
      </div>

      <!-- No Patients -->
      <div v-if="state.cdsFeedbackItems?.total === 0">
        <div class="flex flex-col items-center justify-center h-full py-20 border-2 border-silver space-y-5">
          <DsText size="2xl" weight="light">
            No Feedback Found
          </DsText>
          <DsText size="sm" weight="light">
            Check the filters and try again. Otherwise submit feedback to view it here.
          </DsText>
        </div>
      </div>

      <!-- Feedback Table -->
      <DsTable
          v-if="state.cdsFeedbackItems && state.cdsFeedbackItems.resources.length"
          :headers="tableHeaders"
          :items="state.cdsFeedbackItems!.resources"
          :id-selector="item => item.id"
      >
        <template #default="{item}">
          <div class="flex flex-col">
            <DsText size="sm" weight="light">{{ item.card.summary }}</DsText>
            <DsText size=xs weight="light">{{ item.card.detail }}</DsText>
          </div>
          <DsText size="sm" weight="light">{{ item.card.indicator }}</DsText>
          <DsText size="sm" weight="light">{{ item.feedback.outcome }}</DsText>
          <DsText size="sm" weight="light"> {{format(new Date(item.feedback.outcomeTimestamp), 'yyyy-MM-dd hh:mm a') }}
          </DsText>

          <div class="flex space-x-1">
            <DsButton :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" size="xs" @click.stop="showFhirResource(item.id, 'card')">
              View Card
            </DsButton>
            <DsButton :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" size="xs" @click.stop="showFhirResource(item.id, 'feedback')">
              View Feedback
            </DsButton>
          </div>
        </template>
        <template #footer>
          <DsTablePager v-if="state.cdsFeedbackItems.totalPages > 1" class="pl-6" :paged-result-info="state.cdsFeedbackItems!" @go-to-page="onRefreshClick($event, state.pageSize, state.queryStartDate, state.queryEndDate)"/>
        </template>
      </DsTable>
    </div>
  </DsContainer>
</template>
