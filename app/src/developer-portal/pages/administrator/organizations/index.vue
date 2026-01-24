<script setup lang="ts">
import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult';
import type { ApiFilter } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter';
import type { OrganizationDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/OrganizationDto';
import { Colors } from '~/types/ui/colors';
import { OrganizationType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/OrganizationType';
import type { DeleteOrganizationResourcesMessage } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DeleteOrganizationResourcesMessage';

useHead({ title: 'Organization Admin | MeldRx' });

const { $api } = useNuxtApp();
const confirmation = useConfirmation();

const state = reactive<{
  isLoading: boolean;
  organizations?: PagedResult<OrganizationDto>;
  resources?: {
    id: string,
    data: DeleteOrganizationResourcesMessage
  };
  apiFilter: ApiFilter;
}>({
    isLoading: false,
    organizations: undefined,
    apiFilter: {
        page: 1,
        orderBy: 'name'
    }
});

useQuerySync(() => state.apiFilter);

const tableHeaders = [
    {
        title: 'Name',
        orderBy: 'name'
    },
    {
        title: 'Type',
        orderBy: 'type'
    },
    'Actions'
];

async function loadOrganizations (apiFilter: ApiFilter) {
    state.isLoading = true;

    try {
        state.organizations = await $api.admin.organizations.search(apiFilter);
        state.apiFilter = apiFilter;
        state.apiFilter.page = state.organizations?.currentPage ?? 1;
    } catch (error) {
        handleApiError(error, 'Unable to load organizations');
    }

    state.isLoading = false;
}

async function viewOrganizationApps (orgId: string) {
    navigateTo(`/administrator/apps?orgId=${orgId}`);
}

async function viewOrganizationWorkspaces (orgId: string) {
    navigateTo(`/administrator/workspaces?orgId=${orgId}`);
}

async function openInIdentity (orgId: string) {
    navigateTo({ path: location.origin + `/Admin/Organization/OrganizationProfile/${orgId}` }, { external: true });
}

async function previewDeleteResources(orgId: string) {
    state.isLoading = true;

    try {
        state.resources = {
            id: orgId,
            data: await $api.admin.organizations.previewDelete(orgId)
        };
    } catch (error) {
        handleApiError(error, 'Unable to load organization resources');
    }

    state.isLoading = false;
}

async function deleteOrganization(orgId: string) {
    const { isCancelled } = await confirmation(
        'Are you sure you want to delete this reseller organization? All resources that belong to this organization will be deleted. This action cannot be undone.',
        'Delete Organization'
    );
    if (isCancelled) {
        return;
    }

    state.isLoading = true;

    try {
        await $api.admin.organizations.delete(orgId);
        successNotification('Request accepted, organization will be deleted in the background.')
    } catch (error) {
        handleApiError(error, 'Failed to start resource deletion');
    } finally {
        state.resources = undefined;
        state.isLoading = false;
    }
}

loadOrganizations(state.apiFilter);
</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="state.isLoading" />
    <div class="w-full">
      <DsText size="2xl" weight="light">
        Organizations
      </DsText>
      <div class="pb-5" />
      <DsTextInput
        id="search-organization"
        v-model="state.apiFilter.filter"
        placeholder="Search organization name"
        @enter-pressed="() => loadOrganizations(state.apiFilter)"
      >
        <template #right>
          <DsButton :color="Colors.white" :text-color='Colors.gray' variant="outline" @click="() => loadOrganizations(state.apiFilter)">
            Search
            <DsIcon name="heroicons:magnifying-glass"/>
          </DsButton>
        </template>
      </DsTextInput>
    </div>
    <div class="pb-5" />
    <DsTable
      v-if="state.organizations" :headers="tableHeaders" :items="state.organizations.resources"
      :id-selector="item => item.id" :api-filter="state.apiFilter" @update:api-filter="loadOrganizations"
    >
      <template #default="{ item }">
        <div class="flex-col">
          <DsText size="lg" weight="light">
            {{ item.name }}
          </DsText>
          <DsBadge
            v-if="item.hasMipsReports"
            class="ml-1.5"
            :color="Colors.mulberry50"
            :text-color="Colors.white"
            size="xs"
            label="Mips Reports"
            >
            Mips Reports
          </DsBadge>
          <DsTextWithCopyButton
            size="xs" :text="`Organization ID: ${item.id}`" :text-to-copy="item.id"
            :show-toast-on-copy="true" toast-message-on-copy="Copied organization ID to clipboard."
          />
        </div>
        <DsText size="md" weight="light">
          {{ item.type }}
        </DsText>
        <div class="flex gap-2">
          <DsButton
            :id="`view-apps-${item.name?.replaceAll(' ', '-').toLowerCase()}-button`"
            :color="Colors.primary"
            :text-color='Colors.primary'
            variant="outline"
            @click="viewOrganizationApps(item.id)"
          >
            View Apps
          </DsButton>
          <DsButton
            :id="`view-workspaces-${item.name?.replaceAll(' ', '-').toLowerCase()}-button`"
            :color="Colors.primary"
            :text-color='Colors.primary'
            variant="outline"
            @click="viewOrganizationWorkspaces(item.id)"
          >
            View Workspaces
          </DsButton>
          <DsButton
            :id="`open-${item.name?.replaceAll(' ', '-').toLowerCase()}-button`"
            :color="Colors.secondary"
            :text-color='Colors.secondary'
            variant="outline"
            @click="openInIdentity(item.id)"
          >
            Open in Identity
          </DsButton>
          <DsButton
            v-if="item.type === OrganizationType.Developer || item.type === OrganizationType.Provider"
            :id="`delete-${item.name?.replaceAll(' ', '-').toLowerCase()}-button`"
            :color="Colors.fire"
            :text-color='Colors.fire'
            variant="outline"
            @click="previewDeleteResources(item.id)"
          >
            Delete
          </DsButton>
        </div>
      </template>
      <template #footer>
        <DsTablePager
          class="pl-6" :paged-result-info="state.organizations"
          @go-to-page="(page) => loadOrganizations({ ...state.apiFilter, page })"
        />
      </template>
    </DsTable>
  </DsContainer>
  <DsDrawer v-if="state.resources !== undefined" show>
    <template #header>
      Organization Resources Delete Preview
    </template>
    <template #default>
      <div>
        <DsText size="lg" weight="bold">Organizations</DsText>
        <ul class="list-disc list-inside">
          <li v-for="org in state.resources.data.organizations" :key="`organization-${org.id}`">
            {{ org.id }} - {{ org.name }}
          </li>
        </ul>
        <DsText size="lg" weight="bold">Subscriptions</DsText>
        <ul class="list-disc list-inside">
          <li v-for="subscription in state.resources.data.subscriptions" :key="`subscription-${subscription.id}`">
            {{ subscription.id }} ({{ subscription.type }})
          </li>
        </ul>
        <DsText size="lg" weight="bold">Workspaces</DsText>
        <ul class="list-disc list-inside">
          <li v-for="workspace in state.resources.data.workspaces" :key="`workspace-${workspace.id}`">
            {{ workspace.name }}
          </li>
        </ul>
        <DsText size="lg" weight="bold">Apps</DsText>
        <ul class="list-disc list-inside">
          <li v-for="app in state.resources.data.apps" :key="`app-${app.id}`">
            {{ app.name }} - {{ app.clientId }}
          </li>
        </ul>
        <DsText size="lg" weight="bold">Users</DsText>
        <ul class="list-disc list-inside">
          <li v-for="user in state.resources.data.users" :key="`user-${user.id}`">
            {{ user.email }}
          </li>
        </ul>

        <template v-if="state.resources.data.organizationsWithMipsReport.length > 0">
          <DsText size="lg" weight="bold">Mips Organizations (not deleted)</DsText>
          <ul class="list-disc list-inside">
            <li v-for="org in state.resources.data.organizationsWithMipsReport" :key="`organization-mips-${org.id}`">
              {{ org.id }} - {{ org.name }}
            </li>
          </ul>
        </template>

        <template v-if="state.resources.data.mipsUsers.length > 0">
          <DsText size="lg" weight="bold">Mips Users (not deleted)</DsText>
          <ul class="list-disc list-inside">
            <li v-for="user in state.resources.data.mipsUsers" :key="`organization-mips-${user.id}`">
              {{ user.email }}
            </li>
          </ul>
        </template>

      </div>
    </template>
    <template #footer>
      <div class="space-x-2">
        <DsButton
          id="close-resources-button"
          :color="Colors.gray"
          :text-color="Colors.gray"
          variant="subtle"
          @click="state.resources = undefined"
        >
          Close
        </DsButton>
        <DsButton
          id="delete-resources-button"
          :color="Colors.fire"
          :text-color="Colors.fire"
          variant="outline"
          @click="deleteOrganization(state.resources.id)"
        >
          Delete
        </DsButton>
      </div>
    </template>
  </DsDrawer>
</template>
