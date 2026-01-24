<script setup lang="ts">
import { Colors } from '~/types/ui/colors'
import type { WorkspaceProviderDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceProviderDto';
import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult';
import type { UpdateWorkspaceProviderInfoCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceProviders/UpdateWorkspaceProviderInfoCommand'
import type { ApiFilter } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter'
import type { DirectorySelectorView } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDirectory/DirectorySelectorView'
import type { CreateWorkspaceProvidersFromNpisCommand } from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceProviders/CreateWorkspaceProvidersFromNpisCommand";
import type { CreateWorkspaceProviderCommand } from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceProviders/CreateWorkspaceProviderCommand";
import AddProviderWithoutNpiModal from "~/components/providers/AddProviderWithoutNpiModal.vue";

definePageMeta({ layout: 'workspace', middleware: ['require-workspace'] });
useHead({ title: 'Workspace Providers | MeldRx' });

const { $api } = useNuxtApp()
const route = useRoute()

const { permissions, isAdmin } = useAuth()
const workspaceSlug = ref<string>(route.params.workspaceSlug as string)
const state = reactive<{
  isLoading: boolean;
  openModal: boolean;
  openNpisModal: boolean;
  openProviderModal: boolean;
  workspaceProviders?: PagedResult<WorkspaceProviderDto>;
  directories: DirectorySelectorView[];
  apiFilter: ApiFilter;
  editProvider?: WorkspaceProviderDto;
}>({
    isLoading: false,
    openModal: false,
    openNpisModal: false,
    openProviderModal: false,
    workspaceProviders: undefined,
    directories: [],
    apiFilter: {
        page: 1
    }
});

useQuerySync(() => state.apiFilter)

const canCreateWorkspaceProviders = computed(() =>
    !!permissions.value.developerPermissionsDto?.canCreateDeveloperWorkspaces ||
    !!permissions.value.developerPermissionsDto?.canSellWorkspaces ||
    isAdmin()
)

const tableHeaders = [
    { title: 'NPI', orderBy: 'npi' },
    { title: 'Provider Name', orderBy: 'name' },
    { title: 'Status', orderBy: 'status' },
    { title: 'Date Added', orderBy: 'added' },
    'Date of Activation',
    { title: 'Actions', class: 'text-center' }
];

const { subscriptionProvidersLimitReached, subscription, loadSubscription } = useSubscription()

function getSupportEmail()
{
    const subject = `Increase Provider Limit for ${subscription.value?.organizationName}`;
    const body = `Please increase the number of providers for ${subscription.value?.organizationName} from ${subscription.value?.includedProviders} to NEW_NUMBER_OF_PROVIDERS`;
    return `mailto:support@meldrx.com?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`;
}

function getDirectoryLink()
{
    return `/workspaces/${workspaceSlug.value}/directory`;
}

async function loadProvidersByWorkspaceSlug(filter: ApiFilter) {
    state.isLoading = true;
    try {
        state.workspaceProviders = await $api.workspaces.providers.list(workspaceSlug.value, filter);
        state.apiFilter = filter;
        state.apiFilter.page = state.workspaceProviders?.currentPage ?? 1;
    } catch (error) {
        handleApiError(error, 'Unable to load providers.')
    } finally {
        state.isLoading = false;
    }
}

async function addProviders(command: CreateWorkspaceProvidersFromNpisCommand) {
    state.isLoading = true;
    try {
        await $api.workspaces.providers.createFromNpis(workspaceSlug.value, command);
        await loadProvidersByWorkspaceSlug(state.apiFilter)
        state.openNpisModal = false
        loadSubscription()
    } catch (error) {
        handleApiError(error, 'Unable to add provider')
    } finally {
        state.isLoading = false;
    }
}
async function addProviderManually(command: CreateWorkspaceProviderCommand) {
    state.isLoading = true;
    try {
        await $api.workspaces.providers.createFromCommand(workspaceSlug.value, command);
        await loadProvidersByWorkspaceSlug(state.apiFilter)
        state.openProviderModal = false
        loadSubscription()
    } catch (error) {
        handleApiError(error, 'Unable to add provider')
    } finally {
        state.isLoading = false;
    }
}
async function updateStatus(providerId:string, newStatus: boolean) {
    state.isLoading = true;
    try {
        await $api.workspaces.providers.updateStatus(workspaceSlug.value, { providerId, active: newStatus });
        await loadProvidersByWorkspaceSlug(state.apiFilter)
    } catch (error) {
        handleApiError(error, 'Unable to update provider status')
    } finally {
        state.isLoading = false;
    }
}
async function deleteProvider(providerId:string) {
    state.isLoading = true;
    try {
        await $api.workspaces.providers.delete(workspaceSlug.value, providerId);
        await loadProvidersByWorkspaceSlug(state.apiFilter)
    } catch (error) {
        handleApiError(error, 'Unable to delete provider')
    } finally {
        state.isLoading = false;
    }
}
async function updateInfo(command: UpdateWorkspaceProviderInfoCommand) {
    state.isLoading = true;
    try {
        await $api.workspaces.providers.updateInfo(workspaceSlug.value, command);
        await loadProvidersByWorkspaceSlug(state.apiFilter)
        state.editProvider = undefined
    } catch (error) {
        handleApiError(error, 'Unable to update provider info')
    } finally {
        state.isLoading = false;
    }
}

loadProvidersByWorkspaceSlug(state.apiFilter);
$api.workspaces.directories.listAll(workspaceSlug.value).then(directories => state.directories = directories)
</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="state.isLoading" />

    <div class="space-y-5">
      <DsText size="2xl" weight="light">
        Workspace Providers
      </DsText>

      <div>
        <DsText size="sm" weight="light">
          Manage the providers that will appear in the MeldRx directory for this workspace.
          For customers on an NPI licensing model,
          your workspace will not be active until the NPIs have been properly configured.
        </DsText>

        <br>

        <DsText v-if="!canCreateWorkspaceProviders" size="sm" color="text-fire">
          You are only allowed to view this list.
        </DsText>
      </div>

      <div class="w-full">
        <DsTextInput
          v-model="state.apiFilter.filter"
          placeholder="Search provider by name or npi"
          @enter-pressed="() => loadProvidersByWorkspaceSlug(state.apiFilter)"
        >
          <template #right>
            <DsButton :color="Colors.white" :text-color='Colors.gray' variant="outline" @click="() => loadProvidersByWorkspaceSlug(state.apiFilter)" >
              Search
              <DsIcon name='heroicons:magnifying-glass'/>
            </DsButton>
          </template>
        </DsTextInput>
      </div>
      <div v-if="canCreateWorkspaceProviders" class="flex gap-4">
        <DsButton
            :color="Colors.primary"
            :text-color='Colors.gray'
            :disabled="!canCreateWorkspaceProviders || subscriptionProvidersLimitReached || state.directories.length === 0"
            variant="filled"
            @click="() => state.openNpisModal = true"
        >
          <DsIcon name="heroicons:plus" size='sm' />
          Add Provider(s)
        </DsButton>
        <DsButton
            v-if="subscription.allowNpiOverride"
            :color="Colors.secondary"
            :text-color='Colors.secondary'
            :disabled="!canCreateWorkspaceProviders || subscriptionProvidersLimitReached || state.directories.length === 0"
            variant="subtle"
            @click="() => state.openProviderModal = true"
        >
          <DsIcon name="heroicons:plus" size='sm' />
          Add NPI Without Validation
        </DsButton>
        <DsBanner v-if="subscriptionProvidersLimitReached" icon='heroicons:information-circle' :color='Colors.fire' :href="getSupportEmail()" class='pl-5'>
          You have reached your Provider limit.
          <DsLink :href='getSupportEmail()'>Contact support to increase your limit.</DsLink>
        </DsBanner>
        <DsBanner v-else-if="state.directories.length === 0" icon='heroicons:information-circle' icon-size='xl' :color='Colors.fire' class='pl-5'>
          <DsLink :href='getDirectoryLink()' underline="hover">You need at least one directory listing to add a provider.</DsLink>
        </DsBanner>
      </div>

      <AddProvidersFromNpisModal
        v-if="canCreateWorkspaceProviders && state.openNpisModal"
        :show="state.openNpisModal"
        :workspace-slug="workspaceSlug"
        :directories="state.directories"
        @create="addProviders"
        @close="() => state.openNpisModal = false"
      />

      <AddProviderWithoutNpiModal
          v-if="canCreateWorkspaceProviders && state.openProviderModal"
          :show="state.openProviderModal"
          :workspace-slug="workspaceSlug"
          :directories="state.directories"
          @create="addProviderManually"
          @close="() => state.openProviderModal = false"
      />

      <EditProviderInfo
        :provider="state.editProvider"
        :directories="state.directories"
        @close="state.editProvider = undefined"
        @update="updateInfo"
      />

      <DsTable
        v-if="state.workspaceProviders"
        :headers="tableHeaders"
        :items="state.workspaceProviders.resources"
        :id-selector="item => item.id"
        :api-filter="state.apiFilter"
        @update:api-filter="loadProvidersByWorkspaceSlug"
      >
        <template #default="{item}">
          <DsText size="md" weight="light">
            {{ item.npi }}
          </DsText>

          <DsText size="md" weight="light">
            {{ item.displayName }}
          </DsText>

          <DsText size="md" weight="light">
            {{ item.active ? 'Active' : 'Inactive' }}
          </DsText>

          <DsText size="md" weight="light">
            {{ item.createdOn }}
          </DsText>

          <DsText size="md" weight="light">
            {{ item.dateOfActivation }}
          </DsText>

          <div class="flex justify-center gap-2">
            <template v-if="item.active">
              <DsButton
                :id="`edit-${item.npi}-button`"
                :disabled="!canCreateWorkspaceProviders"
                :color="Colors.white"
                :text-color='Colors.gray'
                size="sm"
                variant="outline"
                @click="state.editProvider = item"
              >
                Edit
              </DsButton>
              <DsConfirmation confirmation-message="Deactivate NPI" @confirm="updateStatus(item.id, false)">
                <template #default="{show}">
                  <DsButton
                    :id="`deactivate-${item.npi}-button`"
                    :disabled="!canCreateWorkspaceProviders"
                    :color="Colors.fire"
                    :text-color='Colors.fire'
                    size="sm"
                    variant="outline"
                    @click="show"
                  >
                    <DsIcon name='heroicons:x-mark'/>
                  Deactivate
                </DsButton>
                </template>
                <template #message>
                  <DsText>
                    Are you sure you want to deactivate
                    <DsText weight="bold">
                      {{ item.displayName }}
                    </DsText>
                    from this workspace?
                  </DsText>
                </template>
              </DsConfirmation>
            </template>
            <template v-else>
              <DsButton
                :id="`reactivate-${item.npi}-button`"
                :disabled="!canCreateWorkspaceProviders"
                :color="Colors.white"
                :text-color='Colors.gray'
                size="sm"
                variant="outline"
                @click="updateStatus(item.id,true)"
              >
                Reactivate
              </DsButton>
            </template>
            <template v-if="isAdmin()">
              <DsConfirmation confirmation-message="Delete NPI" @confirm="deleteProvider(item.id)">
                <template #default="{show}">
                  <DsButton :id="`delete-${item.npi}-button`" :color="Colors.fire" :text-color='Colors.fire' size="sm" variant="outline" @click="show">
                    <DsIcon name='heroicons:x-mark'/>
                    Delete
                  </DsButton>
                </template>
                <template #message>
                  <DsText>
                    Are you sure you want to delete
                    <DsText weight="bold">
                      {{ item.displayName }}
                    </DsText>
                    from this workspace?
                  </DsText>
                </template>
              </DsConfirmation>
            </template>
          </div>
        </template>
        <template #footer>
          <DsTablePager
              v-if="state.workspaceProviders.totalPages > 1"
              class="pl-6"
              :paged-result-info="state.workspaceProviders"
              @go-to-page="(page) => loadProvidersByWorkspaceSlug({...state.apiFilter, page})"
          />
        </template>
      </DsTable>
    </div>
  </DsContainer>
</template>
