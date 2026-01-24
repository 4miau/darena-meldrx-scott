<script setup lang="ts">
import type { ApiUsageTimeFilter, StorageUsageTimeFilter } from '~/types/meldrx-api/subscriptions'
import type UsageGraphPoint from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/UsageGraphPoint';

useHead({ title: 'Billing and Usage | MeldRx' });
const { $api } = useNuxtApp();

const route = useRoute();

const optionsApiCallsTimeRange: { value: ApiUsageTimeFilter, title: string }[] = [
    { value: 'current-month', title: 'Current Month' },
    { value: 'year', title: 'Year' }
];

const optionsStorageTimeRange: { value: StorageUsageTimeFilter, title: string }[] = [
    { value: 'current-month', title: 'Current Month' },
    { value: 'year', title: 'Year' }
];

const { subscription } = useSubscription()

const state = reactive<{
  isLoading: boolean;
  apiCallsUsed: number;
  apiUsage: UsageGraphPoint[];
  storageUsed: string;
  storageUsage: UsageGraphPoint[];
  workspaceOptions: { value: string, title: string }[];

  selectedWorkspaceOption: string;
  selectedApiCallsTimeRangeOption: ApiUsageTimeFilter;
  selectedStorageTimeRangeOption: StorageUsageTimeFilter;

  moreWorkspaces: number;
}>({
    isLoading: false,
    apiCallsUsed: 0,
    apiUsage: [],
    storageUsed: '0',
    storageUsage: [],
    workspaceOptions: [],

    selectedWorkspaceOption: route.query.workspaceSlug as string | undefined ?? 'all',
    selectedApiCallsTimeRangeOption: optionsApiCallsTimeRange[0].value,
    selectedStorageTimeRangeOption: optionsStorageTimeRange[0].value,

    moreWorkspaces: subscription.value.includedWorkspaces
});

async function loadWorkspaces() {
    try {
        const workspaces = await $api.subscriptions.workspaces(subscription.value.id);
        state.workspaceOptions = workspaces.map(x => ({ value: x.id, title: `${x.name} ${x.deleted ? '(Deleted)' : ''}` }));
        state.workspaceOptions.unshift({ value: 'all', title: 'All Workspaces' });
    } catch (error) {
        handleApiError(error, 'Unable to load workspaces');
    }
}

async function loadApiUsage(workspaceId: string, filter: ApiUsageTimeFilter) {
    state.isLoading = true;

    state.apiUsage = await $api.subscriptions.getApiUsage(workspaceId, filter);

    state.isLoading = false;
}

async function loadStorageUsage(workspaceId: string, filter: StorageUsageTimeFilter) {
    state.isLoading = true;

    state.storageUsage = await $api.subscriptions.getStorageUsage(workspaceId, filter);

    state.isLoading = false;
}

// When workspace changes, reload API and Storage data...
function onWorkspaceChange(workspaceId: string) {
    loadApiUsage(workspaceId, state.selectedApiCallsTimeRangeOption);
    loadStorageUsage(workspaceId, state.selectedStorageTimeRangeOption);
}

// When API Usage filter changes, reload API usage data...
function onApiUsageFilterChange(filter: ApiUsageTimeFilter) {
    state.selectedApiCallsTimeRangeOption = filter
    loadApiUsage(state.selectedWorkspaceOption, state.selectedApiCallsTimeRangeOption);
}

// When Storage filter changes, reload storage data...
function onStorageFilterChange(filter: StorageUsageTimeFilter) {
    state.selectedStorageTimeRangeOption = filter;
    loadStorageUsage(state.selectedWorkspaceOption, state.selectedStorageTimeRangeOption);
}

const apiChartOptions = computed(() => ({
    chart: {
        type: 'bar'
    },
    plotOptions: {
        bar: {
            borderRadius: 0,
            borderRadiusApplication: 'around'
        }
    },
    colors: ['#004266'],
    dataLabels: {
        enabled: false
    },
    fill: {
        opacity: 1.0
    },
    xaxis: {
        categories: state.apiUsage.map(d => d.date),
        title: {
            text: 'Date'
        }
    },
    yaxis: {
        title: {
            text: 'API Calls'
        }
    }
}))

const apiChartSeries = computed(() => [
    {
        name: 'API Calls',
        data: state.apiUsage.map(d => d.amount)
    }
])
const storageChartOptions = computed(() => ({
    chart: {
        type: 'bar'
    },
    plotOptions: {
        bar: {
            borderRadius: 0,
            borderRadiusApplication: 'around'
        }
    },
    colors: ['#02B689'],
    dataLabels: {
        enabled: false
    },
    fill: {
        opacity: 1.0
    },
    xaxis: {
        categories: state.storageUsage.map(d => d.date),
        title: {
            text: 'Date'
        }
    },
    yaxis: {
        title: {
            text: 'Storage (MB)'
        }
    }
}))

const storageChartSeries = computed(() => [
    {
        name: 'Storage Used',
        data: state.storageUsage.map(d => d.amount)
    }
])

loadWorkspaces();
loadApiUsage('all', state.selectedApiCallsTimeRangeOption);
loadStorageUsage('all', state.selectedStorageTimeRangeOption);
</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="state.isLoading" />

    <div>
      <!-- Title / Description -->
      <DsText size="2xl" weight="light">
        Billing and Usage
      </DsText>
      <div class="pb-5" />
      <DsText size="sm" weight="light">
        View details about your subscription and workspace usage.
      </DsText>
      <div class="pb-5" />

      <!-- Subscription Details -->
      <div v-if="subscription" class="grid grid-cols-12 gap-14">
        <div class="col-span-4">
          <DsLabeledText
            label="Subscription Details"
            text="This information outlines the specifics of your organization's MeldRx subscription."
          />
        </div>

        <!-- Workspace Type Buttons -->
        <div class="col-span-8 space-y-2">
          <div>
            <DsLabeledText label="Status">
              <div>
                <DsText size="xs" weight="light">
                  {{ subscription.status }}
                </DsText>
              </div>
            </DsLabeledText>
          </div>

          <DsLabeledText label="Subscription Id">
            <DsTextWithCopyButton
              id="subscription-id"
              size="xs"
              weight="light"
              :text="`${subscription.id}`"
              :text-to-copy="subscription.id"
              :show-toast-on-copy="true"
              toast-message-on-copy="Copied Subscription ID to clipboard."
            />
          </DsLabeledText>

          <div class="capitalize">
            <DsLabeledText label="Subscription">
              <DsText size="xs" weight="light">
                {{ subscription.subscriptionType }}
              </DsText>
            </DsLabeledText>
          </div>

          <div class="flex space-x-11">
            <DsLabeledText label="Provider Licenses Used">
              <DsText size="xs" weight="light">
                {{ subscription.providersCount }} of
                <template v-if="subscription.includedProviders === -1">
                  Unlimited
                </template>
                <template v-else>
                  {{ subscription.includedProviders.toLocaleString() }}
                </template>
              </DsText>
            </DsLabeledText>
          </div>

          <div>
            <DsLabeledText label="Workspace Licenses Used">
              <DsText size="xs" weight="light">
                {{ subscription.workspaceCount }} of {{ subscription.includedWorkspaces.toLocaleString() }}
              </DsText>
            </DsLabeledText>
          </div>

          <div class="flex space-x-10">
            <DsLabeledText label="API Calls Used">
              <DsText size="xs" weight="light">
                {{ subscription.apiCallsUsed.toLocaleString() }} of
                <template v-if="subscription.includedApiCalls === -1">
                  Unlimited
                </template>
                <template v-else>
                  {{ subscription.includedApiCalls.toLocaleString() }}
                </template>
              </DsText>
            </DsLabeledText>
          </div>

          <div class="flex space-x-11">
            <DsLabeledText label="Storage Used">
              <DsText size="xs" weight="light">
                {{ subscription.dataStorageUsed.toLocaleString() }} MiB of {{ subscription.includedDataStorage.toLocaleString() }} MiB
              </DsText>
            </DsLabeledText>
          </div>

        </div>
      </div>
      <DsDivider />

      <!-- Usage History -->
      <div class="space-y-5">
        <div>Usage History</div>

        <DsSelect
          v-model="state.selectedWorkspaceOption"
          :items="state.workspaceOptions"
          :placeholder="`Workspace`"
          searchable
          search-placeholder="Search Workspaces"
          class="w-1/4"
          @update:model-value="onWorkspaceChange"
        />

        <DsText size="md" weight="bold" class="flex justify-center ">
          API Usage
        </DsText>

        <div class="flex justify-start pl-10">
          <DsSelect
            :model-value="state.selectedApiCallsTimeRangeOption"
            :items="optionsApiCallsTimeRange"
            @update:model-value="onApiUsageFilterChange"
          />
        </div>

        <div class="flex justify-start pl-10 space-x-10">
          <div v-if="state.selectedApiCallsTimeRangeOption != 'year'">
            <DsText size="md" weight="bold" class="border border-silver pt-[10px] pr-[20px] pb-[10px] pl-[20px] shadow-md">
              Total Usage: {{ subscription.apiCallsUsed.toLocaleString() }} of {{ subscription.includedApiCalls.toLocaleString() }} calls
            </DsText>
          </div>

          <div v-if="state.selectedWorkspaceOption != 'all' && state.selectedApiCallsTimeRangeOption != 'year'">
            <DsText size="md" weight="bold" class="border border-silver pt-[10px] pr-[20px] pb-[10px] pl-[20px] shadow-md">
              For Selected Workspace: {{ state.apiUsage.reduce((s, n) => s + n.amount, 0).toLocaleString() }} calls
            </DsText>
          </div>

          <div v-if="state.selectedApiCallsTimeRangeOption === 'year'">
            <DsText size="md" weight="bold" class="border border-silver pt-[10px] pr-[20px] pb-[10px] pl-[20px] shadow-md">
              Total Usage: {{ state.apiUsage.reduce((s, n) => s + n.amount, 0).toLocaleString() }} calls
            </DsText>
          </div>
        </div>

        <apexchart
          key="chart-api-usage"
          height="400"
          width="100%"
          :options="apiChartOptions"
          :series="apiChartSeries"
        />

        <DsDivider />

        <DsText size="md" weight="bold" class="flex justify-center ">
          Storage Usage
        </DsText>

        <div class="flex justify-start pl-10">
          <DsSelect
            :model-value="state.selectedStorageTimeRangeOption"
            :items="optionsStorageTimeRange"
            @update:model-value="onStorageFilterChange"
          />
        </div>

        <div class="flex justify-start pl-10 space-x-10">
          <div v-if="state.selectedStorageTimeRangeOption != 'year'">
            <DsText size="md" weight="bold" class="border border-silver pt-[10px] pr-[20px] pb-[10px] pl-[20px] shadow-md">
              Total Usage: {{ subscription.dataStorageUsed.toLocaleString() }} MiB of {{ subscription.includedDataStorage.toLocaleString() }} MiB
            </DsText>
          </div>

          <div v-if="state.selectedWorkspaceOption != 'all' && state.selectedStorageTimeRangeOption != 'year'">
            <DsText size="md" weight="bold" class="border border-silver pt-[10px] pr-[20px] pb-[10px] pl-[20px] shadow-md">
              For Selected Workspace: {{ state.storageUsage.toReversed().find(x => x.amount > 0)?.amount.toLocaleString() ?? '0' }} MiB
            </DsText>
          </div>

          <div v-if="state.selectedStorageTimeRangeOption === 'year'">
            <DsText size="md" weight="bold" class="border border-silver pt-[10px] pr-[20px] pb-[10px] pl-[20px] shadow-md">
              Total Usage: {{ subscription.dataStorageUsed.toLocaleString() }} MiB
            </DsText>
          </div>
        </div>

        <apexchart
          key="chart-storage-usage"
          height="400"
          width="100%"
          :options="storageChartOptions"
          :series="storageChartSeries"
        />
      </div>
      <DsDivider />
    </div>
  </DsContainer>
</template>
