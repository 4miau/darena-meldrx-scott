<!--
    Step 2 of "Create Workspace"
    Asks the user for details to create a standalone/blank workspace
-->

<script setup lang="ts">
import { ref } from 'vue';
import { Colors } from '~/types/ui/colors';
import type { FhirServerValidationOption } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/FhirServerValidationOption'
import type { IDsSelectItem } from '~/types/ui/DsSelect';

defineProps<{
  workspaceId: string;
  workspaceSlug: string;
  workspaceName: string;
  validationOption: FhirServerValidationOption;
  fhirServerEndpoint: string;
  email: string | null;
  firstName: string | null;
  lastName: string | null;
  organizationName: string;
  organizationIdentifier: string;
  successButtonName: string
  showPreviousStepButton?: boolean;
  appId?: string;
  showCanSellWorkspaceData: boolean;
  showCanChangeSlug: boolean;
  showCanAddLiteWorkspace: boolean;
  isLiteWorkspace: boolean;
}>();

const emit = defineEmits<{
  'update:workspaceName': [value: string];
  'update:validationOption': [value: FhirServerValidationOption];
  'update:email': [value: string | null];
  'update:firstName': [value: string | null];
  'update:lastName': [value: string | null];
  'update:organizationName': [value: string];
  'update:organizationIdentifier': [value: string];
  'update:workspaceSlug': [value: string];
  'update:appId': [value: string];
  'update:isLiteWorkspace': [value: boolean];
  'success': [];
  'cancel': [];
  'previous': [];
}>();

const { $api } = useNuxtApp();

const formRef = ref<FormRef>()
const errorMessage = ref<string>('');
const baseUrl = location.origin
const slugTaken = ref<boolean>();
const appsMap = ref<IDsSelectItem<string>[]>();
const addAppForm = reactive<{
  clientId?: string
}>({});
const showCreateAdmin = ref<boolean>(true);
const { debounce: debounceCheckWorkspaceSlug } = useDebounce(checkWorkspaceSlug, 500);

// Load Apps
async function loadApps(): Promise<void> {
    try {
        appsMap.value = await $api.apps.list()
            .then(apps =>
                apps
                    .filter(app => app.soFAppUserType === 'System')
                    .map(x => ({
                        value: x.client_id,
                        title: x.client_name
                    }))
            );
    } catch (error) {
        handleApiError(error, 'Unable to load apps');
    }
}

// "Create Workspace"...
function onCreateWorkspace() {
    // Clear errors...
    errorMessage.value = '';

    // An error occurred with the form...
    if (!formRef.value) {
        return;
    }

    // Try to validate the form...
    const isFormValid = formRef.value.validate();
    if (!isFormValid || slugTaken.value) {
        errorMessage.value = 'Please fix the errors above.';
        return;
    }

    emit('success');
}

async function checkWorkspaceSlug(workspaceSlug: string) {
    slugTaken.value = ValidationRules.valid(workspaceSlug, ValidationRules.slug)
        ? await $api.workspaces.checkSlug(workspaceSlug)
        : true;
}

function skipAdminCreate() {
    showCreateAdmin.value = false;
    emit('update:firstName', null);
    emit('update:lastName', null);
    emit('update:email', null);
}

loadApps();
</script>

<template>
  <DsForm ref="formRef">
    <!-- Workspace Details Section-->
    <div class="grid">
      <!-- Workspace Details -->
      <div class="grid grid-cols-12 gap-x-10">
        <div class="col-span-4">
          <DsLabeledText
            label="Workspace Details"
            text="This information will be used internally to help you identify this workspace."
          />
        </div>

        <div class="col-span-8 space-y-5">
          <!-- Workspace Type -->
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

          <!-- Workspace Identifier -->
          <DsTextInput
            v-if="showCanSellWorkspaceData"
            :model-value="organizationIdentifier"
            label="Workspace Identifier"
            placeholder="EIN, TIN or leave blank for random"
            @update:model-value="$emit('update:organizationIdentifier', $event ?? '')"
          />

          <!-- Workspace URL Slug -->
          <DsTextInput
            v-if="showCanChangeSlug"
            :model-value="workspaceSlug"
            label="Workspace URL Slug"
            placeholder="Custom workspace URL slug or leave blank for random"
            :rules="ValidationRules.slug"
            @keyup="debounceCheckWorkspaceSlug(workspaceSlug)"
            @update:model-value="$emit('update:workspaceSlug', $event ?? '')"
          />

          <div v-if="showCanChangeSlug && workspaceSlug">
            <div>
              <DsText size="sm">
                {{ baseUrl + '/api/fhir/' + workspaceSlug }}
              </DsText>
            </div>
            <div class="items-center">
              <DsText size="sm" class="flex row">
                <template v-if="slugTaken">
                  <DsIcon name="heroicons:exclamation-circle" size="sm" :color='Colors.fire' />
                  Not available
                </template>
                <template v-else>
                  <DsIcon name="heroicons:check-circle" size="sm" :color='Colors.mint' />
                  Available
                </template>
              </DsText>
            </div>
          </div>
        </div>

        <div v-if="showCanSellWorkspaceData" class="grid grid-cols-12 gap-x-10 col-span-12">
          <DsDivider class="col-span-12" />

          <!-- Workspace Administrator -->
          <div class="col-span-4 space-y-5">
            <DsLabeledText
              label="Workspace Administrator"
              text="This information is used to create the initial administrator."
            />
          </div>

          <div class="col-span-8 space-y-5">
            <!-- Create Workspace Admin -->
            <DsLabeledInput label="Create Initial Workspace Admin Account">
              <DsButtonGroup
                :model-value="showCreateAdmin"
                :active-color="Colors.secondary"
              >
                <DsButton :value="true" size="sm" :text-color='Colors.gray' @click="showCreateAdmin = true">
                  Yes
                </DsButton>
                <DsButton :value="false" size="sm" :text-color='Colors.gray' @click="skipAdminCreate">
                  No
                </DsButton>
              </DsButtonGroup>
            </DsLabeledInput>

            <!-- First Name -->
            <DsTextInput
              v-if="showCanSellWorkspaceData && showCreateAdmin"
              :model-value="firstName"
              label="First Name"
              :required="true"
              :rules="[[(v) => !!v, 'First name is required']]"
              @update:model-value="$emit('update:firstName', $event ?? '')"
            />

            <!-- Last Name -->
            <DsTextInput
              v-if="showCanSellWorkspaceData && showCreateAdmin"
              :model-value="lastName"
              label="Last Name"
              :required="true"
              :rules="[[(v) => !!v, 'Last name is required']]"
              @update:model-value="$emit('update:lastName', $event ?? '')"
            />

            <!-- Email -->
            <DsTextInput
              v-if="showCanSellWorkspaceData && showCreateAdmin"
              :model-value="email"
              label="Email"
              :required="true"
              :rules="[[(v) => !!v, 'Email is required'], [(v) => /.+@.+\..+/.test(v ?? ''), 'Email must be valid']]"
              @update:model-value="$emit('update:email', $event ?? '')"
            />
          </div>
        </div>

        <DsDivider class="col-span-12" />

        <template v-if="showCanAddLiteWorkspace">
          <div class="col-span-4">
            <DsLabeledText
              label="Lite Workspace"
              text="This setting determines if patient data is allowed or if it will only be listed in the endpoint directory." />
          </div>
          <div class="col-span-8">
            <DsLabeledInput label="Lite Workspace">
              <template #popoverarea>
                <DsIcon name="heroicons:information-circle" size="sm" />
              </template>
              <template #popovercontent>
                <DsText size="sm">
                  A lite workspace is not enabled for API interaction. It only appears in the public directories. Full workspaces do not have this restriction.
                </DsText>
              </template>
              <DsButtonGroup
                :model-value="isLiteWorkspace"
                :active-color="Colors.secondary"
                @update:model-value="(value?: boolean | boolean[] | undefined) => {
                  if (Array.isArray(value)) {
                    emit('update:isLiteWorkspace', value[0] ?? false);
                  } else {
                    emit('update:isLiteWorkspace', value ?? false);
                  }
                }">
                <DsButton :value="true" size="sm" :text-color='Colors.gray'>
                  Lite
                </DsButton>
                <DsButton :value="false" size="sm" :text-color='Colors.gray'>
                  Full
                </DsButton>
              </DsButtonGroup>
            </DsLabeledInput>
          </div>
          <DsDivider class="col-span-12" />
        </template>

        <!-- Resource Validation -->
        <div class="col-span-4">
          <DsLabeledText
            label="FHIR Profile Validation"
            text="This setting determines whether FHIR resources/bundles require profile validation. This does not impact CCDA import validation."
          />
        </div>

        <div class="col-span-8">
          <DsLabeledInput label="Profile Validation">
            <template #popoverarea>
              <DsIcon name="heroicons:information-circle" size="sm" />
            </template>
            <template #popovercontent>
              <DsText size="sm">
                During POST and PUT calls, resources that have a profile set in their metadata field will be validated
                against that profile.
              </DsText>
            </template>
            <DsButtonGroup
              :model-value="validationOption"
              :active-color="Colors.secondary"
              @update:model-value="$emit('update:validationOption', $event as FhirServerValidationOption)"
            >
              <DsButton value="Enabled" size="sm" :text-color='Colors.gray'>
                Enabled
              </DsButton>
              <DsButton value="Disabled" size="sm" :text-color='Colors.gray'>
                Disabled
              </DsButton>
            </DsButtonGroup>
          </DsLabeledInput>
        </div>

        <!-- Add default system app -->
        <div v-if="appsMap && appsMap.length > 0" class="grid grid-cols-12 gap-x-10 col-span-12">
          <DsDivider class="col-span-12" />

          <!-- App Details -->
          <div class="col-span-4">
            <DsLabeledText
              label="App Details"
              text="This optionally identifies a default system app used to access this workspace."
            />
          </div>

          <div class="col-span-8">
            <DsSelect
              v-model="addAppForm.clientId"
              :items="appsMap"
              label="App Name"
              searchable
              placeholder="Select an App"
              @update:model-value="(appId) => $emit('update:appId', appId)"
            />
          </div>
        </div>
      </div>

      <!-- Cancel/Previous/Create Workspace Buttons -->
      <DsDivider />
      <div class="justify-start items-start gap-5 inline-flex pb-10">
        <DsButton :color="Colors.white" :text-color='Colors.gray' variant="subtle" @click="$emit('cancel');">
          Cancel
        </DsButton>
        <DsButton v-if="showPreviousStepButton" :color="Colors.primary" :text-color='Colors.gray' variant="outline" @click="$emit('previous')">
          Previous Step
        </DsButton>
        <DsButton :color="Colors.primary" @click="onCreateWorkspace">
          {{ successButtonName }}
        </DsButton>
      </div>

      <!-- Error Messages -->
      <div v-if="!!errorMessage">
        <div class="p-4">
          <DsText id="create-error" size="sm" :color="Colors.fire">
            {{ errorMessage }}
          </DsText>
        </div>
      </div>
    </div>
  </DsForm>
</template>
