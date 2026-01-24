<script setup lang="ts">
import type { DirectoryListingDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DirectoryListingDto';
import { Colors } from '~/types/ui/colors';
import type { ApiFilter } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter'
import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'

definePageMeta({ layout: 'workspace', middleware: ['require-workspace'], refreshWorkspace: true });
useHead({ title: 'Directory Settings | MeldRx' })

const { $api } = useNuxtApp()


const { workspace } = useWorkspace()

const state = reactive<{
  isLoading: boolean;
  directories?: PagedResult<DirectoryListingDto>;
  form?: DirectoryListingDto;
  apiFilter: ApiFilter;
}>({
    isLoading: false,
    apiFilter: {
        page: 1
    }
})

useQuerySync(() => state.apiFilter)

const tableHeaders = [
    { title: 'Display Name', orderBy: 'name' },
    { title: 'Display', orderBy: 'display' },
    { title: 'Directory Visibility', orderBy: 'status' },
    'Actions'
];

// Load directory listing settings...
async function loadDirectoryListings (filter: ApiFilter) {
    state.isLoading = true
    try {
        state.directories = await $api.workspaces.directories.list(workspace.value!.fhirDatabaseDisplayName, filter)
        state.apiFilter = filter
        state.apiFilter.page = state.directories.currentPage
    } catch (error) {
        handleApiError(error, 'Unable to load directory listings');
    }
    state.isLoading = false
}

function onAddDirectoryListingModalClick(listing?: DirectoryListingDto) {
    if (listing) {
        state.form = listing
    } else {
        state.form = {
            address1: '',
            address2: '',
            city: '',
            active: false,
            displayName: '',
            state: '',
            zip: ''
        }
    }
}
async function createEditDirectory (directoryListing: DirectoryListingDto) {
    state.isLoading = true

    if (workspace.value?.organizationId) {
        directoryListing.organizationId = workspace.value?.organizationId;
    }

    if (directoryListing.id) {
        try {
            await $api.workspaces.directories.update(workspace.value!.fhirDatabaseDisplayName, directoryListing)
        } catch (error) {
            handleApiError(error, 'Unable to update directory listing');
        }
    }
    else {
        try {
            await $api.workspaces.directories.create(workspace.value!.fhirDatabaseDisplayName, directoryListing)
        } catch (error) {
            handleApiError(error, 'Unable to create directory listing');
        }
    }

    state.isLoading = false
    await loadDirectoryListings(state.apiFilter)
}

async function changeDisplayStatus(directoryListing: DirectoryListingDto, newStatus: boolean) {
    if (!directoryListing.id) {
        notification({ title: 'Error', description: 'Unable to find directory listing', displayTime: 3000, variant: 'error' });
        return
    }

    state.isLoading = true

    try {
        await $api.workspaces.directories.updateStatus(workspace.value!.fhirDatabaseDisplayName, directoryListing.id, newStatus)
    } catch (error) {
        handleApiError(error, 'Unable to update directory listing. This feature is enabled for Enterprise subscriptions only.');
    }
    state.isLoading = false
    await loadDirectoryListings(state.apiFilter)
}

loadDirectoryListings(state.apiFilter)
</script>

<template>
  <DsContainer class="space-y-5">
    <DsLoadingOverlay :loading="state.isLoading" />

    <AddDirectoryModal
      v-if="state.form"
      :directory-listing="state.form"
      @close="() => state.form = undefined"
      @save="(v) => createEditDirectory(v)"
    />

    <!-- Header Text -->
    <DsText size="2xl" weight="light">
      Directory Settings
    </DsText>
    <DsText size="sm" weight="light" class="block">
      Edit the details as they appear in the Public Directory and places such as HealthIT.gov Lantern.
    </DsText>

    <div class="space-y-5">
      <div class="flex space-x-5">
        <!-- Add Directory Listing -->
        <DsButton :color="Colors.primary" variant="filled" @click="onAddDirectoryListingModalClick()">
          + Add Directory Listing
        </DsButton>
      </div>
      <div>
        <DsTable
          v-if="state.directories"
          :headers="tableHeaders"
          :items="state.directories.resources"
          :id-selector="item => item.id!"
          :api-filter="state.apiFilter"
          @update:api-filter="loadDirectoryListings"
        >
          <template #default="{item}">
            <DsText size="md" weight="light">
              {{ item?.displayName }}
            </DsText>

            <DsText size="md" weight="light">
              {{ item?.displayAddress }}
            </DsText>

            <DsText size="md" weight="light">
              {{ item.active ? 'Shown' : 'Hidden' }}
              <DsToggle
                :id="`toggle-${item.displayName}-button`"
                :model-value="item.active"
                @update:model-value="changeDisplayStatus(item, $event)"
              />
            </DsText>

            <DsButton
              :id="`edit-${item.displayName}-button`"
              @click="onAddDirectoryListingModalClick(item)"
            >
              Edit
            </DsButton>
          </template>
          <template #footer>
            <DsTablePager
              v-if="state.directories.totalPages > 1"
              class="pl-6"
              :paged-result-info="state.directories"
              @go-to-page="(page) => loadDirectoryListings({...state.apiFilter, page})"
            />
          </template>
        </DsTable>
      </div>
    </div>
  </DsContainer>
</template>
