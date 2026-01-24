<script setup lang="ts">
import { format } from 'date-fns';
import type { BaseBackgroundJobDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/BackgroundJobs/BaseBackgroundJobDto';
import {
    BackgroundJobStatus,
    backgroundJobStatusConfig
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/BackgroundJobStatus';
import { BackgroundJobType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/BackgroundJobType';
import { Colors } from '~/types/ui/colors';
import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'
import type { UploadDocumentTypes } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/UploadDocumentTypes';

definePageMeta({ layout: 'workspace', middleware: ['require-workspace'] });
useHead({ title: 'Import Data | MeldRx' });

const { $api } = useNuxtApp()

const route = useRoute()


const state = ref<{
  backgroundJobItems?:PagedResult<BaseBackgroundJobDto>;
  page: number;
  pageSize: number;
  queryStartDate?: string;
  queryEndDate?: string;
}>({
    backgroundJobItems: undefined,
    page: 1,
    pageSize: 10,
    queryStartDate: new Date(new Date().setDate(new Date().getDate() - 7)).toISOString().split('T')[0],
    queryEndDate: new Date().toISOString().split('T')[0],
});

const isLoading = ref<boolean>(false);
const workspaceSlug = ref<string>(route.params.workspaceSlug as string);
const tableHeaders = ['Job Id', 'Date Submitted', 'Status'];
const openImportDataModal = ref<boolean>(false);
const currentJobStatus = ref<BackgroundJobStatus>(BackgroundJobStatus.None);
const allowedFileImports = ref<UploadDocumentTypes[]>([
    { fileExtension: ['.xml'], fileDescription: 'CCDA (.xml)' },
    { fileExtension: ['.json'], fileDescription: 'FHIR R4 Bundle (.json)' }
]);

// "Import Data" button clicked...
function onImportDataButtonClick() {
    openImportDataModal.value = true;
}
// After the user submits the modal...
async function onImportData(value: { fileContents: string, fileExtension: string }) {
    openImportDataModal.value = false;
    const { fileContents, fileExtension } = value;

    if (fileExtension === 'xml') {
        isLoading.value = true;
        try {
            await $api.dataImport.importCcda(workspaceSlug.value, fileContents);
        } catch (error) {
            handleApiError(error, 'Unable to import');
            isLoading.value = false;
            return;
        }
        notification({ title: 'Success', description: 'Data imported successfully', displayTime: 3000, variant: 'success' });
    } else if (fileExtension === 'json') {
        isLoading.value = true;
        const bundle = FHIRUtils.createBundleFromFileContents(fileContents);
        try {
            await $api.fhir.uploadBundle(workspaceSlug.value, bundle);
        } catch (error) {
            handleApiError(error, 'Unable to import');
            isLoading.value = false;
            return;
        }

        notification({ title: 'Success', description: 'Data imported successfully', displayTime: 3000, variant: 'success' });
    }
    await onRefreshClick(state.value.page, state.value.pageSize, state.value.queryStartDate, state.value.queryEndDate);
    isLoading.value = false;
}

async function onRefreshClick(currentPage:number, pageSize:number, queryStartDate?:string, queryEndDate?:string) {
    isLoading.value = true;
    try {
        state.value.backgroundJobItems = await $api.dataImport.listJobs(
            workspaceSlug.value,
            BackgroundJobType.StructuredFileImport,
            BackgroundJobStatus[currentJobStatus.value as keyof typeof BackgroundJobStatus],
            queryStartDate ? DateTime.toISODateString(queryStartDate) : '',
            queryEndDate ? `${DateTime.toISODateString(queryEndDate)}T23:59:59` : '',
            currentPage,
            pageSize
        );
    } catch (error) {
        handleApiError(error, 'Unable to import');
        isLoading.value = false;
        return;
    }
    isLoading.value = false;
}

await onRefreshClick(state.value.page, state.value.pageSize, state.value.queryStartDate, state.value.queryEndDate);

</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="isLoading" />
    <!-- Import Data Modal -->
    <ImportDataModal
      :show="openImportDataModal"
      title="Import Data"
      description="Import data into this workspace. Supported types include:"
      :allowed-import-types = allowedFileImports
      @close="() => openImportDataModal = false"
      @cancel="() => openImportDataModal = false"
      @import-data="onImportData"
    />

    <!-- No Import Data -->
    <div v-if="false" class="space-y-5">
      <div class="flex flex-col items-center justify-center h-full py-20 border-2 border-silver">
        <DsText size="2xl" weight="light">
          No Import Jobs
        </DsText>
        <DsText size="sm" weight="light">
          There are no import jobs currently.
        </DsText>
        <DsButton variant="filled" @click="() => onImportDataButtonClick()">
          Import Data
        </DsButton>
      </div>
    </div>

    <div v-else class="space-y-5">
      <!-- Header Text -->
      <DsText size="2xl" weight="light">
        Import Data
      </DsText>
      <DsText size="sm" weight="light" class="block">
        Import data into the workspace. View the status of import jobs.
      </DsText>

      <!-- Import Data Button -->
      <DsButton variant="filled" @click="() => onImportDataButtonClick()">
        <DsIcon name="heroicons:plus" />
        Import Data
      </DsButton>

      <DsDivider />

      <div class="flex items-center space-x-4">
        <!-- Date Filters with Labels -->
        <div class="flex flex-col">
          <label class="mb-1 text-onyx">Start Date</label>
          <DsDatePicker v-model="state.queryStartDate" past-only />
        </div>

        <div class="flex flex-col">
          <label class="mb-1 text-onyx">End Date</label>
          <DsDatePicker v-model="state.queryEndDate" />
        </div>

        <!-- Job Status with Label -->
        <div class="flex flex-col ml-8">
          <label class="mb-1 text-onyx">Job Status</label>
          <DsSelect v-model="currentJobStatus" :items="backgroundJobStatusConfig" class="w-[120px]" />
        </div>

        <!-- Refresh Button -->
        <div class="ml-4 self-end">
          <DsButton :color="Colors.primary" :text-color='Colors.primary' variant="outline" @click="() => onRefreshClick(state.page, state.pageSize, state.queryStartDate, state.queryEndDate)">
            Refresh
          </DsButton>
        </div>
      </div>

      <!-- Jobs Table -->
      <DsTable
        v-if="state.backgroundJobItems"
        :headers="tableHeaders"
        :items="state.backgroundJobItems.resources"
        :id-selector="item => item.id"
      >
        <template #default="{item}">
          <DsText size="md" weight="light">
            {{ item.id }}
          </DsText>

          <DsText size="md" weight="light">
            {{ format(new Date(item.createdOn.toString()), 'yyyy-MM-dd hh:mm a') }}
          </DsText>

          <DsText size="md" weight="light">
            {{ item.status }}
          </DsText>
        </template>
        <template #footer>
          <DsTablePager class="pl-6" :paged-result-info="state.backgroundJobItems" @go-to-page="onRefreshClick($event, state.pageSize, state.queryStartDate, state.queryEndDate)" />
        </template>
      </DsTable>
    </div>
  </DsContainer>
</template>
