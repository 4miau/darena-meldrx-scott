<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import EditStandaloneWorkspaceDetails from '~/components/workspaces/EditStandaloneWorkspaceDetails.vue';
import type { FhirServerSettings } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/FhirServerSettings';
import { FhirServerType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/FhirServerType';

definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    refreshWorkspace: true,
    allowLinked: true,
});
useHead({ title: 'General Settings | MeldRx' })

const { $api } = useNuxtApp()

const route = useRoute()
const url = useRequestURL();
const confirmation = useConfirmation()
const { loadSubscription, subscription } = useSubscription()
const { isAdmin, permissions } = useAuth()
const { workspace, loadingWorkspace, refreshWorkspace } = useWorkspace()

defineEmits<{
  'saveWorkspace': [];
  'cancel': [];
}>()

// Get workspace ID from route...
const workspaceSlug = ref<string>(route.params.workspaceSlug as string)
const isLoading = ref<boolean>(false)
const workspaceSettings = ref<FhirServerSettings>()
const showDeleteButton = computed(() => (permissions.value.isDeveloper && subscription.value.subscriptionType == 'developer') || isAdmin)

// Load workspace settings...
async function loadSettings () {
    workspaceSettings.value = await $api.workspaces.getSettings(workspaceSlug.value)
}

// "Save Workspace"...
async function onSaveWorkspace (settings?:FhirServerSettings) {
    if (isLoading.value || loadingWorkspace.value || !workspace.value) {
        return
    }

    isLoading.value = true

    try {
        if (workspace.value.linkedFhirApiDto) {
            // if linked
            await $api.workspaces.updateLinked({
                workspaceId: workspace.value.id,
                workspaceName: workspace.value.name,
                validationOption: workspace.value.validationOption,
                fhirApiProviderMeldRxIdentifier: workspace.value.linkedFhirApiDto.fhirApiProviderMeldRxIdentifier,
                baseUrl: workspace.value.linkedFhirApiDto.baseUrl,
                launchAppClientId: workspace.value.linkedFhirApiDto.launchAppClientId,
                launchAppScopes: workspace.value.linkedFhirApiDto.launchAppScopes
            })
        } else {
            // if standalone
            await $api.workspaces.update({
                workspaceId: workspace.value.id,
                workspaceName: workspace.value.name,
                validationOption: workspace.value.validationOption,
                isLiteWorkspace: workspace.value.isLiteWorkspace
            })
        }

        await refreshWorkspace()
    } catch (error) {
        handleApiError(error, 'Unable to save workspace');
    }
    try {
        if (settings) {
            await $api.workspaces.updateSettings(workspaceSlug.value, settings)
        }
    } catch (error) {
        handleApiError(error, 'Unable to save workspace settings');
    } finally {
        isLoading.value = false
    }
}

// "Delete Workspace"...
async function onDeleteWorkspace () {
    // Check if user really wants to delete it first...
    const { isCancelled } = await confirmation(
        'Are you sure you want to delete this workspace? All data will be permanently lost. This action cannot be undone.',
        'Delete workspace'
    )
    if (isCancelled) {
        return
    }

    isLoading.value = true

    try {
        await $api.workspaces.delete(workspaceSlug.value)
        loadSubscription()
    } catch (error) {
        handleApiError(error, 'Unable to delete workspace');
        isLoading.value = false
        return
    }

    isLoading.value = false
    navigateTo('/workspaces')
}

// "Upgrade Workspace"...
async function onUpgradeWorkspace () {
    if (isLoading.value || loadingWorkspace.value || !workspace.value) {
        return
    }

    // Confirm the upgrade
    const { isCancelled } = await confirmation(
        'Are you sure you want to upgrade this workspace? This will trigger an invoice for this workspace. This action cannot be undone. The upgrade process may take up to 3 hours.',
        'Upgrade workspace'
    )
    if (isCancelled) {
        return
    }

    try {
        workspace.value.isLiteWorkspace = false;

        await onSaveWorkspace();

        notification({
            title: 'Success',
            description: 'Workspace upgraded successfully',
            displayTime: 3000,
            variant: 'success'
        })
    } catch (error) {
        handleApiError(error, 'Unable to upgrade workspace');
    } finally {
        isLoading.value = false
    }
}

loadSettings()
</script>
<template>
  <DsContainer>
    <DsLoadingOverlay :loading="isLoading || loadingWorkspace" />
    <div v-if="workspace" class="mb-5">
      <DsText size="2xl" weight="light" class="block">
        General Settings
      </DsText>
      <DsText size="xs" weight="light">
        {{ workspace.name }}
      </DsText>
      <div v-if="workspace.isSandbox || workspace.isLiteWorkspace" class="mt-5 space-x-1.5">
        <DsBadge
            v-if="workspace.isLiteWorkspace"
            :color="Colors.lapisLazuli50"
            :text-color="Colors.white"
            size="xs"
        >
          Lite
        </DsBadge>
        <DsBadge
            v-if="workspace.isSandbox"
            :color="Colors.fire50"
            :text-color="Colors.white"
            size="xs"
        >
          Sandbox
        </DsBadge>
      </div>
      <div class="mt-5">
        <DsText v-if="workspace.type === FhirServerType.Virtual || subscription.subscriptionType !== 'enterprise'" size="md" weight="light">
          View and modify details about this workspace.
        </DsText>
        <DsText v-else-if="subscription.subscriptionType === 'enterprise'" size="md" weight="light">
          View and modify details about this workspace. To delete this workspace, contact support.
        </DsText>
      </div>
    </div>

    <div v-if="workspace" class="w-[1100px]">
      <!-- Workspace Actions -->
      <div v-if="subscription.subscriptionType !== 'enterprise' || (workspace.type === FhirServerType.Virtual || workspace.isLiteWorkspace)" class="grid grid-cols-12 gap-14">
        <!-- Workspace Type Description -->
        <div class="col-span-4">
          <DsLabeledText
            label="Workspace Actions"
            text="Administrative actions for this workspace"
          />
        </div>

        <!-- Workspace Type Buttons -->
        <div class="col-span-8">
          <DsText size="sm">
            Actions
          </DsText>
          <div class="py-1" />
          <div class="space-x-2">
            <DsButton v-if="workspace.isLiteWorkspace" :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" @click="onUpgradeWorkspace">
              <DsIcon name="heroicons:chevron-double-up" size='sm' />
              Upgrade Lite to Full
            </DsButton>
            <DsButton v-if="showDeleteButton" :color="Colors.fire" :text-color='Colors.fire' variant="outline" @click="onDeleteWorkspace">
              <DsIcon name="heroicons:x-mark" size='sm' />
              Delete Workspace
            </DsButton>
          </div>
        </div>
      </div>
      <DsDivider />

      <EditLinkedWorkspaceDetails
        v-if="workspace.linkedFhirApiDto"
        v-model:workspace-id="workspace.id"
        v-model:workspace-identifier="workspace.workspaceIdentifier"
        v-model:workspace-slug="workspace.fhirDatabaseDisplayName"
        v-model:workspace-name="workspace.name"
        v-model:validation-option="workspace.validationOption"
        v-model:patient-strategy="workspace.linkedFhirApiDto.patientStrategy"
        v-model:linked-fhir-api="workspace.linkedFhirApiDto"
        v-model:fhir-url="workspace.linkedFhirApiDto.baseUrl"
        v-model:fhir-server-endpoint="workspace.fhirServerEndpoint"
        v-model:ehr-launch-url="workspace.ehrLaunchUrl"
        v-model:launch-app-client-id="workspace.linkedFhirApiDto.launchAppClientId"
        v-model:launch-app-scopes="workspace.linkedFhirApiDto.launchAppScopes"
        success-button-name="Save"
        @success="onSaveWorkspace"
      />

      <EditStandaloneWorkspaceDetails
        v-else-if="workspaceSettings"
        v-model:workspace-id="workspace.id"
        v-model:workspace-identifier="workspace.workspaceIdentifier"
        v-model:workspace-slug="workspace.fhirDatabaseDisplayName"
        v-model:workspace-name="workspace.name"
        v-model:validation-option="workspace.validationOption"
        v-model:fhir-server-endpoint="workspace.fhirServerEndpoint"
        v-model:is-lite-workspace="workspace.isLiteWorkspace"
        :workspace-settings="workspaceSettings"
        :mcp-sse-url="`${url.protocol}//${url.host}/api/Mcp/${workspace.id}/sse`"
        success-button-name="Save"
        @success="(settings) => onSaveWorkspace(settings)"
      />
    </div>
  </DsContainer>
</template>
