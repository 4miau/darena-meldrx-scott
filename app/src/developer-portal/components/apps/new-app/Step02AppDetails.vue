<script setup lang="ts">
import { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod';
import { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';
import { Colors } from '~/types/ui/colors';
import { appScopes, systemAppScopes } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Scopes';
import type { CreateCdsHooksAppCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CreateCdsHooksAppCommand';
import { CdsAppType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/CdsAppType';

const props = defineProps<{
  appName: string;
  userType: SoFAppUserType;
  authenticationClientType: SoFAppTokenAuthMethod;
  scopes: string;
  redirectUrls: string[];
  ehrLaunchUrl?: string;
}>();

const emit = defineEmits<{
  'update:appName': [value: string];
  'update:appPublisherUrl': [value?: string];
  'update:authenticationClientType': [value: SoFAppTokenAuthMethod];
  'update:scopes': [value: string];
  'update:redirectUrls': [value: string[]];
  'update:ehrLaunchUrl': [value?: string];
  'createCdsHookApp': [CreateCdsHooksAppCommand];
  'next': [];
  'previous': [];
  'cancel': [];
}>();

const formRef = ref<FormRef>();
const state = reactive<{
  createCdsHookAppCommand: CreateCdsHooksAppCommand
  cdsAppType: CdsAppType,
  errorMessage: string
}>({
    createCdsHookAppCommand: {},
    cdsAppType: CdsAppType.External,
    errorMessage: ''
});

watch(
    () => state.cdsAppType,
    () => {
        state.createCdsHookAppCommand = {}
    }
)

function onNextStep () {
    if (!formRef.value?.validate()) {
        state.errorMessage = 'Please fix the errors above.';
        return;
    }
    emit('next');
}

function createApp () {
    if (!formRef.value?.validate() || props.userType !== SoFAppUserType.CdsHooks) {
        return;
    }

    state.createCdsHookAppCommand.name = props.appName;
    emit('createCdsHookApp', state.createCdsHookAppCommand);
}

const cdsHookOptions: { value: CdsAppType, title: string }[] = [
    {
        value: CdsAppType.External,
        title: 'Service URL'
    },
    {
        value: CdsAppType.Hosted,
        title: 'Upload CQL (ELM Files)'
    },
    {
        value: CdsAppType.HostedCustomCql,
        title: 'Custom CQL'
    }
];

</script>

<template>
  <DsForm ref="formRef">
    <div class="space-y-8">
      <!-- App Details -->
      <div class="grid md:grid-cols-12 gap-8">
        <div class="col-span-4">
          <DsLabeledText
            label="App Details"
            text="Define the basic information about your application that will be visible to others."
          />
        </div>

        <div class="col-span-8">
          <DsTextInput
            :model-value="appName"
            type="text"
            label="App Name"
            placeholder=""
            :required="true"
            :disabled="false"
            :rules="[
              [v => !!v, 'App Name is required'],
              [v => v!.trim().length > 0, 'App Name is required'],
              [v => v!.length <= 200, 'App Name must be 200 characters or less']
            ]"
            @update:model-value="$emit('update:appName', $event ?? '')"
          />
        </div>
      </div>


      <template v-if="userType === SoFAppUserType.CdsHooks">
        <DsDivider />

        <div class="grid md:grid-cols-12 gap-8">
          <div class="col-span-4">
            <DsLabeledText
              label="CDS Hook Details"
              text="Specify how the CDS Hook service is accessed. You can share the CDS Service URL. You can also upload ELM files for a CQL to host in MeldRx CDS services"
            />
          </div>

          <div class="col-span-8 space-y-5">
            <DsSingleSelectButtonList
              v-model="state.cdsAppType"
              :options="cdsHookOptions"
            />
          </div>
        </div>

        <div v-if="state.cdsAppType == CdsAppType.External" class="grid md:grid-cols-12 gap-8">
          <div class="col-span-4">
            <DsLabeledText
              label="CDS Hook Service URL"
              text="Specify the Service URL for your CDS Hook Service"
            />
          </div>

          <div class="col-span-8 space-y-5">
            <DsTextInput
              v-model="state.createCdsHookAppCommand.cdsHookServiceUrl"
              type="text"
              label="CDS Hook Service URL"
              placeholder="https://sandbox-services.cds-hooks.org/cds-services/patient-greeting"
              :required="true"
              :disabled="false"
              :rules="ValidationRules.url"
            />
          </div>
        </div>

        <CdsHookAppElmForm
          v-else-if="state.cdsAppType === CdsAppType.Hosted"
          v-model="state.createCdsHookAppCommand"
        />

        <CdsHookAppCqlEditorForm
          v-else-if="state.cdsAppType == CdsAppType.HostedCustomCql"
          v-model="state.createCdsHookAppCommand"
        />

      </template>

      <template v-else>
        <!-- Authentication -->
        <template v-if="userType === SoFAppUserType.Patient || userType === SoFAppUserType.Provider">
          <DsDivider />
          <div class="grid md:grid-cols-12 gap-8">
            <div class="col-span-4">
              <DsLabeledText
                label="Authentication"
                text="Select the authentication method to secure your application and user data. This cannot be modified after the app has been created."
              />
            </div>

            <div class="col-span-8">
              <AuthenticationClientTypeSelect
                :model-value="authenticationClientType"
                label="Authentication Client Type"
                :required="true"
                @update:model-value="$emit('update:authenticationClientType', $event ?? SoFAppTokenAuthMethod.Public)"
              />
            </div>
          </div>

        </template>

        <!-- Scopes -->
        <DsDivider />

        <div class="grid md:grid-cols-12 gap-8">
          <div class="col-span-4">
            <DsLabeledText
              label="Scopes"
              text="Provide the scopes required by your application."
            />
          </div>

          <div class="col-span-8">
            <ScopeSelector
              :model-value="scopes"
              required
              :rules="[[(v?: string[]) => !!v && v.length > 0, 'Scopes are required']]"
              :scopes="userType === SoFAppUserType.System ? systemAppScopes : appScopes"
              @update:model-value="$emit('update:scopes', $event)"
            />
          </div>
        </div>

        <!-- Redirect URLs -->
        <template v-if="userType === SoFAppUserType.Patient || userType === SoFAppUserType.Provider">

          <DsDivider />

          <div class="grid md:grid-cols-12 gap-8">
            <div class="col-span-4">
              <DsLabeledText
                label="Redirect URLs"
                text="The URLs we will accept as destinations when returning authentication responses (tokens) after successfully authenticating or signing out users. Maximum 5 redirect URLs."
              />
            </div>

            <div class="col-span-8">
              <RedirectUrlList
                :model-value="redirectUrls"
                :limit="5"
                :rules="[
                [(v?: string[]) => !!v && v.length > 0, 'At least one Redirect URL is required'],
                [(v?: string[]) => !!v && ((new Set(v)).size === v.length), 'Cannot have duplicate Redirect URLs']
              ]"
                @update:model-value="$emit('update:redirectUrls', $event)"
              />
            </div>
          </div>
        </template>

        <template v-if="userType === SoFAppUserType.Provider || userType === SoFAppUserType.Patient">

          <DsDivider class="my-8" />

          <div class="grid md:grid-cols-12 gap-8">
            <div class="col-span-4">
              <DsLabeledText
                label="EHR Launch URL"
                text="If this app supports EHR Launch, please specify the URL"
              />
            </div>

            <div class="col-span-8 space-y-5">
              <DsTextInput
                :model-value="ehrLaunchUrl"
                type="text"
                label="EHR Launch URL"
                placeholder="https://myapp.com/launch"
                :required="false"
                :disabled="false"
                :rules="ValidationRules.url"
                @update:model-value="$emit('update:ehrLaunchUrl', $event)"
              />
            </div>
          </div>
        </template>

      </template>

      <DsDivider />

      <!-- Cancel/Next Buttons -->
      <div class="justify-start items-start gap-5 inline-flex">
        <DsButton id='cancel-button' :color="Colors.white" :text-color='Colors.gray' variant="subtle" @click="$emit('cancel');">
          Cancel
        </DsButton>
        <DsButton id='previous-step-button' :color="Colors.primary" :text-color='Colors.primary' variant="outline" @click="$emit('previous');">
          Previous Step
        </DsButton>
        <DsButton v-if="userType!==SoFAppUserType.CdsHooks" id='next-step-button' :color="Colors.primary" @click="onNextStep">
          Next Step
        </DsButton>
        <DsButton v-else-if="userType===SoFAppUserType.CdsHooks" id='register-app-button' :color="Colors.primary" @click="createApp">
          Register App
        </DsButton>
      </div>
    </div>

    <!-- Error Messages -->
    <div v-if="!!state.errorMessage">
      <div class="p-4">
        <DsText id="app-details-error" size="sm" :color="Colors.fire">
          {{ state.errorMessage }}
        </DsText>
      </div>
    </div>
  </DsForm>
</template>
