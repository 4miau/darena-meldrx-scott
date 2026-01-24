<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import type { ApiFilter } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter';
import type { SourceAttributeGroup } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/SourceAttribute';
import {DsiOption, DsiOptionMap} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/DsiOption';
import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult';
import type { AppSourceType } from '~/types/ui/apps/AppSourceType';
import type { RegisteredAppWithWorkspaceDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/RegisteredAppWithWorkspaceDto';
import type { AddWorkspaceEhrAppCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/AddWorkspaceEhrAppCommand';
import type {ChaiModelCardGroup} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/ChaiModelCardForm";

const { $api } = useNuxtApp()
const { workspaces } = useAuth()


const route = useRoute()

const props = defineProps<{
    appSourceType: AppSourceType;
    workspaceSlug: string;
}>();

defineEmits<{
    'cancel': [];
    'previous': [];
}>();

const headersExtensions = ['Extension Name', 'Type', 'DSI Type', 'Actions'];
const state = reactive<{
    displayApps?: PagedResult<RegisteredAppWithWorkspaceDto>;
    apiFilter: ApiFilter,
    appForActivation: string;
    isLoading: boolean,
    showDsiDrawer: boolean,
    showModelCard: boolean,
    dsiAppName?: string,
    dsiAppId?: string,
    selectAppDetails?: RegisteredAppWithWorkspaceDto
    form: {
        dsi: DsiOption;
        sourceAttributes?: SourceAttributeGroup[];
        chaiModelCard?: ChaiModelCardGroup[];
    },
}>({
    apiFilter: {
        page: 1,
        filter: '',
    },
    appForActivation: '',
    isLoading: false,
    showDsiDrawer: false,
    showModelCard: false,
    form: {
        dsi: DsiOption.None,
    },
})

async function loadAccessibleApps(filter: ApiFilter) {
    state.isLoading = true;
    try {
        if (props.appSourceType === 'Published') {
            state.displayApps = await $api.ehr.listOrgAccessibleApps(props.workspaceSlug, filter)
        } else if (props.appSourceType === 'Ehr' || props.appSourceType === 'Internal') {
            state.displayApps = await $api.ehr.listOrgOnlyApps(props.workspaceSlug, filter);
        } else if (props.appSourceType === 'Workspace') {
            state.displayApps = await $api.ehr.listWorkspaceApps(props.workspaceSlug, filter, false);
        }

    } catch (error) {
        handleApiError(error, 'Unable to get apps for activation');
    } finally {
        state.isLoading = false;
    }
}

async function loadAppSrcAttributes(appId: string, appName: string) {
    state.isLoading = true;
    try {
        const appDetails = await $api.apps.getPublishedAppForEdit(appId);
        if (appDetails) {
            state.form.sourceAttributes = appDetails.sourceAttributeGroups;
            state.showDsiDrawer = true;
        }
        state.dsiAppName = appName;
        state.dsiAppId = appId;
        state.selectAppDetails = state.displayApps?.resources.find(app => app.appId === appId);
    } catch (error) {
        handleApiError(error, 'Unable to load app details');
    } finally {
        state.isLoading = false;
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
        state.selectAppDetails = state.displayApps?.resources.find(app => app.appId === appId);
    } catch (error) {
        handleApiError(error, 'Unable to load model card');
    } finally {
        state.isLoading = false;
    }
}

async function addAppToWorkspace(appId: string) {
    state.isLoading = true;
    try {
        const orgUserModificationModel: AddWorkspaceEhrAppCommand = { appId: appId, workspaceId:workspaces.value.find(x=> x.fhirDatabaseDisplayName == props.workspaceSlug)?.id };
        await $api.ehr.create(props.workspaceSlug, orgUserModificationModel);
        notification({ title: 'Success', description: 'Extension has been added to your workspace', displayTime: 2000, variant: 'success' });
    } catch (error) {
        handleApiError(error, 'Unable to add extension to workspace');
    } finally {
        state.isLoading = false;
        navigateTo(`/workspaces/${route.params.workspaceSlug}/extensions`);
    }
}

function onCancelFromDsiDrawer() {
    state.showDsiDrawer = false;
}

async function onClickActivateExtension() {
    if (state.appForActivation) {
        await addAppToWorkspace(state.appForActivation);
        await loadAccessibleApps(state.apiFilter);
    }else{
        notification({
            title: 'Error',
            description: 'Please select an extension to activate',
            displayTime: 3000,
            variant: 'error'
        })
    }
}

function configureSelectedExtension(appId: string) {
    state.appForActivation = state.appForActivation === appId ? '' : appId;
}


await loadAccessibleApps(state.apiFilter);
</script>

<template>
    <!-- Dsi Source Attributes Viewer/Editor Drawer -->
  <DsDrawer :show="state.showDsiDrawer" @close="onCancelFromDsiDrawer">
    <DsSourceAttributeModifier
        v-if="state.dsiAppName"
        title="View Extension Details"
        :sub-title="state.dsiAppName"
        :source-attributes="state.form.sourceAttributes ?? []"
        description="View source attributes about this application"
        :form-entry-disabled="true"
        @on-cancel="onCancelFromDsiDrawer"
    >
    <template #additional>
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
            <DsDivider />
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

    <!-- Search bar -->
    <div class="flex items-center py-4">
        <DsTextInput
            id="extension-searchbar"
            v-model="state.apiFilter.filter"
            class="w-full"
            placeholder="Search extensions"
            @enter-pressed="() => loadAccessibleApps(state.apiFilter)"
        >
            <template #right>
                <DsButton
                    :color="Colors.white"
                    :text-color='Colors.gray'
                    variant="outline"
                    @click="loadAccessibleApps(state.apiFilter)"
                >
                  Search
                  <DsIcon name="heroicons:arrow-right"/>
                </DsButton>
            </template>
        </DsTextInput>
    </div>

    <DsTable
        v-if="state.displayApps"
        :headers="headersExtensions"
        :items="state.displayApps.resources"
        :id-selector="item => item.appId"
        :disable-stripe="true"
        :highlight-selector="item => item.appId === state.appForActivation">
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
                    {{ item.ehrIntegration }}
                </DsText>
                <DsText size="sm" weight="light">
                    {{ DsiOptionMap(item.dsiType) }}
                </DsText>
                <div class="space-x-2 flex flex-row">
                    <DsButton
                        :id="`view-${item.appName.replaceAll(' ', '-').toLowerCase()}-button`"
                        :color="Colors.white"
                        :text-color='Colors.gray'
                        variant="outline"
                        size="md"
                        @click="loadAppSrcAttributes(item.appId, item.appName)"
                    >
                      <DsIcon name='heroicons:arrow-right'/>
                      View
                    </DsButton>
                    <DsButton
                        v-if="!item.fhirServerId"
                        :id="`select-${item.appName.replaceAll(' ', '-').toLowerCase()}-button`"
                        :color="Colors.white"
                        :text-color='Colors.gray'
                        :variant="item.appId === state.appForActivation ? 'filled' : 'outline'"
                        size="md"
                        @click="configureSelectedExtension(item.appId)"
                    >
                      <DsIcon name='heroicons:arrow-right'/>
                      {{ item.appId === state.appForActivation ? 'Selected' : 'Select' }}
                    </DsButton>
                    <DsButton
                        v-if="item.hasChaiModelCard && DsiOptionMap(item.dsiType) === 'Predictive'"
                        :id="`chai-${item.appName.replaceAll(' ', '-').toLowerCase()}-button`"
                        :color="Colors.primary"
                        :text-color='Colors.primary'
                        variant="outline"
                        size="md"
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
            :paged-result-info="state.displayApps"
            @go-to-page="loadAccessibleApps({ ...state.apiFilter, page: $event })"
            />
        </template>
    </DsTable>
    <DsDivider />

    <!-- Cancel/Previous/Next Buttons -->
    <div class="justify-start items-start gap-5 inline-flex">
      <DsButton :color="Colors.white" :text-color='Colors.gray' size="md" variant="subtle" @click="$emit('cancel');">
        Cancel
      </DsButton>
      <DsButton :color="Colors.primary" :text-color='Colors.primary' size="md" variant="outline" @click="$emit('previous');">
        Previous Step
      </DsButton>
      <DsButton :color="Colors.primary" size="md" @click="onClickActivateExtension">
        Activate Extension
      </DsButton>
    </div>
</template>
