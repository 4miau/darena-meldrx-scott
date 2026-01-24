<script setup lang="ts">
import type { WorkspaceDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto';
import { Colors } from '~/types/ui/colors';
import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'
import type { ApiFilter } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter'
import { FhirServerType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/FhirServerType';

definePageMeta({
    middleware: ['admin-validation']
})

useHead({ title: 'Workspaces | MeldRx' });
const { $api } = useNuxtApp();
const { permissions } = useAuth()
const { subscription, subscriptionWorkspaceLimitReached } = useSubscription()

const state = reactive<{
  isLoading: boolean;
  workspaces?: PagedResult<WorkspaceDto>;
  apiFilter: ApiFilter;
}>({
    isLoading: false,
    workspaces: undefined,
    apiFilter: {
        page: 1,
        orderBy: 'name'
    }
});

useQuerySync(() => state.apiFilter)

function getSupportEmail()
{
    const subject = `Increase Workspace Limit for ${subscription.value?.organizationName}`;
    const body = `Please increase the number of workspaces for ${subscription.value?.organizationName} from ${subscription.value?.includedWorkspaces} to NEW_NUMBER_OF_WORKSPACES`;
    return `mailto:support@meldrx.com?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`;
}

const canCreateWorkspace = computed(() =>
    !!permissions.value.developerPermissionsDto?.canCreateDeveloperWorkspaces ||
    !!permissions.value.developerPermissionsDto?.canSellWorkspaces
)

const tableHeaders = [
    { title: 'Workspace Name', orderBy: 'name' },
    { title: 'Type', orderBy: 'type' },
    'Actions'
];

// Load workspaces from server...
async function loadWorkspaces(apiFilter: ApiFilter) {
    state.isLoading = true;

    try {
        state.workspaces = await $api.workspaces.list(apiFilter);
        state.apiFilter = apiFilter;
        state.apiFilter.page = state.workspaces?.currentPage ?? 1;
    } catch (error) {
        handleApiError(error, 'Unable to load workspaces');
    }

    state.isLoading = false;
}

// "Create Workspace"...
async function onCreateNewWorkspace() {
    await navigateTo('/workspaces/new');
}

loadWorkspaces(state.apiFilter);
</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="state.isLoading" />
    <div class="space-y-5">
      <DsText size="2xl" weight="light">
        Workspaces
      </DsText>

      <div>
        <DsText size="sm" weight="light">
          A MeldRx workspace is a fully-loaded FHIR server with additional capabilities.
          Linked workspaces interact and store data for other FHIR interfaces such as an EHR.
          Standalone workspaces are FHIR servers ready for independent use, capable of handling FHIR resources,
          managing patient data, and supporting healthcare applications without needing integration with an existing EHR.
        </DsText>

        <br>

        <DsText v-if="subscription.subscriptionType === 'developer'" size="sm" :color="Colors.fire">
          You are currently on a developer subscription, which is intended solely for testing, development, and evaluation purposes.
          This subscription type does not permit the use of the service for storing Protected Health Information (PHI).
          If your project requires the handling of PHI please consider upgrading to one of our paid subscriptions.
        </DsText>
      </div>

      <div class="w-full">
        <DsTextInput
          v-model="state.apiFilter.filter"
          placeholder="Search workspace name"
          @enter-pressed="() => loadWorkspaces(state.apiFilter)"
        >
          <template #right>
            <DsButton
              :color="Colors.white"
              :text-color='Colors.gray'
              variant="outline"
              @click="() => loadWorkspaces(state.apiFilter)"
            >
              Search
              <DsIcon name="heroicons:magnifying-glass" />
            </DsButton>
          </template>
        </DsTextInput>
      </div>

      <div class="flex">
        <DsButton
          :color="Colors.primary"
          :text-color="Colors.white"
          :disabled="!canCreateWorkspace || subscriptionWorkspaceLimitReached"
          variant="filled"
          @click="onCreateNewWorkspace"
        >
          <DsIcon name="heroicons:plus" />
          Create Workspace
        </DsButton>
        <DsBanner v-if='subscriptionWorkspaceLimitReached' icon='heroicons:information-circle' :color='Colors.fire' class='pl-5'>
            You have reached your Workspace limit.<br>
            <DsLink underline="hover" :href="getSupportEmail()" target='_blank'>
              Upgrade your subscription to create more.
            </DsLink>
        </DsBanner>
      </div>

      <DsTable
        v-if="state.workspaces"
        :headers="tableHeaders"
        :items="state.workspaces.resources"
        :id-selector="item => item.id"
        :api-filter="state.apiFilter"
        @update:api-filter="loadWorkspaces"
      >
        <template #default="{item}">
          <div class="flex-col">
            <DsText size="lg" weight="light" class="inline-flex items-center">
              <DsLink underline="always" :href="`/workspaces/${item.fhirDatabaseDisplayName}`">
                {{ item.name }}
              </DsLink>
              <DsBadge
                  v-if="item.isLiteWorkspace"
                  class="ml-1.5"
                  size="xs"
                  :color="Colors.lapisLazuli50"
                  :text-color="Colors.white"
              >
                Lite
              </DsBadge>
              <DsBadge
                  v-if="item.isSandbox"
                  class="ml-1.5"
                  :color="Colors.fire50"
                  :text-color="Colors.white"
                  size="xs"
                  label="Sandbox"
              >
                Sandbox
              </DsBadge>
              <DsBadge
                v-if="item.type === FhirServerType.Virtual"
                class="ml-1.5"
                size="xs"
                :color="Colors.lapisLazuli50"
                :text-color="Colors.white"
              >
                Virtual
              </DsBadge>
            </DsText>
            <DsTextWithCopyButton
              size="xs"
              :text="`Workspace URL: ${item.fhirServerEndpoint}`"
              :text-to-copy="item.fhirServerEndpoint"
              :show-toast-on-copy="true"
              toast-message-on-copy="Copied Workspace URL to clipboard."
            />
          </div>

          <DsText size="md" weight="light">
            {{ !!item.linkedFhirApiDto ? 'Linked' : 'Standalone' }}
          </DsText>

          <DsButton
            :id="`select-${item.name.replaceAll(' ', '-').toLowerCase()}-button`"
            :color="Colors.white"
            :text-color="Colors.gray"
            variant="outline"
            size="md"
            @click="() => navigateTo(!!item.linkedFhirApiDto ? `/workspaces/${item.fhirDatabaseDisplayName}` : `/workspaces/${item.fhirDatabaseDisplayName}/patients`)"
          >
            Select
            <DsIcon name="heroicons:arrow-small-right" />
          </DsButton>
        </template>
        <template #footer>
          <DsTablePager
            class="pl-6"
            :paged-result-info="state.workspaces"
            @go-to-page="(page) => loadWorkspaces({...state.apiFilter, page})"
          />
        </template>
      </DsTable>
    </div>
  </DsContainer>
</template>
