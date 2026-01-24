<script setup lang="ts">
import type FhirApiProviderDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';
import type { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';
import type { INewLinkedApp } from '~/types/ui/apps/NewLinkedApp';
import { Colors } from '~/types/ui/colors';
import type { LinkedWorkspacePatientStrategy } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/LinkedWorkspacePatientStrategy'
import type { FhirServerValidationOption } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/FhirServerValidationOption'
import type { IDsSelectItem } from '~/types/ui/DsSelect';

const props = defineProps<{
  workspaceId: string;
  workspaceSlug: string;
  workspaceName: string;
  patientStrategy: LinkedWorkspacePatientStrategy;
  validationOption: FhirServerValidationOption;
  email: string | null;
  firstName: string | null;
  lastName: string | null;
  organizationName: string;
  organizationIdentifier: string;
  fhirUrl?: string;
  fhirApiProviderMeldRxIdentifier?: string;
  fhirServerEndpoint: string;
  currentLinkedApps: INewLinkedApp[];
  successButtonName: string
  fhirApiProviders: FhirApiProviderDto[];
  showPreviousStepButton?: boolean;
  userType?: SoFAppUserType;
  appId?: string;
  showCanSellWorkspaceData: boolean;
  showCanChangeSlug: boolean;
}>();

const emit = defineEmits<{
  'update:workspaceName': [value: string];
  'update:patientStrategy': [value: LinkedWorkspacePatientStrategy];
  'update:validationOption': [value: FhirServerValidationOption];
  'update:email': [value: string | null];
  'update:firstName': [value: string | null];
  'update:lastName': [value: string | null];
  'update:organizationName': [value: string];
  'update:organizationIdentifier': [value: string];
  'update:fhirUrl': [value: string];
  'update:fhirApiProviderMeldRxIdentifier': [value: string];
  'update:workspaceSlug': [value: string];
  'update:appId': [value: string];
  'success': [];
  'cancel': [];
  'previous': [];
}>();

const { $api } = useNuxtApp();

const formRef = ref<FormRef>()
const errorMessage = ref<string>('');
const slugTaken = ref<boolean>();
const baseUrl = location.origin
const appsMap = ref<IDsSelectItem<string>[]>();
const addAppForm = reactive<{
  clientId?: string
}>({});
const showCreateAdmin = ref<boolean>(true);

const {debounce: debounceCheckWorkspaceSlug} = useDebounce(checkWorkspaceSlug,500);

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

// Load Apps
async function loadApps (): Promise<void> {
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

const sandboxOptions = computed(() =>
    EHRUtils.getSandboxFhirUrls(props.fhirApiProviderMeldRxIdentifier, props.userType)
        .map(x => ({ value: x.fhirUrl, title: x.name }))
)

function selectSandboxOption (fhirUrl: string) {
    emit('update:fhirUrl', fhirUrl)
}

function onFhirProviderSelect(fhirApiProviderMeldRxIdentifier: string) {
    emit('update:fhirUrl', '')
    emit('update:fhirApiProviderMeldRxIdentifier', fhirApiProviderMeldRxIdentifier)
}

async function checkWorkspaceSlug(workspaceSlug: string) {
    if (workspaceSlug !== '') {
        slugTaken.value = await $api.workspaces.checkSlug(workspaceSlug);
    }
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
    <div class="grid grid-cols-12 gap-x-10">
      <!-- Workspace Details -->
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
          text="Linked App Workspace"
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
            <DsText v-if="slugTaken" size="sm" class="flex row">
              <DsIcon name="heroicons:exclamation-circle" size="sm" :color='Colors.fire' />
              Not available
            </DsText>
            <DsText v-if="!slugTaken" size="sm" class="flex row">
              <DsIcon name="heroicons:check-circle" size="sm" :color='Colors.mint' />
              Available
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

      <!-- Resource Validation -->
      <div class="col-span-4">
        <DsLabeledText
          label="FHIR Profile Validation"
          text="This setting determines whether FHIR resources/bundles require profile validation. This does not impact CCDA import validation."
        />
      </div>

      <div class="col-span-8">
        <DsLabeledInput
          label="Profile Validation"
        >
          <template #popoverarea>
            <DsIcon name="heroicons:information-circle" size="sm" />
          </template>
          <template #popovercontent>
            <DsText size="sm">
              During POST and PUT calls, resources that have a profile set in their metadata field will be validated against that profile.
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

      <DsDivider class="col-span-12" />

      <!-- Patient Strategy -->
      <div class="col-span-4">
        <DsLabeledText
          label="Patient App Authentication Strategy"
          text="This setting only applies for Patient apps and determines how users will authenticate using either the external identity management of the linked FHIR API or using MeldRx as an identity provider."
        />
      </div>

      <div class="col-span-8">
        <DsLabeledInput
          label="Patient Strategy"
        >
          <template #popoverarea>
            <DsIcon name="heroicons:information-circle" size="sm" />
          </template>
          <template #popovercontent>
            <DsText size="sm">
              This will dictate how users using the patient app will authenticate: to use either the Default(external) or the MeldRx(internal) authentication flow.
            </DsText>
          </template>
          <DsButtonGroup
            :model-value="patientStrategy"
            :active-color="Colors.secondary"
            @update:model-value="$emit('update:patientStrategy', $event as LinkedWorkspacePatientStrategy)"
          >
            <DsButton value="Default" size="sm" :text-color='Colors.gray'>
              Default
            </DsButton>
            <DsButton value="MeldRx" size="sm" :text-color='Colors.gray'>
              MeldRx
            </DsButton>
          </DsButtonGroup>
        </DsLabeledInput>
      </div>

      <DsDivider class="col-span-12" />

      <!-- FHIR Provider -->
      <!-- App Details -->
      <div class="col-span-4">
        <DsLabeledText
          label="FHIR API Provider"
          text="This information identifies the platform associated with this workspace."
        />
      </div>
      <!-- FHIR Provider -->
      <div class="col-span-8 max-w-[500px]">
        <FhirProviderSelect
          :fhir-api-provider-meld-rx-identifier="fhirApiProviderMeldRxIdentifier"
          :rules="[[(v) => !!v, 'FHIR API Provider is required']]"
          :include-ehr-quick-buttons="true"
          :current-linked-apps="currentLinkedApps"
          @update:fhir-api-provider-meld-rx-identifier="onFhirProviderSelect"
        />
      </div>
    </div>

    <template v-if="sandboxOptions.length > 0">
      <!-- Sandbox Environment FHIR API Details Section -->
      <div class="grid grid-cols-12 gap-10">
        <!-- Sandbox Environment Details -->
        <div class="col-span-4 max-w-[400px]">
          <DsLabeledText
            label="Sandbox FHIR API URLs"
            text="This FHIR API Provider has some sandbox environments you can use to test with mock patient data."
          />
        </div>
        <div class="col-span-8 max-w-[500px]">
          <DsSelect
            :model-value="fhirUrl"
            label="Select an available sandbox"
            placeholder="Search a sandbox"
            :items="sandboxOptions"
            @update:model-value="selectSandboxOption($event)"
          />
        </div>
      </div>
    </template>

    <DsDivider />

    <!-- External FHIR API Details Section -->
    <div class="grid grid-cols-12 gap-10">
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

    <!-- Cancel/Previous/Create Workspace Buttons -->
    <DsDivider />
    <div class="justify-start items-start gap-5 inline-flex">
      <DsButton :color="Colors.white" :text-color='Colors.gray' variant="subtle" @click="$emit('cancel');">
        Cancel
      </DsButton>
      <DsButton v-if="showPreviousStepButton" :color="Colors.primary" :text-color='Colors.primary' variant="outline" @click="$emit('previous')">
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
  </DsForm>
</template>
