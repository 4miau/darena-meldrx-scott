<script setup lang="ts">

import {Colors} from "~/types/ui/colors";
import type {MarketplaceApiFilter} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter";
import type {PagedResult} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult";
import type {
    PublishedAppDetailsForMarketplace
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/PublishedAppDetails";
import {PublishedStatus} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/PublishedAppDto";

useHead({ title: 'App and Extension Marketplace' });
definePageMeta({ anonymous: true, layout: 'default' });
const { $api } = useNuxtApp()
const confirmation = useConfirmation()

const state = reactive<{
  isLoading: boolean,
  apps?: PagedResult<PublishedAppDetailsForMarketplace>,
  apiFilter: MarketplaceApiFilter,
}>({
    isLoading: false,
    apiFilter: {
        page: 1,
        category: '',
        price: '',
        verified: '',
        hosted: '',
        ascending: true,
    }
})

async function loadApps(filter: MarketplaceApiFilter) {
    state.isLoading = true;
    try {
        state.apps = await $api.marketplace.getPublishedApps(filter)

    } catch (error) {
        handleApiError(error, 'Unable to get apps for activation');
    } finally {
        state.isLoading = false;
    }
}

const categoryItems = [
    { value: "", title: 'All Categories' },
    { value: "cds-hooks", title: 'CDS Hooks' },
    { value: "ehr-launch", title: 'EHR Launch' },
]
const priceItems = [
    { value: "free", title: 'Free' },
    { value: "paid", title: 'Paid' },
    { value: "", title: 'All Apps' },
]
const verifiedItems = [
    { value: "verified", title: 'Verified' },
    { value: "not-verified", title: 'Not Verified' },
    { value: "", title: 'All Apps' },
]
const hostedItems = [
    { value: "hosted", title: 'Hosted' },
    { value: "not-hosted", title: 'Not Hosted' },
    { value: "", title: 'All Apps' },
]
const sortingItems = [
    { value: true, title: 'A-Z' },
    { value: false, title: 'Z-A' },
]

async function adminUnpublish(appId: string) {
    const {isCancelled} = await confirmation(
        'Are you sure you want to unpublish this app?',
        'Unpublish'
    )
    if (isCancelled) {
        return
    }
    
    try {
        await $api.admin.apps.appDetailsUpdate({
            appId:appId,
            publishedStatus: PublishedStatus.NotPublished
        })
    }catch (e) {
        handleApiError(e, 'Action failed.')
    }

    await loadApps(state.apiFilter)
}

loadApps(state.apiFilter)
</script>

<template>
  <DsContainer class="space-y-5">
    <div class="flex gap-5">
      <div class="flex flex-col gap-3">
        <DsText size="2xl" weight="light">
          App and Extension Marketplace
        </DsText>
        <DsText size="sm" weight="light">
          Get access to apps or add them to your workspace as an extension.
        </DsText>
      </div>
    </div>
    
    <div class="w-full">
      <DsTextInput
          id="directory-search"
          v-model="state.apiFilter.filter"
          placeholder="Search apps by name, publisher, category"
          @enter-pressed="() => loadApps(state.apiFilter)"
      >
        <template #right>
          <DsButton
              :color="Colors.white"
              :text-color='Colors.gray'
              variant="outline"
              @click="() => loadApps(state.apiFilter)"
          >
            Search
            <DsIcon name='i-heroicons-magnifying-glass' />
          </DsButton>
        </template>
      </DsTextInput>
    </div>

    <div class="flex gap-5 justify-between">
      <div class="flex gap-3 items-center">
        <DsText size="sm" weight="light">
          Category
        </DsText>
        <DsSelect :model-value="state.apiFilter.category" :items="categoryItems" @update:model-value="state.apiFilter.category = $event, loadApps(state.apiFilter)" />
      </div>
      <div class="flex gap-3 items-center">
        <DsText size="sm" weight="light">
          Price
        </DsText>
        <DsSelect :model-value="state.apiFilter.price" :items="priceItems" @update:model-value="state.apiFilter.price = $event, loadApps(state.apiFilter)" />
      </div>
      <div class="flex gap-3 items-center">
        <DsText size="sm" weight="light">
          MeldRx Verified
        </DsText>
        <DsSelect :model-value="state.apiFilter.verified" :items="verifiedItems" @update:model-value="state.apiFilter.verified = $event, loadApps(state.apiFilter)" />
      </div>
      <div class="flex gap-3 items-center">
        <DsText size="sm" weight="light">
          MeldRx Hosted
        </DsText>
        <DsSelect :model-value="state.apiFilter.hosted" :items="hostedItems" @update:model-value="state.apiFilter.hosted = $event, loadApps(state.apiFilter)" />
      </div>
      <div class="flex gap-3 items-center">
        <DsText size="sm" weight="light">
          Sort By
        </DsText>
        <DsSelect :model-value="state.apiFilter.ascending" :items="sortingItems" @update:model-value="state.apiFilter.ascending = $event, loadApps(state.apiFilter)" />
      </div>
    </div>
    
    <MarketplaceTable
        v-if="state.apps?.resources && state.apps?.resources.length > 0"
        :apps="state.apps?.resources"
        @unpublish="(e) => adminUnpublish(e)"
    />
    <DsTablePager
        v-if="state.apps"
        class="pl-6"
        :paged-result-info="state.apps"
        @go-to-page="(page) => loadApps({...state.apiFilter, page})"
    />
    
  </DsContainer>
</template>