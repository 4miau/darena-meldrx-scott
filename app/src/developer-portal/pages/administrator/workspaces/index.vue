<script setup lang="ts">
import {Colors} from "~/types/ui/colors";
import type {PagedResult} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult";
import type {WorkspaceDto} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto";
import type {ApiFilter} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter";
import { FhirServerType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/FhirServerType';
import type {OrganizationDto} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/OrganizationDto";
import type {
    CreateWorkspaceForExistingOrganization,
    WorkspaceOrganizationCommand
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/AdminWorkspaceCommands";
import type {CreateAppPermissionCommand} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CreateFhirServerGrantDto";
import {AppRole} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/AppRole";
import {ref} from "vue";
import type {IDsSelectItem} from "~/types/ui/DsSelect";

useHead({title: 'Workspace Admin | MeldRx'});
const confirmation = useConfirmation();

const {$api} = useNuxtApp();

const router = useRouter();
const formRef = ref<FormRef>();

const state = reactive<{
  isLoading: boolean;
  workspaces?: PagedResult<WorkspaceDto>;
  organizations?: PagedResult<OrganizationDto>;
  apiFilter: ApiFilter;
  organizationApiFilter: ApiFilter;
  openModal: boolean;
  selectedWorkspaceDto: WorkspaceDto | null;
  selectedOrganization: string;
  showEditDrawer: boolean;
  showUpdateOrg: boolean;
  showCreateWorkspace: boolean;
  form: CreateWorkspaceForExistingOrganization;
  selectedSystemApp: string;
}>({
    isLoading: false,
    workspaces: undefined,
    apiFilter: {
        page: 1,
        orderBy: 'name'
    },
    organizationApiFilter: {
        page: 1,
        orderBy: 'name'
    },
    openModal: false,
    selectedWorkspaceDto: null,
    selectedOrganization: 'string',
    showEditDrawer: false,
    showUpdateOrg: false,
    showCreateWorkspace: false,
    form: {
        name: '',
        slug: '',
        organizationId: ''
    },
    selectedSystemApp: ''
});

const appsMap = ref<IDsSelectItem<string>[]>();
const tableHeaders = [
    {title: 'Workspace Name', orderBy: 'name'},
    {title: 'Type', orderBy: 'type'},
    'Actions'
];

async function deleteWorkspace(workspaceId: string) {
    const {isCancelled} = await confirmation(
        'Are you sure you want to delete this workspace? This action cannot be undone.',
        'Delete Workspace'
    );
    if (isCancelled) {
        return;
    }

    state.isLoading = true;

    try {
        await $api.admin.workspaces.delete(workspaceId);
    } catch (error) {
        handleApiError(error, 'Unable to delete workspace');
    }

    // Reload linked apps for this app...
    await loadWorkspaces(state.apiFilter);
    state.isLoading = false;
}

async function loadWorkspaces(apiFilter: ApiFilter) {
    state.isLoading = true;

    try {
        state.workspaces = await $api.admin.workspaces.search(apiFilter, router.currentRoute.value.query.orgId?.toString());
        state.apiFilter = apiFilter;
        state.apiFilter.page = state.workspaces?.currentPage ?? 1;
    } catch (error) {
        handleApiError(error, 'Unable to load workspaces');
    }

    state.isLoading = false;
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

function onUpdateOrganizationClick(workspace: WorkspaceDto) {
    state.organizationApiFilter.filter = ''
    state.selectedOrganization = '';
    state.selectedWorkspaceDto = workspace;
    state.showUpdateOrg = true;
}

async function changeAnonymousAccess(workspaceId: string, newStatus: boolean) {
    state.isLoading = true
    if(!state.selectedWorkspaceDto) {
        return
    }

    try {
        await $api.admin.workspaces.updateAnonymousReadAccess(workspaceId, newStatus)
        state.selectedWorkspaceDto.allowAnonymousReadAccess = !state.selectedWorkspaceDto.allowAnonymousReadAccess
    } catch (error) {
        handleApiError(error, 'Unable to update workspace')
    }
    state.isLoading = false
    await loadWorkspaces(state.apiFilter)
}

async function changeSandboxStatus(workspaceId: string, newStatus: boolean) {
    state.isLoading = true
    if(!state.selectedWorkspaceDto) {
        return
    }

    try {
        await $api.admin.workspaces.updateSandboxStatus(workspaceId, newStatus)
        state.selectedWorkspaceDto.isSandbox = !state.selectedWorkspaceDto.isSandbox
    } catch (error) {
        handleApiError(error, 'Unable to update workspace')
    }
    state.isLoading = false
    await loadWorkspaces(state.apiFilter)
}

async function changeLiteStatus(workspaceId: string, newStatus: boolean) {
    state.isLoading = true
    if(!state.selectedWorkspaceDto) {
        return
    }

    try {
        await $api.admin.workspaces.changeLiteStatus(workspaceId, newStatus)
        state.selectedWorkspaceDto.isLiteWorkspace = !state.selectedWorkspaceDto.isLiteWorkspace
    } catch (error) {
        handleApiError(error, 'Unable to update workspace')
    }
    state.isLoading = false
    await loadWorkspaces(state.apiFilter)
}

async function updateWorkspaceOrganization(org: OrganizationDto, isOrgUpdate: boolean) {
    if(state.selectedWorkspaceDto == null) {
        return
    }
    state.isLoading = true

    const command:WorkspaceOrganizationCommand =
    {
        workspaceId : state.selectedWorkspaceDto.id,
        resellerId : isOrgUpdate ? state.selectedWorkspaceDto.resellerId : org.id,
        organizationId : isOrgUpdate ? org.id : state.selectedWorkspaceDto.organizationId
    };

    try {
        await $api.admin.workspaces.updateWorkspaceOrg(command)
        state.showUpdateOrg = false
    } catch (error) {
        handleApiError(error, 'Unable to update workspace organization')
    }
    state.isLoading = false
    await loadWorkspaces(state.apiFilter)
}

async function migrateToDedicatedDB(workspaceId: string) {
    const {isCancelled} = await confirmation(
        'Are you sure you want to migrate this workspace DB? This action cannot be undone.',
        'Migrate Workspace DB'
    );
    if (isCancelled) {
        return;
    }

    state.isLoading = true

    try {
        await $api.admin.workspaces.migrateDb(workspaceId)
        state.showEditDrawer = false
        notification({ title: 'Success', description: 'Workspace DB Migrated', displayTime: 3000, variant: 'success' });

    } catch (error) {
        handleApiError(error, 'Unable to migrate workspace db')
    }
    state.isLoading = false
    await loadWorkspaces(state.apiFilter)
}

function onEdit(workspace: WorkspaceDto){
    state.selectedWorkspaceDto = workspace
    state.showEditDrawer = true
}

async function backToOrganizations(){
    navigateTo(`organizations?page=1&orderBy=name&filter=${router.currentRoute.value.query.orgId?.toString()}`)
}

async function loadApps(): Promise<void> {
    try {
        appsMap.value = await $api.admin.apps.search({
            page: 1,
            orderBy: 'name'
        },router.currentRoute.value.query.orgId?.toString())
            .then(apps =>
                apps.resources
                    .filter(app => app.appType === 'System')
                    .map(x => ({
                        value: x.appId,
                        title: x.appName
                    }))
            );
    } catch (error) {
        handleApiError(error, 'Unable to load apps');
    }
}
async function onCreateWorkspace(){
    state.showCreateWorkspace = true
    state.selectedSystemApp = ''
    await loadApps()
}
async function createWorkspaceForOrganization(){
    if(!router.currentRoute.value.query.orgId){
        return
    }
    // Try to validate the form...
    if (!formRef.value) { return; }
    const isFormValid = formRef.value.validate();
    if (!isFormValid) { return; }

    state.isLoading = true
    try{
        state.form.organizationId = router.currentRoute.value.query.orgId.toString()
        const workspace = await $api.admin.workspaces.createForExistingOrg(state.form)
        state.showCreateWorkspace = false
        if (state.selectedSystemApp !== '') {
            try {
                const orgUserModificationModel: CreateAppPermissionCommand = {
                    clientId: state.selectedSystemApp,
                    appRole: AppRole.Administrator,
                    workspaceSlug: workspace.fhirDatabaseDisplayName
                };
                await $api.workspaces.createAppAccess(workspace.fhirDatabaseDisplayName, orgUserModificationModel);
            } catch (error) {
                handleApiError(error, 'Unable to add App Permissions')
            }
        }
    } catch (error){
        handleApiError(error, 'Unable to create workspace')
    }
    state.isLoading = false
}

loadWorkspaces(state.apiFilter)
</script>

<template>
  <DsContainer class="space-y-5">
    <DsLoadingOverlay :loading="state.isLoading" />
    <div class="w-full space-y-5">
      <DsText
        size="2xl"
        weight="light"
      >
        Workspaces
      </DsText>
      <DsTextInput
        id="search-organization"
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
            <DsIcon name="heroicons:magnifying-glass"/>
          </DsButton>
        </template>
      </DsTextInput>
      <div v-if="!!router.currentRoute.value.query.orgId" class="flex gap-4">
        <DsButton
            :color="Colors.white"
            :text-color='Colors.gray'
            variant="outline"
            @click="backToOrganizations"
        >
          Back to Organization
        </DsButton>
        <DsButton :color="Colors.primary" variant="filled" @click="onCreateWorkspace">
          Create Workspace
        </DsButton>
      </div>
    </div>
    <DsTable
      v-if="state.workspaces"
      :headers="tableHeaders"
      :items="state.workspaces.resources"
      :id-selector="item => item.id"
      :api-filter="state.apiFilter"
      @update:api-filter="loadWorkspaces"
    >
      <template #default="{ item }">
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
            :text="`Workspace ID: ${item.id}`"
            :text-to-copy="item.id"
            :show-toast-on-copy="true"
            toast-message-on-copy="Copied workspace ID to clipboard."
          />
        </div>
        <DsText
          size="md"
          weight="light"
        >
          {{ !!item.linkedFhirApiDto ? 'Linked' : 'Standalone' }}
        </DsText>
        <div class="space-x-2">
          <DsButton
              :id="`edit-${item.name?.replaceAll(' ', '-').toLowerCase()}-button`"
              :color="Colors.primary"
              :text-color='Colors.primary'
              variant="outline"
              @click="onEdit(item)"
          >
            Edit
          </DsButton>
          <DsButton
              :id="`manage-${item.name?.replaceAll(' ', '-').toLowerCase()}-org-button`"
              :color="Colors.secondary"
              :text-color='Colors.secondary'
              variant="outline"
              @click="() => onUpdateOrganizationClick(item)"
          >
            Manage Org/Reseller
          </DsButton>
          <DsButton
              :id="`delete-${item.name?.replaceAll(' ', '-').toLowerCase()}-button`"
              :color="Colors.fire"
              :text-color="Colors.fire"
              variant="outline"
              @click="deleteWorkspace(item.id)"
          >
            Delete
          </DsButton>
        </div>
      </template>
      <template #footer>
        <DsTablePager
          class="pl-6"
          :paged-result-info="state.workspaces"
          @go-to-page="(page) => loadWorkspaces({ ...state.apiFilter, page })"
        />
      </template>
    </DsTable>
  </DsContainer>

  <DsDrawer v-if="state.selectedWorkspaceDto" :show="state.showEditDrawer" @close="state.showEditDrawer = false">
    <div class="flex flex-col space-y-5">
      <DsIcon name="heroicons:x-mark" size="sm" class="absolute top-4 right-4 cursor-pointer" @click="state.showEditDrawer = false"/>
      <DsText size="2xl" weight="light">
        Manage Workspace
      </DsText>
      <DsText size="md" weight="normal">
        <b>Workspace Name:</b> {{ state.selectedWorkspaceDto.name }} <br>
        <b>Workspace Id:</b> {{ state.selectedWorkspaceDto.id }} <br>
        <b>Workspace Slug:</b> {{ state.selectedWorkspaceDto.fhirDatabaseDisplayName }} <br>
        <b>Workspace URL:</b> {{ state.selectedWorkspaceDto.fhirServerEndpoint }} <br><br>


        <b>Organization Id:</b> {{ state.selectedWorkspaceDto.organizationId }} <br>
        <b>Organization Name:</b> {{ state.selectedWorkspaceDto.organizationName }} <br>
        <b>Reseller Id:</b> {{ state.selectedWorkspaceDto.resellerId }} <br>
        <b>Reseller Name:</b> {{ state.selectedWorkspaceDto.resellerName }} <br>
      </DsText>

      <DsDivider/>

      <DsLabeledInput label="Anonymous Access">
        {{ state.selectedWorkspaceDto.allowAnonymousReadAccess ? 'Allowed' : 'Not Allowed' }}
        <DsToggle
            :id="`toggle-${state.selectedWorkspaceDto.id}-anonymous-button`"
            :model-value="state.selectedWorkspaceDto.allowAnonymousReadAccess"
            @update:model-value="changeAnonymousAccess(state.selectedWorkspaceDto.id, $event)"
        />
      </DsLabeledInput>

      <DsLabeledInput label="Sandbox / Production">
        {{ state.selectedWorkspaceDto.isSandbox ? 'Sandbox' : 'Production' }}
        <DsToggle
            :id="`toggle-${state.selectedWorkspaceDto.id}-sandbox-button`"
            :model-value="state.selectedWorkspaceDto.isSandbox"
            @update:model-value="changeSandboxStatus(state.selectedWorkspaceDto.id, $event)"
        />
      </DsLabeledInput>

      <DsLabeledInput label="Lite / Full">
        {{ state.selectedWorkspaceDto.isLiteWorkspace ? 'Lite' : 'Full' }}
        <DsToggle
            :id="`toggle-${state.selectedWorkspaceDto.id}-lite-button`"
            :model-value="state.selectedWorkspaceDto.isLiteWorkspace"
            @update:model-value="changeLiteStatus(state.selectedWorkspaceDto.id, $event)"
        />
      </DsLabeledInput>

      <DsLabeledInput v-if="state.selectedWorkspaceDto.sharedFhirServerId" label="Migrate to dedicated DB">
        <DsButton
            :id="`migrate-${state.selectedWorkspaceDto.name?.replaceAll(' ', '-').toLowerCase()}-button`"
            :color="Colors.primary"
            @click="migrateToDedicatedDB(state.selectedWorkspaceDto.id)"
        >
          Migrate
        </DsButton>
      </DsLabeledInput>
    </div>
  </DsDrawer>

  <DsDrawer v-if="state.selectedWorkspaceDto" :show="state.showUpdateOrg" @close="state.showUpdateOrg = false">
    <div class="w-full p-4 space-y-4">
      <div class="flex flex-col">
        <DsText size="2xl" weight="light">
          Manage Workspace Organization/Reseller
        </DsText>
        <DsIcon name="heroicons:x-mark" size="sm" class="absolute top-4 right-4 cursor-pointer" @click="state.showUpdateOrg = false"/>
        <DsText size="md" weight="light">
          <b>Selected Workspace:</b> {{ state.selectedWorkspaceDto.name }} <br>
          <b>Current Organization:</b> {{ state.selectedWorkspaceDto.organizationName }} <br>
          <b>Current Reseller:</b> {{ state.selectedWorkspaceDto.resellerName }} <br>
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
          <div class="flex flex-col">
            <DsText size="md" weight="light">
              {{ item.name }}
            </DsText>
            <DsText size="xs" weight="light">
              Type: {{ item.type }}
            </DsText>
            <DsTextWithCopyButton
                size="xs" :text="`Organization ID: ${item.id}`" :text-to-copy="item.id"
                :show-toast-on-copy="true" toast-message-on-copy="Copied organization ID to clipboard." />
          </div>
          <div class="flex justify-center gap-2">
            <DsButton
                :id="`select-${item.name?.replaceAll(' ', '-').toLowerCase()}-button`"
                :color="Colors.white"
                :text-color='Colors.gray'
                variant="outline"
                size="sm"
                @click="updateWorkspaceOrganization(item, true)"
            >
              Set as Org
            </DsButton>
            <DsButton
                v-if="item.type.toString() === 'Developer'"
                :id="`select-${item.name?.replaceAll(' ', '-').toLowerCase()}-button`"
                :color="Colors.white"
                :text-color='Colors.gray'
                variant="outline"
                size="sm"
                @click="updateWorkspaceOrganization(item, false)"
            >
              Set as Reseller
            </DsButton>
          </div>
        </template>
        <template #footer>
          <DsTablePager
              class="pl-6" :paged-result-info="state.organizations"
              @go-to-page="(page) => loadOrganizations({ ...state.organizationApiFilter, page })" />
        </template>
      </DsTable>
    </div>
  </DsDrawer>

  <DsModal :model-value="state.showCreateWorkspace" @close="state.showCreateWorkspace = false">
    <DsModalProgressCard :total-steps='1' :current-step='1'>
      <div class="flex w-full py-6 bg-space justify-center items-center">
        <div class="flex w-full items-center justify-center">
          <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center">
            <MeldRxApp />
          </div>
        </div>
      </div>
      <DsForm ref="formRef">
        <div class="flex w-full py-4 px-8">
          <div class="flex flex-col w-full">
            <div class="w-full flex-col justify-center gap-5 flex">
              <DsText size="xl" weight="normal">
                Create Workspace for Organization
              </DsText>
              <DsText size="lg" weight="light">
                <strong> Selected Organization: </strong><br>
                {{ router.currentRoute.value.query.orgId}}
              </DsText>
              <div class="w-full grid grid-cols-1 gap-4">
                <DsTextInput
                    v-model="state.form.name"
                    required
                    type="text"
                    label="Workspace Name"
                    :rules="[[v => !!v, 'Workspace Name is required']]"
                />
                <DsTextInput
                    v-model="state.form.slug"
                    required
                    type="text"
                    label="Workspace Slug"
                    :rules="[[v => !!v, 'Workspace Slug is required']]"
                />
                <DsSelect
                    v-if="appsMap && appsMap.length > 0"
                    v-model="state.selectedSystemApp"
                    :items="appsMap"
                    label="App Name"
                    placeholder="Select an App"
                    @update:model-value="(appId) => state.selectedSystemApp = appId"
                />
              </div>
              <DsDivider />
              <div class="flex justify-center w-full gap-5">
                <DsButton title="Cancel" :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" @click="state.showCreateWorkspace = false">
                    Cancel
                </DsButton>
                <DsButton :color="Colors.secondary" @click="createWorkspaceForOrganization">
                  Create Workspace
                </DsButton>
              </div>
            </div>
          </div>
        </div>
      </DsForm>
    </DsModalProgressCard>
  </DsModal>
</template>
