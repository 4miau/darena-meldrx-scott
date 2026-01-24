<script setup lang="ts">
import {Colors} from "~/types/ui/colors";
import type {PagedResult} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult";
import type {ApiFilter} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter";
import type {AppDto} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/App";
import type {OrganizationDto} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/OrganizationDto";
import type {
    AdminAppUpdateCommand
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/AdminAppUpdateCommand";

useHead({ title: 'Apps Admin | MeldRx' });

const { $api } = useNuxtApp();
const confirmation = useConfirmation();


const router = useRouter();

const state = reactive<{
  isLoading: boolean;
  apps?: PagedResult<AppDto>;
  organizations?: PagedResult<OrganizationDto>;
  apiFilter: ApiFilter;
  organizationApiFilter: ApiFilter;
  showUpdateOrg: boolean;
  selectedApp?: AppDto;
  selectedOrganization: string;
  showEditApp: boolean;
  selectedAppDetails?: AdminAppUpdateCommand
}>({
    isLoading: false,
    apps: undefined,
    organizations: undefined,
    apiFilter: {
        page: 1,
        orderBy: 'name'
    },
    organizationApiFilter: {
        page: 1,
        orderBy: 'name'
    },
    showUpdateOrg: false,
    selectedOrganization: '',
    showEditApp: false,
});

const tableHeaders = [
    { title: 'App Name', orderBy: 'name' },
    'Organization',
    'Actions'
];

async function loadApps(apiFilter: ApiFilter) {
    state.isLoading = true;

    try {
        state.apps = await $api.admin.apps.search(apiFilter, router.currentRoute.value.query.orgId?.toString());
        state.apiFilter = apiFilter;
        state.apiFilter.page = state.apps?.currentPage ?? 1;
    } catch (error) {
        handleApiError(error,  'Unable to load apps');
    }

    state.isLoading = false;
}
function onUpdateOrganizationClick(app: AppDto) {
    state.organizationApiFilter.filter = ''
    state.selectedOrganization = '';
    state.selectedApp = app;
    state.showUpdateOrg = true;
}

async function loadOrganizations(organizationApiFilter: ApiFilter) {
    state.isLoading = true;

    try {
        state.organizations = await $api.admin.organizations.search(organizationApiFilter);
        state.organizationApiFilter = organizationApiFilter;
        state.organizationApiFilter.page = state.organizations?.currentPage ?? 1;
    } catch (error) {
        handleApiError(error, 'Unable to load organizations')
    }

    state.isLoading = false;
}

async function updateAppOrganization(orgId:string) {
    if(!state.selectedApp){
        return
    }
    if(orgId === ''){
        notification({
            title: 'Error',
            description: 'Select an organization.',
            displayTime: 3000,
            variant: 'error'
        })
        return
    }
    state.isLoading = true;

    try {
        await $api.admin.apps.updateOrganizationId(state.selectedApp?.appId, orgId);
        state.showUpdateOrg = false;
        await loadApps(state.apiFilter)
    } catch (error) {
        handleApiError(error, 'Unable to update app organization')
    }

    state.isLoading = false;
}
async function deleteApp(appId:string, orgId:string){
    if(!appId || !orgId){
        handleApiError('Error', 'appId or orgId is missing')
        return
    }

    const {isCancelled} = await confirmation(
        'Are you sure you want to delete this app? This action cannot be undone.',
        'Delete App'
    );
    if (isCancelled) {
        return;
    }
    try {
        await $api.admin.apps.delete(appId, orgId);
        await loadApps(state.apiFilter)
    } catch (error) {
        handleApiError(error, 'Unable to delete app')
    }
}
async function onEditDetailsClick(appId:string){
    try {
        state.selectedAppDetails = await $api.admin.apps.appDetails(appId)
    }catch (error) {
        handleApiError(error, 'Unable to get app details')
    }
    finally {
        state.showEditApp = true;
    }
}

async function saveAppDetails(){
    if(!state.selectedAppDetails){
        return
    }
  
    try {
        const updateCommand:AdminAppUpdateCommand = {
            appId: state.selectedAppDetails?.appId,
            publishedStatus: state.selectedAppDetails.publishedStatus,
            meldRxVerified: state.selectedAppDetails.meldRxVerified,
            meldRxHosted: state.selectedAppDetails.meldRxHosted
        }
        await $api.admin.apps.appDetailsUpdate(updateCommand)
    }catch (error) {
        handleApiError(error, 'Unable to save app details.')
    }
    finally {
        state.showEditApp = false;
    }
}

async function backToOrganizations(){
    navigateTo(`organizations?page=1&orderBy=name&filter=${router.currentRoute.value.query.orgId?.toString()}`)
}

loadApps(state.apiFilter)
</script>

<template>
  <DsContainer class="space-y-5">
    <DsLoadingOverlay :loading="state.isLoading" />
    <div class="w-full space-y-5">
      <DsText size="2xl" weight="light">
        Apps
      </DsText>
      <DsTextInput
        id="search-app"
        v-model="state.apiFilter.filter"
        placeholder="Search app name"
        @enter-pressed="() => loadApps(state.apiFilter)">
        <template #right>
          <DsButton :color="Colors.white" :text-color='Colors.onyx' variant="outline" @click="() => loadApps(state.apiFilter)">
            Search
            <DsIcon name="heroicons:magnifying-glass" />
          </DsButton>
        </template>
      </DsTextInput>
      <DsButton v-if="!!router.currentRoute.value.query.orgId" :color="Colors.white" :text-color='Colors.gray' variant="outline" @click="backToOrganizations">
        Back to Organization
      </DsButton>
    </div>
    <DsTable
      v-if="state.apps" :headers="tableHeaders" :items="state.apps.resources" :id-selector="item => item.appId"
      :api-filter="state.apiFilter" @update:api-filter="loadApps">
      <template #default="{ item }">
        <div class="flex-col">
          <DsText size="lg" weight="light">
            {{ item.appName }}
          </DsText>
          <DsTextWithCopyButton
              size="xs" :text="`App Id: ${item.appId}`" :text-to-copy="item.appId"
              :show-toast-on-copy="true" toast-message-on-copy="Copied Id to clipboard."
          />
        </div>
        <div>
          <div v-if="item.orgName" class="flex-col">
            <DsText size="md" weight="light">
              {{ item.orgName }}
            </DsText>
            <DsTextWithCopyButton
                size="xs" :text="`Org Id: ${item.orgId}`" :text-to-copy="item.orgId"
                :show-toast-on-copy="true" toast-message-on-copy="Copied Id to clipboard."
            />
          </div>
          <DsText v-else size="md" weight="light" :color="Colors.fire">
            No Organization
          </DsText>
        </div>
        <div class="space-x-3">
          <DsButton
              :id="`edit-${item.appName?.replaceAll(' ', '-').toLowerCase()}-button`"
              :color="Colors.white"
              :text-color='Colors.onyx'
              variant="outline"
              @click="() => onEditDetailsClick(item.appId)"
          >
            Edit Details
          </DsButton>
          <DsButton
              :id="`update-${item.appName?.replaceAll(' ', '-').toLowerCase()}-button`"
              :color="Colors.white"
              :text-color='Colors.onyx'
              variant="outline"
              @click="() => onUpdateOrganizationClick(item)"
          >
            {{ (item.orgId ? 'Update' : 'Assign') + ' Organization' }}
          </DsButton>
          <DsButton
              :id="`delete-${item.appName?.replaceAll(' ', '-').toLowerCase()}-button`"
              :disabled="!item.appId || !item.orgId"
              :color="Colors.fire"
              :text-color='Colors.fire'
              variant="outline"
              @click="() => deleteApp(item.appId, item.orgId)"
          >
            Delete
          </DsButton>
        </div>
      </template>
      <template #footer>
        <DsTablePager
          class="pl-6" :paged-result-info="state.apps"
          @go-to-page="(page) => loadApps({ ...state.apiFilter, page })" />
      </template>
    </DsTable>
  </DsContainer>
  
  <DsModal v-if="state.showEditApp && state.selectedAppDetails" v-model="state.showEditApp">
    <DsModalProgressCard :total-steps="1" :current-step="1"/>
    <div class="flex w-full py-8 bg-space justify-center items-center">
      <div class="flex w-full items-center justify-center">
        <div class="w-14 h-14 rounded-full flex justify-center items-center pt-2">
          <MeldRxRabbit class="mb-2" />
        </div>
      </div>
      <div class="flex-1" />
    </div>
    
    <div class="p-4 space-y-3">
      <div class="w-full flex-col justify-center items-center gap-5 flex">
        <DsText size="2xl" weight="light">
          Edit App Details
        </DsText>

        <DsText size="sm" weight="light" class="text-center">
          Change the published status and other details of an app.
        </DsText>
      </div>
      <DsLabeledInput label="Publish Status">
        <DsButtonGroup
            v-model="state.selectedAppDetails.publishedStatus"
            :active-color="Colors.secondary"
            :rounded="false"
        >
          <DsButton value="NotPublished" :text-color='Colors.gray'>
            Not Published
          </DsButton>
          <DsButton value="Published" :text-color='Colors.gray'>
            Published
          </DsButton>
        </DsButtonGroup>
      </DsLabeledInput>
      <DsLabeledInput label="MeldRx Verified">
        <DsToggle v-model="state.selectedAppDetails.meldRxVerified!"/>
      </DsLabeledInput>
      <DsLabeledInput label="MeldRx Hosted">
        <DsToggle v-model="state.selectedAppDetails.meldRxHosted!"/>
      </DsLabeledInput>
      
      <div class="flex justify-center gap-5">
        <DsButton size="md" :text-color="Colors.secondary" variant="subtle" @click="state.showEditApp = false">
          Cancel
        </DsButton>
        <DsButton size="md" @click="saveAppDetails">
          Save
        </DsButton>
      </div>
    </div>
  </DsModal>

  <DsDrawer v-if="state.selectedApp" :show="state.showUpdateOrg" @close="state.showUpdateOrg = false">
    <div class="w-full p-4 space-y-4">
      <DsIcon name="heroicons:x-mark" size="sm" class="absolute top-4 right-4 cursor-pointer" @click="state.showUpdateOrg = false"/>
      <div class="flex flex-col">
        <DsText size="2xl" weight="light">
          Update App Organization
        </DsText>
        <DsText size="md" weight="light">
          <b>Selected App:</b> {{ state.selectedApp.appName }} <br>
          <b>Current Organization:</b> {{ state.selectedApp.orgName }} <br>
        </DsText>
      </div>
      <DsTextInput
          v-model="state.organizationApiFilter.filter" placeholder="Search organization name"
          @enter-pressed="() => loadOrganizations(state.organizationApiFilter)">
        <template #right>
          <DsButton :color="Colors.white" :text-color='Colors.gray' variant="outline" @click="() => loadOrganizations(state.organizationApiFilter)">
            Search
            <DsIcon name="heroicons:magnifying-glass"/>
          </DsButton>
        </template>
      </DsTextInput>
      <DsTable
          v-if="state.organizations"
          :headers="['Organization','Actions']"
          :items="state.organizations.resources"
          :id-selector="item => item.id"
          :api-filter="state.organizationApiFilter"
          @update:api-filter="loadOrganizations"
      >
        <template #default="{ item }">
          <div class="flex-col">
            <DsText size="md" weight="light">
              {{ item.name }}
            </DsText>
            <DsTextWithCopyButton
                size="xs" :text="`Organization ID: ${item.id}`" :text-to-copy="item.id"
                :show-toast-on-copy="true" toast-message-on-copy="Copied organization ID to clipboard." />
          </div>
          <DsButton
              :id="`assign-${item.name?.replaceAll(' ', '-').toLowerCase()}-button`"
              :color="Colors.white"
              :text-color='Colors.onyx'
              variant="outline"
              size="sm"
              @click="updateAppOrganization(item.id)"
          >
            Assign
          </DsButton>
        </template>
        <template #footer>
          <DsTablePager
              class="pl-6" :paged-result-info="state.organizations"
              @go-to-page="(page) => loadOrganizations({ ...state.organizationApiFilter, page })" />
        </template>
      </DsTable>
    </div>
  </DsDrawer>
</template>
