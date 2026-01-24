<script setup lang="ts">
import {Colors} from '~/types/ui/colors'
import type {PagedResult} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'
import type {WorkspaceEhrAppDto} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceEhrAppDto'
import type {ApiFilter} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter'
import {DsiOption, DsiOptionMap} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/DsiOption';
import type {SourceAttributeGroup} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/SourceAttribute';
import type DynamicRegistrationDto
    from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/DynamicRegistrationDto';
import type {ChaiModelCardGroup} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/ChaiModelCardForm";

definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    allowLinked: true,
})
useHead({title: 'Active Extensions | MeldRx'})

const {$api} = useNuxtApp()
const route = useRoute()

const confirmation = useConfirmation()

const workspaceSlug = ref<string>(route.params.workspaceSlug as string)
const {permissions} = useAuth()
const headersExtensions = ['Extension Name', 'Type', 'DSI Type', 'Actions'];

const state = reactive<{
  workspaceEhrApps?: PagedResult<WorkspaceEhrAppDto>;
  apiFilter: ApiFilter,
  openModal: boolean;
  openPublishedAppsModal: boolean;
  isLoading: boolean,
  showDsiDrawer: boolean,
  showModelCard: boolean,
  showAppDetailDrawer: boolean,
  showAdditionalAppDetails: boolean,
  disableEditAttributes: boolean,
  attributeDrawerTitle?: string,
  attributeDrawerDescription?: string,
  selectAppDetails?: WorkspaceEhrAppDto,
  ehrAppDetails?: DynamicRegistrationDto
  dsiAppName?: string,
  dsiAppId?: string,
  form: {
    dsi: DsiOption;
    sourceAttributes?: SourceAttributeGroup[];
    chaiModelCard?: ChaiModelCardGroup[];
    originalSourceAttributes?: SourceAttributeGroup[];
  },
}>({
    apiFilter: {
        page: 1,
        filter: '',
    },
    openModal: false,
    openPublishedAppsModal: false,
    isLoading: false,
    showDsiDrawer: false,
    showModelCard: false,
    showAppDetailDrawer: false,
    showAdditionalAppDetails: false,
    disableEditAttributes: false,
    form: {
        dsi: DsiOption.None,
    },
})

async function loadWorkspaceEhrApps(apiFilter: ApiFilter) {
    state.isLoading = true
    try {
        state.workspaceEhrApps = await $api.ehr.list(workspaceSlug.value, apiFilter);
        state.apiFilter = apiFilter;
        state.apiFilter.page = state.workspaceEhrApps?.currentPage ?? 1;
    } catch (error) {
        handleApiError(error, 'Unable to load extensions')
    } finally {
        state.isLoading = false
    }
}

async function loadAppSrcAttributes(appId: string, appName: string, disableEdit: boolean) {
    state.isLoading = true;
    try {
        const originalApp = await $api.ehr.getDsiSourceAttributes(workspaceSlug.value, appId);
        const originalSourceAttributes = await $api.apps.getPublishedAppForEdit(appId);
        if (originalApp) {
            state.form.originalSourceAttributes = originalSourceAttributes.sourceAttributeGroups
            state.form.sourceAttributes = originalApp;
            state.showDsiDrawer = true;
            state.disableEditAttributes = disableEdit;
            state.dsiAppName = appName;
            state.dsiAppId = appId;
            state.selectAppDetails = state.workspaceEhrApps?.resources.find(x => x.appId === appId);
            if (disableEdit) {
                state.attributeDrawerTitle = 'View Extension Details';
                state.attributeDrawerDescription = 'View source attributes of this extension';
                state.showAdditionalAppDetails = true;
            } else {
                state.attributeDrawerTitle = 'Modify Source Attributes';
                state.attributeDrawerDescription = 'View and modify source attributes of this extension';
                state.showAdditionalAppDetails = true;
            }
        }
    } catch (error) {
        handleApiError(error, 'Unable to load extension details');
    } finally {
        state.isLoading = false;
    }
}

async function removeApp(ehrAppId: string) {
    const {isCancelled} = await confirmation(
        'Are you sure you want to remove this Extension from workspace? This action cannot be undone.',
        'Remove Extension'
    )
    if (isCancelled) {
        return
    }

    state.isLoading = true
    try {
        await $api.ehr.delete(workspaceSlug.value, ehrAppId)
    } catch (error) {
        handleApiError(error, 'Unable to remove extension')
    } finally {
        state.isLoading = false
        await loadWorkspaceEhrApps(state.apiFilter)
    }
}

async function onSaveFromDsiDrawer(sourceAttributes: SourceAttributeGroup[]) {
    state.isLoading = true;
    try {
        if (state.form.sourceAttributes && state.dsiAppId) {
            await $api.ehr.putDsiSourceAttributes(workspaceSlug.value, state.dsiAppId, sourceAttributes);
            state.showDsiDrawer = false;
            notification({
                title: 'Success',
                description: 'Source attributes saved successfully',
                displayTime: 3000,
                variant: 'success',
            });
        }
    } catch (error) {
        handleApiError(error, 'Unable to save source attributes');
    } finally {
        state.isLoading = false;
        state.dsiAppName = '';
        state.dsiAppId = '';
    }
}

async function loadAppModelCard(appId: string) {
    state.isLoading = true;
    try {
        const appDetails = await $api.apps.getPublishedAppForEdit(appId);
        if (appDetails) {
            state.form.chaiModelCard = appDetails.chaiModelCardGroups;
            state.showModelCard = true;
        }
        state.dsiAppId = appId;
        state.selectAppDetails = state.workspaceEhrApps?.resources.find(app => app.appId === appId);
    } catch (error) {
        handleApiError(error, 'Unable to load model card');
    } finally {
        state.isLoading = false;
    }
}
function onCancelFromDsiDrawer() {
    state.showDsiDrawer = false;
}

async function onActivateExtension() {
    await navigateTo(`/workspaces/${route.params.workspaceSlug}/extensions/activation`)
}


loadWorkspaceEhrApps(state.apiFilter)

</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="state.isLoading"/>

    <!-- Dsi Source Attributes Viewer/Editor Drawer -->
    <DsDrawer :show="state.showDsiDrawer" @close="onCancelFromDsiDrawer">
      <DsSourceAttributeModifier
          v-if="state.form.sourceAttributes != null && state.dsiAppName"
          :title="state.attributeDrawerTitle ?? 'View Source Attributes'"
          :sub-title="state.dsiAppName"
          :source-attributes="state.form.sourceAttributes"
          :original-source-attributes="state.form.originalSourceAttributes"
          :description="state.attributeDrawerDescription"
          :form-entry-disabled="state.disableEditAttributes"
          @on-cancel="onCancelFromDsiDrawer"
          @on-save="onSaveFromDsiDrawer">
        <template v-if="state.showAdditionalAppDetails || state.form.sourceAttributes == null" #additional>
          <div v-if="state.selectAppDetails" class="flex flex-col gap-2">
            <DsText v-if="state.selectAppDetails.organizationName" size="sm" weight="light">
              <strong>Organization: </strong> {{ state.selectAppDetails.organizationName }}
            </DsText>
            <DsText v-if="state.selectAppDetails.publisherUrl" size="sm" weight="light">
              <strong>Publisher: </strong>
              <DsLink :href="state.selectAppDetails.publisherUrl" underline="hover" target="_blank">
                {{ state.selectAppDetails.publisherUrl }}
              </DsLink>
            </DsText>
            <DsText v-if="state.selectAppDetails.termsOfServiceUrl" size="sm" weight="light">
              <strong>Terms of Service: </strong>
              <DsLink :href="state.selectAppDetails.termsOfServiceUrl" underline="hover" target="_blank">
                {{ state.selectAppDetails.termsOfServiceUrl }}
              </DsLink>
            </DsText>
            <DsText v-if="state.selectAppDetails.privacyPolicyUrl" size="sm" weight="light">
              <strong>Privacy Policy: </strong>
              <DsLink :href="state.selectAppDetails.privacyPolicyUrl" underline="hover" target="_blank">
                {{ state.selectAppDetails.privacyPolicyUrl }}
              </DsLink>
            </DsText>
            <DsDivider/>
          </div>
        </template>
      </DsSourceAttributeModifier>
    </DsDrawer>

    <DsViewer v-if="state.showModelCard" @close="state.showModelCard = false">
      <ChaiModelCard
          v-if="state.form.chaiModelCard"
          class="h-full overflow-auto m-5 space-y-5"
          :model-card-form="state.form.chaiModelCard"
      />
    </DsViewer>

    <div class="space-y-5">
      <!-- Header Section -->
      <DsText size="2xl" weight="light" class="block">
        Active Extensions
      </DsText>

      <DsText size="sm" weight="light" class="block">
        Manage the extensions for your workspace. Extensions are apps, provided by your EHR or third-party developers,
        that can be integrated into the patient chart. Some extensions allow you to open apps directly with the
        patient already selected. Others trigger alerts based on patient data, offering insights and ability to
        perform actions. You should review the details of each extension, including their source and attributes, to
        better understand how they are developed and their applicability within your workflow.
      </DsText>

      <!-- Search bar -->
      <div class="flex items-center">
        <DsTextInput
            id="extension-searchbar"
            v-model="state.apiFilter.filter"
            class="w-full"
            placeholder="Search extensions"
            @enter-pressed="() => loadWorkspaceEhrApps(state.apiFilter)"
        >
          <template #right>
            <DsButton :color="Colors.white" :text-color='Colors.gray' variant="outline" @click="loadWorkspaceEhrApps(state.apiFilter)" >
              Search
              <DsIcon name='heroicons:arrow-right'/>
            </DsButton>
          </template>
        </DsTextInput>
      </div>

      <DsButton
          :color="Colors.primary"
          variant="filled"
          :disabled="!permissions.developerPermissionsDto?.canCreateApps && !permissions.workspaceUserPermissionDto?.find(x=> x.slug == workspaceSlug)?.canAddExtensions"
          @click="onActivateExtension"
      >
        <DsIcon name='heroicons:plus'/>
        Activate Extension
      </DsButton>

      <!-- Table Section -->
      <DsTable
          v-if="state.workspaceEhrApps"
          :headers="headersExtensions"
          :items="state.workspaceEhrApps.resources"
          :id-selector="item => item.id"
      >
        <template #default="{ item }">
          <div class="flex flex-col">
            <div class="flex flex-col items-start">
              <DsText size="md" weight="light">
                {{ item.appName }}
              </DsText>
              <DsText size="sm" weight="light">
                App Publisher - {{ item.organizationName }}
              </DsText>
            </div>
            <DsText size="sm" weight="light">
              {{ item.launchUrl ? 'App Launch' : item.cdsHookServiceUrl ? 'Alert Card' : 'None' }}
            </DsText>
            <DsText size="sm" weight="light">
              {{ DsiOptionMap(item.dsiType) }}
            </DsText>
            <div class="space-x-2 flex flex-row">
              <DsButton
                  :id="`Edit-${item.id.replaceAll(' ', '-').toLowerCase()}-button`"
                  :color="Colors.secondary"
                  :text-color='Colors.secondary'
                  variant="outline"
                  size="sm"
                  @click="loadAppSrcAttributes(item.appId, item.appName, (!permissions.workspaceUserPermissionDto?.find(x=> x.slug == workspaceSlug)?.canAddExtensions && !permissions.developerPermissionsDto?.canCreateApps))"
              >
                <DsIcon name='heroicons:arrow-right'/>
                {{ (!permissions.workspaceUserPermissionDto?.find(x=> x.slug == workspaceSlug)?.canAddExtensions && !permissions.developerPermissionsDto?.canCreateApps ? 'View ' : 'Modify ') + 'Source Attributes' }}
              </DsButton>
              <DsButton
                  :id="`select-${item.id.replaceAll(' ', '-').toLowerCase()}-button`"
                  :color="Colors.fire" :text-color='Colors.fire' variant="outline"
                  size="sm"
                  :disabled="!permissions.developerPermissionsDto?.canCreateApps && !permissions.workspaceUserPermissionDto?.find(x=> x.slug == workspaceSlug)?.canAddExtensions"
                  @click="removeApp(item.id)"
              >
                <DsIcon name='heroicons:x-mark'/>
                Remove
              </DsButton>
              <DsButton
                  v-if="item.hasChaiModelCard && DsiOptionMap(item.dsiType) === 'Predictive'"
                  :id="`chai-${item.appName.replaceAll(' ', '-').toLowerCase()}-button`"
                  :color="Colors.primary"
                  :text-color='Colors.primary'
                  variant="outline"
                  size="sm"
                  @click="loadAppModelCard(item.appId)"
              >
                View CHAI Model Card
              </DsButton>
            </div>
          </div>
        </template>
        <template #footer>
          <DsTablePager
              class="pl-6"
              :paged-result-info="state.workspaceEhrApps"
              @go-to-page="loadWorkspaceEhrApps({ ...state.apiFilter, page: $event })"
          />
        </template>
      </DsTable>
    </div>
  </DsContainer>
</template>
