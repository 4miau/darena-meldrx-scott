<!--
    Step 2 of "Create Workspace"
    Asks the user for details to create a standalone/blank workspace
-->

<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import type { FhirServerSettings } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/FhirServerSettings';
import type { FhirServerValidationOption } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/FhirServerValidationOption'

const props = defineProps<{
  workspaceId: string;
  workspaceSlug: string;
  workspaceName: string;
  workspaceIdentifier: string;
  validationOption: FhirServerValidationOption;
  fhirServerEndpoint: string;
  successButtonName: string;
  workspaceSettings: FhirServerSettings;
  mcpSseUrl: string;
}>();

const emit = defineEmits<{
  'update:workspaceName': [value: string];
  'update:validationOption': [value: FhirServerValidationOption];
  'success': [value: FhirServerSettings];
}>();

const formRef = ref<FormRef>()
const errorMessage = ref<string>('');

const settingsForm = ref<FhirServerSettings>({
    emailSubject: props.workspaceSettings.emailSubject,
    inviteTitle: props.workspaceSettings.inviteTitle,
    inviteText: props.workspaceSettings.inviteText,
    hasLogo: props.workspaceSettings.hasLogo,
    nonFhirResourceType: props.workspaceSettings.nonFhirResourceType,
    id: props.workspaceSettings.id,
    organizationName: props.workspaceSettings.organizationName
})

watch(() => props.workspaceSettings, (v) => {
    settingsForm.value = {
        emailSubject: v.emailSubject,
        inviteTitle: v.inviteTitle,
        inviteText: v.inviteText,
        hasLogo: v.hasLogo,
        nonFhirResourceType: v.nonFhirResourceType,
        id: v.id,
        organizationName: v.organizationName
    }
})
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
    emit('success', settingsForm.value);
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

        <DsLabeledText v-if="!!workspaceId" label="Workspace Id">
          <DsTextWithCopyButton
            id="workspace-id"
            size="xs"
            weight="light"
            :text="`${workspaceId}`"
            :text-to-copy="workspaceId"
            :show-toast-on-copy="true"
            toast-message-on-copy="Copied Workspace ID to clipboard."
          />
        </DsLabeledText>

        <DsLabeledText v-if="!!workspaceSlug" label="Workspace URL">
          <DsTextWithCopyButton
            id="workspace-url"
            size="xs"
            weight="light"
            :text="fhirServerEndpoint"
            :text-to-copy="fhirServerEndpoint"
            :show-toast-on-copy="true"
            toast-message-on-copy="Copied Workspace URL to clipboard."
          />
        </DsLabeledText>

        <DsLabeledText v-if="!!workspaceSlug" label="Workspace MCP SSE URL">
          <DsTextWithCopyButton
            id="workspace-mcp-sse-url"
            size="xs"
            weight="light"
            :text="mcpSseUrl"
            :text-to-copy="mcpSseUrl"
            :show-toast-on-copy="true"
            toast-message-on-copy="Copied Workspace MCP SSE URL to clipboard."
          />
        </DsLabeledText>

        <DsLabeledText
          label="Workspace Type"
          text="Standalone Workspace"
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

    <div class="w-[1100px]">
      <!-- Invitation Settings -->
      <div class="grid grid-cols-12 gap-14">
        <!-- Invitation Settings Description -->
        <div class="col-span-4">
          <DsLabeledText
            label="Invitation Settings"
            text="Edit the details for patient invitations."
          />
        </div>

        <!-- Invitation Settings Form -->
        <div class="col-span-8 space-y-5">

          <!-- Invite Email Subject -->
          <DsTextInput
            v-model="settingsForm.emailSubject"
            label="Invite Email Subject"
            :required="true"
            :rules="[[(v) => !!v, 'Email Subject is required']]"
          />

          <!-- Landing Page Title -->
          <DsTextInput
            v-model="settingsForm.inviteTitle"
            label="Landing Page Title"
            :required="true"
            :rules="[[(v) => !!v, 'Landing Page Title is required']]"
          />

          <!-- Landing Page Body -->
          <DsTextArea
            v-model="settingsForm.inviteText"
            label="Landing Page Body"
            :min-rows="3"
            required
            :rules="[[(v) => !!v, 'Landing Page Body is required']]"
          />
        </div>
      </div>
    </div>
    <DsDivider />

    <!-- Cancel/Previous/Create Workspace Buttons -->
    <div class="justify-start items-start gap-5 inline-flex pb-10">
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
