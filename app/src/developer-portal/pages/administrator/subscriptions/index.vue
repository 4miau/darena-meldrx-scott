<script setup lang="ts">
import { Colors } from "~/types/ui/colors";
import type { PagedResult } from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult";
import type { ApiFilter } from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter";
import type SubscriptionDto from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/SubscriptionDto";

useHead({ title: 'Subscription Admin | MeldRx' });

const { $api } = useNuxtApp();


const state = reactive<{
  isLoading: boolean;
  subscriptions?: PagedResult<SubscriptionDto>;
  apiFilter: ApiFilter;
  openModal: boolean;
  selectedSubscription: SubscriptionDto | null
}>({
    isLoading: false,
    subscriptions: undefined,
    apiFilter: {
        page: 1,
        orderBy: 'name'
    },
    openModal: false,
    selectedSubscription: null
});

const tableHeaders = [
    { title: 'Organization Name', orderBy: 'name' },
    { title: 'Type', orderBy: 'type' },
    'Actions'
];

enum Actions {
  edit = 'EDIT',
}

async function loadSubscriptions(apiFilter: ApiFilter) {
    state.isLoading = true;

    try {
        state.subscriptions = await $api.admin.subscriptions.search(apiFilter);
        state.apiFilter = apiFilter;
        state.apiFilter.page = state.subscriptions?.currentPage ?? 1;
    } catch (error) {
        handleApiError(error, 'Unable to load subscriptions');
    }

    state.isLoading = false;
}

function onActionSelected(value: Actions, subscription: SubscriptionDto) {
    if (value === Actions.edit) {
        state.selectedSubscription = subscription;
        state.openModal = true;
    }
}

async function closeModal() {
    state.openModal = false;
    await loadSubscriptions(state.apiFilter);
}

loadSubscriptions(state.apiFilter)
</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="state.isLoading" />
    <div v-if="state.selectedSubscription">
      <EditSubscriptionModal
        :show-modal="state.openModal" :subscription-dto="state.selectedSubscription"
        @close="closeModal" />
    </div>
    <div class="w-full">
      <DsText size="2xl" weight="light">
        Subscriptions
      </DsText>
      <div class="pb-5"/>
      <DsTextInput
        id="search-organization"
        v-model="state.apiFilter.filter"
        placeholder="Search organization"
        @enter-pressed="() => loadSubscriptions(state.apiFilter)">
        <template #right>
          <DsButton :color="Colors.white" :text-color='Colors.gray' variant="outline" @click="() => loadSubscriptions(state.apiFilter)">
            Search
            <DsIcon name="heroicons:magnifying-glass"/>
          </DsButton>
        </template>
      </DsTextInput>
    </div>
    <div class="pb-5"/>
    <DsTable
      v-if="state.subscriptions" :headers="tableHeaders" :items="state.subscriptions.resources"
      :id-selector="item => item.id" :api-filter="state.apiFilter" @update:api-filter="loadSubscriptions">
      <template #default="{ item }">
        <div class="flex-col">
          <DsText size="lg" weight="light" class="inline-flex items-center">
            <DsLink underline="always" :href="`/administrator/organizations?page=1&orderBy=name&filter=${item.resellerId}`">
              {{ item.organizationName }}
            </DsLink>
          </DsText>
          <DsTextWithCopyButton
            size="xs" :text="`Reseller ID: ${item.resellerId}`" :text-to-copy="item.resellerId"
            show-toast-on-copy toast-message-on-copy="Copied reseller ID to clipboard." />
        </div>
        <DsText size="md" weight="light">
            {{ item.subscriptionType }}
          </DsText>
        <div class="flex gap-2">
          <DsButton
            :id="`edit-${item.organizationName?.replaceAll(' ', '-').toLowerCase()}-button`"
            :color="Colors.white"
            :text-color='Colors.gray'
            variant="outline"
            @click="onActionSelected(Actions.edit, item)"
          >
            Edit
          </DsButton>
        </div>
      </template>
      <template #footer>
        <DsTablePager
          class="pl-6" :paged-result-info="state.subscriptions"
          @go-to-page="(page) => loadSubscriptions({ ...state.apiFilter, page })" />
      </template>
    </DsTable>
  </DsContainer>
</template>
