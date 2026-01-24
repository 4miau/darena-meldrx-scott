<script setup lang="ts">
import type { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';
import { Colors } from '~/types/ui/colors';
import type { LinkedWorkspacePatientStrategy } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/LinkedWorkspacePatientStrategy'
import type { FhirServerValidationOption } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/FhirServerValidationOption'
import type LinkedFhirApiDto from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/LinkedFhirApiDto";

const props = defineProps<{
  workspaceId: string;
  workspaceIdentifier: string;
  workspaceSlug: string;
  workspaceName: string;
  validationOption: FhirServerValidationOption;
  patientStrategy: LinkedWorkspacePatientStrategy;
  linkedFhirApi: LinkedFhirApiDto;
  fhirUrl: string;
  fhirServerEndpoint: string;
  ehrLaunchUrl?: string;
  successButtonName: string
  userType?: SoFAppUserType;
  launchAppClientId?: string;
  launchAppScopes?: string;
}>();

const emit = defineEmits<{
  'update:workspaceName': [value: string];
  'update:validationOption': [value: FhirServerValidationOption];
  'update:fhirUrl': [value: string];
  'update:launchAppClientId': [value: string];
  'update:launchAppScopes': [value: string];
  'success': [];
}>();

const formRef = ref<FormRef>()
const errorMessage = ref<string>('');

// "Create Workspace"...
function onCreateWorkspace() {
    // Clear errors...
    errorMessage.value = '';

    // An error occurred with the form...
    if (!formRef.value) { return; }

    // Try to validate the form...
    const isFormValid = formRef.value.validate();
    if (!isFormValid) {
        errorMessage.value = 'Please fix the errors above.';
        return;
    }

    emit('success');
}
</script>

<template>
  <DsForm ref="formRef">
    <!-- Workspace Details Section-->
    <div class="grid grid-cols-12 gap-14">
      <!-- Workspace Details -->
      <div class="col-span-4">
        <DsLabeledText
          label="Workspace Details"
          text="This information will be used internally to help you identify this workspace."
        />
      </div>

      <!-- Workspace ID / Workspace URL / Workspace Type / Workspace Name -->
      <div class="col-span-8 space-y-5">
        <DsLabeledText
            label="Workspace Identifier"
        >
          <DsTextWithCopyButton
              id="workspace-identifier"
              size="xs"
              weight="light"
              :text="workspaceIdentifier"
              :text-to-copy="workspaceIdentifier"
              :show-toast-on-copy="true"
              toast-message-on-copy="Copied Workspace Identifier to clipboard."
          />
        </DsLabeledText>
        <div>
          <DsText size="sm">
            Workspace ID
          </DsText>

          <DsText size="xs" weight="light">
            <DsTextWithCopyButton
              id="workspace-id"
              size="xs"
              weight="light"
              :text="`${workspaceId}`"
              :text-to-copy="workspaceId"
              :show-toast-on-copy="true"
              toast-message-on-copy="Copied to clipboard."
            />
          </DsText>
        </div>

        <div>
          <DsText size="sm">
            Workspace URL
          </DsText>

          <DsText size="xs" weight="light">
            <DsTextWithCopyButton
              id="workspace-url"
              size="xs"
              weight="light"
              :text="fhirServerEndpoint"
              :text-to-copy="fhirServerEndpoint"
              :show-toast-on-copy="true"
              toast-message-on-copy="Copied Workspace URL to clipboard."
            />
          </DsText>
        </div>

        <DsLabeledText
          label="Workspace Type"
          text="Linked App Workspace"
        />

        <DsLabeledText
          label="Patient Strategy"
          :text="patientStrategy"
        />

        <!-- Workspace Name -->
        <DsTextInput
          :model-value="workspaceName"
          label="Workspace Name"
          :required="true"
          :rules="ValidationRules.workspaceName"
          @update:model-value="$emit('update:workspaceName', $event ?? '')"
        />

        <!-- Resource Validation -->
        <DsLabeledInput label="Profile Validation">
          <DsButtonGroup
            :model-value="validationOption"
            :active-color="Colors.secondary"
            @update:model-value="$emit('update:validationOption', $event as FhirServerValidationOption)"
          >
            <DsButton value="Enabled" :text-color='Colors.gray'>
              Enabled
            </DsButton>
            <DsButton value="Disabled" :text-color='Colors.gray'>
              Disabled
            </DsButton>
          </DsButtonGroup>
        </DsLabeledInput>
      </div>
    </div>
    
    <DsDivider />
    
    <!-- EHR Launch -->
    <div class="grid grid-cols-12 gap-14">
      <!-- App Details -->
      <div class="col-span-4">
        <DsLabeledText
            label="EHR Launch Details"
            text="Configure the client id and scopes to be used with this workspace for EHR launch.
            Leave these fields blank to use the default MeldRx logic."
        />
      </div>


      <div class="col-span-8 space-y-5">
        <!-- EHR Launch URL -->
        <div v-if="ehrLaunchUrl">
          <DsText size="sm">
            EHR Launch URL
          </DsText>
  
          <DsText size="xs" weight="light">
            <DsTextWithCopyButton
                id="ehr-launch-url"
                size="xs"
                weight="light"
                :text="ehrLaunchUrl"
                :text-to-copy="ehrLaunchUrl"
                show-toast-on-copy
                toast-message-on-copy="Copied EHR Launch URL to clipboard."
            />
          </DsText>
        </div>

        <!-- Custom Client Id -->
        <DsTextInput
            :model-value="launchAppClientId"
            label="Client Id"
            placeholder="Leave blank to use default"
            @update:model-value="$emit('update:launchAppClientId', $event ?? '')"
        />
        
        <!-- Custom Scopes -->
        <DsTextInput
            :model-value="launchAppScopes"
            label="Scopes (space separated)"
            placeholder="openid patient/*.read"
            @update:model-value="$emit('update:launchAppScopes', $event ?? '')"
        />
      </div>
    </div>

    <DsDivider />
    
    <!-- FHIR Provider -->
    <div class="grid grid-cols-12 gap-14">
      <!-- App Details -->
      <div class="col-span-4">
        <DsLabeledText
          label="FHIR API Details"
          text="This information identifies the platform associated with this workspace."
        />
      </div>
      <!-- FHIR Provider -->
      <div class="col-span-8">
        <DsLabeledText
            label="FHIR API Provider"
            :text="`${props.linkedFhirApi.fhirApiProviderProductName} by ${props.linkedFhirApi.fhirApiProviderOrganizationName} (${props.linkedFhirApi.fhirApiProviderVersion})`"
        />
      </div>
    </div>
    
    <DsDivider />

    <!-- External FHIR API Details Section -->
    <div class="grid grid-cols-12 gap-14">
      <!-- External FHIR API Details -->
      <div class="col-span-4">
        <DsLabeledText
          label="External FHIR API Details"
          text="This information will be used to connect to an external FHIR API"
        />
      </div>

      <!-- FHIR URL -->
      <div class="col-span-8">
        <div>
          <FhirUrlValidator
            :fhir-url="fhirUrl"
            required
            @update:fhir-url="$emit('update:fhirUrl', $event ?? '')"
          />
        </div>
      </div>
    </div>

    <!-- Cancel/Previous/Create Workspace Buttons -->
    <DsDivider />
    <div class="justify-start items-start gap-5 inline-flex">
      <DsButton :color="Colors.primary" @click="onCreateWorkspace">
        {{ successButtonName }}
      </DsButton>
    </div>

    <!-- Error Messages -->
    <div v-if="!!errorMessage">
      <div class="p-4">
        <DsText size="sm" :color="Colors.fire">
          {{ errorMessage }}
        </DsText>
      </div>
    </div>
  </DsForm>
</template>
