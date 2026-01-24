<script setup lang="ts">
import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'
import type { DirectoryTableView } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Directories/DirectoryTableView'
import { Colors } from '~/types/ui/colors'
import type { ApiFilter } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter'

useHead({ title: 'MeldRx Directory' });
definePageMeta({ anonymous: true, layout: 'blank' });
const { $api } = useNuxtApp()

const tableHeaders = [
    { title: 'Practice Name', orderBy: 'practice' },
    { title: 'EHR Vendor', orderBy: 'ehr' },
    'Certified Providers',
    'Actions'
]
const state = reactive<{
  isLoading: boolean,
  directories?: PagedResult<DirectoryTableView>,
  apiFilter: ApiFilter,
}>({
    isLoading: false,
    apiFilter: {
        page: 1,
        orderBy: 'practice'
    }
})

useQuerySync(() => state.apiFilter)

async function loadDirectories (apiFilter: ApiFilter) {
    state.isLoading = true;
    try {
        state.directories = await $api.directories.list(apiFilter)
        state.apiFilter = apiFilter
        state.apiFilter.page = state.directories?.currentPage
    } finally {
        state.isLoading = false;
    }
}

loadDirectories(state.apiFilter)
</script>

<template>
  <DsLoadingOverlay :loading="state.isLoading" />
  <DsContainer class="space-y-5">
    <div class="flex gap-5">
      <div class="flex items-center">
        <svg width="80" height="80" viewBox="0 0 80 80" fill="none" xmlns="http://www.w3.org/2000/svg">
          <g clip-path="url(#clip0_7856_15650)">
            <path d="M40 80C62.0912 80 80 62.0912 80 40C80 17.9086 62.0912 0 40 0C17.9086 0 0 17.9086 0 40C0 62.0912 17.9086 80 40 80Z" fill="#095E86" />
            <path d="M72.2582 40.3209L65.1028 32.1781L66.0999 21.4085L55.5134 19.0153L49.9709 9.67578L40.0004 13.9369L30.0296 9.67578L24.487 18.9861L13.9005 21.3501L14.8976 32.1489L7.74219 40.3209L14.8976 48.4637L13.9005 59.2624L24.487 61.6558L30.0296 70.9662L40.0004 66.6756L49.9709 70.9368L55.5134 61.6267L66.0999 59.2333L65.1028 48.4637L72.2582 40.3209ZM34.399 54.0968L23.2554 42.9768L27.5955 38.6575L34.399 45.4578L51.5544 28.3256L55.8946 32.6451L34.399 54.0968Z" fill="#9AC8DD" />
          </g>
          <defs>
            <clipPath id="clip0_7856_15650">
              <rect width="80" height="80" fill="white" />
            </clipPath>
          </defs>
        </svg>
      </div>
      <div class="flex flex-col gap-3">
        <div>
          <DsText size="2xl" weight="light">
            MeldRx Directory
          </DsText>
        </div>
        <div>
          <DsText size="sm" weight="light">
            This directory lists all of the FHIR APIs and providers that are HTI-1 certified by MeldRx. <br>
            For issues with out-of-date or inaccurate data,
            <DsLink new-tab class="text-secondary" href="https://support.meldrx.com/">contact support</DsLink>.
          </DsText>
        </div>
      </div>
    </div>

    <DsButton
      :color="Colors.secondary"
      variant="filled"
      @click="externalNavigation.downloadFhirDirectoryBundle"
    >
      View as FHIR Bundle
    </DsButton>

    <div class="w-full">
      <DsTextInput
        id="directory-search"
        v-model="state.apiFilter.filter"
        placeholder="Search practice, EHR vendor, provider name"
        @enter-pressed="() => loadDirectories(state.apiFilter)"
      >
        <template #right>
          <DsButton
            :color="Colors.white"
            :text-color='Colors.gray'
            variant="outline"
            @click="() => loadDirectories(state.apiFilter)"
          >
            Search
            <DsIcon name='heroicons:magnifying-glass' />
          </DsButton>
        </template>
      </DsTextInput>
    </div>

    <DsTable
      v-if="state.directories"
      :headers="tableHeaders"
      :items="state.directories.resources"
      :id-selector="item => item.id"
      :api-filter="state.apiFilter"
      @update:api-filter="loadDirectories"
    >
      <template #default="{item}">
        <div>
          <div>
            <DsLink :href="`/directory/${item.id}`">
              <DsText size="xl" weight="light" underline>
                {{ item.practiceName }}
              </DsText>
            </DsLink>
          </div>

          <DsTextWithCopyButton
            size="xs"
            :text="`Workspace URL: ${item.workspaceUrl}`"
            :text-to-copy="item.workspaceUrl"
            show-toast-on-copy
            toast-message-on-copy="Copied Workspace URL to clipboard."
          />
        </div>

        <DsText weight="light">
          {{ item.ehrVendor ?? 'None Specified' }}
        </DsText>

        <DsText weight="light" class="provider-count">
          {{ item.providerCount }}
        </DsText>

        <div class="flex justify-start gap-2">
          <DsButton
            size="sm"
            :color="Colors.white"
            :text-color='Colors.gray'
            variant="outline"
            @click="navigateTo(`/directory/${item.id}`)"
          >
            <DsIcon name='heroicons:arrow-small-right'/>
            Details
          </DsButton>
        </div>
      </template>
      <template #footer>
        <DsTablePager
          v-if="state.directories.totalPages > 1"
          class="pl-6"
          :paged-result-info="state.directories"
          @go-to-page="page => loadDirectories({...state.apiFilter, page})"
        />
      </template>
    </DsTable>
  </DsContainer>
</template>
