<script setup lang="ts">
import type { INewWorkspace } from '~/types/meldrx-api/workspaces'
import useConfirmation from '~/composables/useConfirmation'
import type { CreateWorkspaceAndNewOrgCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CreateWorkspaceAndNewOrgCommand'
import type { CreateDeveloperWorkspaceCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CreateDeveloperWorkspaceCommand'
import { LinkedWorkspacePatientStrategy } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/LinkedWorkspacePatientStrategy'
import { FhirServerValidationOption } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/FhirServerValidationOption'
import type { CreateAppPermissionCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CreateFhirServerGrantDto';
import { AppRole } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/AppRole';
import {MeldRxSubscription} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/MeldRxSubscription';

useHead({ title: 'Create Workspace | MeldRx' });

const { $api } = useNuxtApp();
const { subscription, loadSubscription } = useSubscription()
const { permissions, loadUserWorkspaces } = useAuth()
const confirmation = useConfirmation();
const fhirApiProviders = useFhirApiProviders();


const newWorkspace = ref<INewWorkspace>({
    workspaceType: 'standalone',
    name: '',
    fhirApiProviderId: '',
    fhirUrl: '',
    email: '',
    firstName: '',
    lastName: '',
    organizationName: '',
    organizationIdentifier: '',
    workspaceSlug: '',
    isLiteWorkspace: subscription.value.allowLiteWorkspaces ? true : false,
    patientStrategy: LinkedWorkspacePatientStrategy.Default,
    validationOption: FhirServerValidationOption.Enabled
});

const isLoading = ref<boolean>(false);
const fhirServerEndpoint = ref<string>('');
const workspaceSlug = ref<string>('');
const subscriptionType = subscription.value.subscriptionType;

// "Create Workspace"...
async function onCreateWorkspace() {
    // Create the workspace based on the user's selection...
    let workspace: CreateDeveloperWorkspaceCommand | CreateWorkspaceAndNewOrgCommand
    if (permissions.value.developerPermissionsDto?.canSellWorkspaces) {
        workspace = {
            workspaceName: newWorkspace.value.name,
            email: newWorkspace.value.email,
            firstName: newWorkspace.value.firstName,
            lastName: newWorkspace.value.lastName,
            organizationName: newWorkspace.value.name,
            description: '',
            organizationIdentifier: newWorkspace.value.organizationIdentifier,
            password: null,
            confirmPassword: null,
            slug: newWorkspace.value.workspaceSlug,
            validationOption: newWorkspace.value.validationOption,
            isLiteWorkspace: newWorkspace.value.fhirApiProviderId && newWorkspace.value.fhirUrl
                ? false : newWorkspace.value.isLiteWorkspace,
            linkedFhirApiDto: newWorkspace.value.fhirApiProviderId && newWorkspace.value.fhirUrl
                ? {
                    fhirApiProviderMeldRxIdentifier: newWorkspace.value.fhirApiProviderId,
                    baseUrl: newWorkspace.value.fhirUrl,
                    patientStrategy: newWorkspace.value.patientStrategy,
                    fhirApiProviderProductName: '',
                    fhirApiProviderOrganizationName: '',
                    fhirApiProviderVersion: ''
                }
                : undefined
        }
    } else if (newWorkspace.value.workspaceType === 'standalone') {
        workspace = {
            name: newWorkspace.value.name,
            validationOption: newWorkspace.value.validationOption
        }
    } else {
        workspace = {
            name: newWorkspace.value.name,
            validationOption: newWorkspace.value.validationOption,
            linkedFhirApi: {
                fhirApiProviderMeldRxIdentifier: newWorkspace.value.fhirApiProviderId,
                baseUrl: newWorkspace.value.fhirUrl,
                patientStrategy: newWorkspace.value.patientStrategy
            }
        }
    }

    isLoading.value = true;

    try {
        if (permissions.value.developerPermissionsDto?.canSellWorkspaces) {
            const createdWorkspace = await $api.workspaces.createWithOrganization(workspace as CreateWorkspaceAndNewOrgCommand);
            fhirServerEndpoint.value = createdWorkspace.workspaceDto.fhirServerEndpoint;
            workspaceSlug.value = createdWorkspace.workspaceDto.fhirDatabaseDisplayName;
        } else {
            const createdWorkspace = await $api.workspaces.create(workspace as CreateDeveloperWorkspaceCommand);
            fhirServerEndpoint.value = createdWorkspace.fhirServerEndpoint;
            workspaceSlug.value = createdWorkspace.fhirDatabaseDisplayName;
        }
        if (newWorkspace.value.appId != null) {
            try {
                const orgUserModificationModel: CreateAppPermissionCommand = {
                    clientId: newWorkspace.value.appId,
                    appRole: AppRole.Administrator,
                    workspaceSlug: workspaceSlug.value
                };
                await $api.workspaces.createAppAccess(workspaceSlug.value, orgUserModificationModel);
            } catch (error) {
                handleApiError(error, 'Unable to add App Permissions')
            }
        }
    } catch (error) {
        handleApiError(error, 'Unable to create workspace')
    }
    finally {
        loadSubscription()
        loadUserWorkspaces()
        isLoading.value = false;
    }
}

// "Cancel"
async function onCancel() {
    // Check if user really wants to cancel it first...
    const { isCancelled } = await confirmation(
        'Are you sure you want to stop creating this workspace? All of your progress will be permanently lost. This action cannot be undone.',
        'Discard workspace'
    );
    if (isCancelled) { return; }

    navigateTo('/workspaces');
}

function onGoToWorkspace() {
    navigateTo(newWorkspace.value.workspaceType === 'linked' ? `/workspaces/${workspaceSlug.value}` : `/workspaces/${workspaceSlug.value}/patients`);
}
</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="isLoading" />

    <DsText size="2xl" weight="light">
      Create Workspace
    </DsText>
    <div class="pb-5" />

    <DsText size="sm" weight="light" class="block">
      Create a new MeldRx workspace in two simple steps.
    </DsText>
    <div class="pb-5" />

    <CreateNewWorkspace
      v-model="newWorkspace"
      :fhir-api-providers="fhirApiProviders"
      :show-can-sell-workspace-data="permissions.developerPermissionsDto?.canSellWorkspaces ?? false"
      :show-can-change-slug="subscriptionType === MeldRxSubscription.Enterprise"
      :show-can-add-lite-workspace="subscription.allowLiteWorkspaces"
      @cancel="onCancel"
      @create-workspace="onCreateWorkspace"
    />

    <WorkspaceCreatedModal
      :show="!!fhirServerEndpoint"
      :fhir-server-endpoint="fhirServerEndpoint"
      @close="navigateTo('/workspaces')"
      @go-to-workspace="onGoToWorkspace"
    />
  </DsContainer>
</template>
