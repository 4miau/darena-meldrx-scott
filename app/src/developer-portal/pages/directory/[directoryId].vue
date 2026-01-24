<script setup lang="ts">
import { Colors } from '~/types/ui/colors'
import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'
import type { Guid } from '~/types/common/Guid'
import type { DirectoryListingDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DirectoryListingDto'
import type { DirectoryProviderTableView } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Directories/DirectoryProviderTableView'
import type { ApiFilter } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter'

definePageMeta({ anonymous: true, layout: 'blank' });
const { $api } = useNuxtApp()
const router = useRouter()

const directoryId = router.currentRoute.value.params.directoryId as Guid;
const tableHeaders = [
    { title: 'Provider Name', orderBy: 'name' },
    { title: 'NPI', orderBy: 'npi' }
]
const state = reactive<{
  isLoading: boolean,
  directory?: DirectoryListingDto,
  providers?: PagedResult<DirectoryProviderTableView>,
  apiFilter: ApiFilter,
}>({
    isLoading: false,
    apiFilter: {
        page: 1
    }
})

useQuerySync(() => state.apiFilter)

useHead(() => ({ title: `MeldRx Directory ${state.directory ? '| ' + state.directory.displayName : ''}` }));

async function init(id: Guid) {
    state.isLoading = true;
    try {
        state.directory = await $api.directories.get(id)
        state.providers = await $api.directories.listProviders(id, state.apiFilter)
    } finally {
        state.isLoading = false;
    }
}

async function loadProviders(apiFilter: ApiFilter) {
    state.isLoading = true;
    try {
        state.providers = await $api.directories.listProviders(directoryId, apiFilter)
        state.apiFilter = apiFilter
        state.apiFilter.page = state.providers.currentPage
    } finally {
        state.isLoading = false;
    }
}

init(directoryId)

</script>

<template>
  <DsLoadingOverlay :loading="state.isLoading" />
  <DsContainer v-if="state.directory" class="space-y-5">
    <div class="flex gap-5">
      <div class="">
        <DsText size="2xl" weight="light">
          {{ state.directory.displayName }}
        </DsText>
        <br>
        <DsText size="sm" weight="light">
          {{ state.directory.displayAddress }}
        </DsText>
      </div>
    </div>

    <DsButton
      :color="Colors.white"
      :text-color='Colors.gray'
      size="sm"
      variant="outline"
      @click="navigateTo('/directory')"
    >
      <DsIcon name="heroicons:arrow-small-left"/>
      Back to Directory
    </DsButton>

    <div>
      <DsText weight="light">
        This page lists all of the providers that are HTI-1 certified by {{ state.directory.displayName }}. <br>
        To request access to this API,
        <DsLink new-tab class="text-secondary" href="https://support.meldrx.com/">contact support</DsLink>.
      </DsText>
    </div>

    <div class="flex gap-4 items-center">
      <div>
        <DsText weight="bold">
          Workspace URL:
        </DsText>
        <DsText weight="light">
          {{ state.directory.fhirServerUrl }}
        </DsText>
      </div>
      <DsCopyButton
        class="text-xs font-light"
        :text-to-copy="state.directory.fhirServerUrl ?? ''"
        show-toast-on-copy
        toast-message-on-copy="Copied Workspace URL to clipboard."
      >
        Copy
      </DsCopyButton>
    </div>

    <div>
      <DsText weight="bold">
        EHR Vendor:
      </DsText>
      <DsText weight="light">
        {{ state.directory.organization }}
      </DsText>
    </div>

    <div class="w-full">
      <DsTextInput
        v-model="state.apiFilter.filter"
        placeholder="Search provider by name or npi"
        @enter-pressed="() => loadProviders(state.apiFilter)"
      >
        <template #right>
          <DsButton
            :color="Colors.white"
            :text-color="Colors.gray"
            variant="outline"
            @click="() => loadProviders(state.apiFilter)"
          >
            Search
            <DsIcon name="heroicons:magnifying-glass" />
          </DsButton>
        </template>
      </DsTextInput>
    </div>

    <DsTable
      v-if="state.providers"
      :headers="tableHeaders"
      :items="state.providers.resources"
      :id-selector="item => item.npi"
      :api-filter="state.apiFilter"
      @update:api-filter="loadProviders"
    >
      <template #default="{item}">
        <DsText weight="light">
          {{ item.providerName }}
        </DsText>

        <DsText weight="light">
          {{ item.npi }}
        </DsText>
      </template>
      <template #footer>
        <DsTablePager
          v-if="state.providers.totalPages > 1"
          class="pl-6"
          :paged-result-info="state.providers"
          @go-to-page="(page) => loadProviders({...state.apiFilter, page})"
        />
      </template>
    </DsTable>
  </DsContainer>
</template>
