<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import type { INewLinkedApp, LinkedApp } from '~/types/ui/apps/NewLinkedApp';
import { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';
import type FhirApiProviderDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';
import type LinkedAppDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/LinkedAppDto';
import { DynamicAuthMethods } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/DynamicAuthMethods';
import type DynamicRegistrationDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/DynamicRegistrationDto';
import { appScopes, systemAppScopes } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Scopes';
import { SecretTypes } from '~/types/common/IdentityServerConstants';

const props = defineProps<{
  app: DynamicRegistrationDto,
  linkedApps: LinkedAppDto[];
  fhirApiProviders: FhirApiProviderDto[];
  organizationId: string;
}>();

const emit = defineEmits<{
  'addLinkedApp': [appDetails?: INewLinkedApp];
  'editLinkedApp': [appDetails: LinkedApp];
  'deleteLinkedApp': [linkedAppId: string];

  'createSecret': [];
  'deleteSecret': [secretId: number];

  'save': [DynamicRegistrationDto];
  'cancel': [];
}>();

const formRef = ref<FormRef>();
const state = reactive<{
  errorMessage: string;
  selectedLinkedApp?: LinkedApp;
  showNewLinkedAppModal: boolean;
  appForm: DynamicRegistrationDto;
}>({
    errorMessage: '',
    showNewLinkedAppModal: false,
    appForm: props.app
});

function onOpenModalClick () {
    state.showNewLinkedAppModal = true;
}

function selectAppToEdit (linkedAppIndex: number) {
    state.selectedLinkedApp = LinkedAppUtils.linkedAppDtoToLinkedApp(props.linkedApps[linkedAppIndex]);
}

// When a linked app is saved...
function onSaveLinkedApp (newLinkedApp: INewLinkedApp) {
    const linkedApp = newLinkedApp as LinkedApp;
    emit('editLinkedApp', linkedApp);
    state.selectedLinkedApp = undefined;
}

function deleteLinkedApp (linkedAppIndex: number) {
    if (props.linkedApps) {
        const linkedApp = props.linkedApps[linkedAppIndex];
        emit('deleteLinkedApp', linkedApp.id);
    }
}

// "Save" button on the whole form...
function onSave () {
    // Try to validate the form...
    if (!formRef.value) {
        return;
    }
    const isFormValid = formRef.value?.validate();
    if (!isFormValid) {
        state.errorMessage = 'Please fix the errors above.';
        return;
    }

    emit('save', state.appForm);
}

</script>

<template>
  <DsForm ref="formRef">
    <div class="space-y-5">
      <!-- App Details -->
      <div class="grid md:grid-cols-12 gap-8">
        <div class="col-span-4">
          <DsLabeledText
            label="App Details"
            text="Define the basic information about your application that will be visible to others."
          />
        </div>

        <div class="col-span-8 space-y-5">
          <DsLabeledText v-if="!!state.appForm.client_id" label="App ID / Client ID">
            <DsTextWithCopyButton
              id="client-id"
              size="xs"
              :text="`${state.appForm.client_id}`"
              :text-to-copy="state.appForm.client_id"
              :show-toast-on-copy="true"
              toast-message-on-copy="Copied App ID / Client ID to clipboard."
            />
          </DsLabeledText>

          <DsTextInput
            v-model="state.appForm.client_name"
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
          />
        </div>
      </div>

      <DsDivider />
      <!-- User Information -->
      <div class="grid md:grid-cols-12 gap-8">
        <div class="col-span-4">
          <DsLabeledText
            label="User Information"
            text="The primary user base of this app / the app type."
          />
        </div>
        <div class="col-span-8">
          <DsLabeledText
            label="App Type"
            :text="state.appForm.soFAppUserType"
          />
        </div>

        <template v-if="state.appForm.soFAppUserType == SoFAppUserType.Patient || state.appForm.soFAppUserType == SoFAppUserType.Provider">
          <div class="col-span-4">
            <DsLabeledText
              label="Authentication"
              text="Authentication method to secure your application and user data."
            />
          </div>

          <div class="col-span-8">
            <DsLabeledText
              label="Authentication Client Type"
              :text="AppUtils.dynamicAuthMethodDisplayString(state.appForm.token_endpoint_auth_method)"
            />
          </div>
        </template>
      </div>


      <DsDivider />

      <!-- Scopes -->
      <div class="grid md:grid-cols-12 gap-8">
        <div class="col-span-4">
          <DsLabeledText
            label="Scopes"
            text="Provide the scopes required by your application."
          />
        </div>

        <div class="col-span-8">
          <ScopeSelector
            v-model="state.appForm.scope"
            required
            :rules="[[(v?: string[]) => !!v && v.length > 0, 'Scopes are required']]"
            :scopes="app.soFAppUserType === SoFAppUserType.System ? systemAppScopes : appScopes"
          />
        </div>
      </div>

      <template v-if="state.appForm.soFAppUserType == SoFAppUserType.Patient || state.appForm.soFAppUserType == SoFAppUserType.Provider">

        <!-- Redirect URLs -->
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
              v-model="state.appForm.redirect_uris"
              :limit="5"
              :rules="[
                [(v?: string[]) => !!v && v.length > 0, 'At least one Redirect URL is required'],
                [(v?: string[]) => !!v && ((new Set(v)).size === v.length), 'Cannot have duplicate Redirect URLs']
              ]"
            />
          </div>
        </div>

        <DsDivider />

        <div class="grid md:grid-cols-12 gap-8">
          <div class="col-span-4">
            <DsLabeledText
              label="EHR Launch URL"
              text="Specify the EHR Launch URL if your app can be launched using SMART on FHIR EHR Launch"
            />
          </div>

          <div class="col-span-8 space-y-5">
            <DsTextInput
              v-model="state.appForm.ehrLaunchUrl"
              type="text"
              label="EHR Launch URL"
              placeholder="https://mycoolapp.com/launch"
              :required="false"
              :disabled="false"
              :rules="ValidationRules.url"
            />
          </div>
        </div>

      </template>

      <DsDivider />

      <div class="grid md:grid-cols-12 gap-8">
        <div class="col-span-4">
          <DsLabeledText
            label="Connect Linked Apps"
            text="Select the external systems your application will interact with."
          />
        </div>

        <div class="col-span-8">
          <LinkedAppList
            :linked-apps="linkedApps.map(x => LinkedAppUtils.soFAppBaseDtoToNewLinkedApp(x))"
            @add-linked-app="onOpenModalClick"
            @edit-linked-app="selectAppToEdit"
            @delete="deleteLinkedApp"
          />
        </div>
      </div>

      <template v-if="(app.token_endpoint_auth_method === DynamicAuthMethods.ClientSecretPost) || (app.client_secret.length > 0)">
        <DsDivider class="my-8" />

        <div class="grid md:grid-cols-12 gap-8">
          <div class="col-span-4">
            <DsLabeledText
              label="Manage Secrets"
              text="Manage your Client secrets"
            />
          </div>

          <div class="col-span-8">
            <!-- Secrets -->
            <DsLabeledInput label="Secrets" required>
              <div
                v-for="(secret, index) in app.client_secret.filter(x => x.secretType === SecretTypes.SharedSecret)"
                :key="secret.secret"
                class="flex flex-row items-start items-center border-b border-bliss py-2"
              >
                <DsText size="md">
                  {{ secret.secret ?? '***************************' }}
                </DsText>
                <DsButton
                  :id="'delete-secret-' + index + '-button'" :text-color='Colors.black' variant="subtle" :color="Colors.white"
                  @click="$emit('deleteSecret', secret.id)"
                >
                    X
                </DsButton>
              </div>
              <DsButton
                id="new-secret-button"
                variant="transparent"
                :color="Colors.fire"
                :text-color='Colors.fire'
                @click="$emit('createSecret')"
              >
              + New Secret
              </DsButton>
            </DsLabeledInput>

            <div class="pb-5" />

            <!-- JWKS URI -->
            <div>
              <DsTextInput
                v-model="state.appForm.jwks_uri"
                label="JWKS URI"
                type="text"
                placeholder="JWKS URL"
                :required="false"
                :disabled="false"
                :rules="ValidationRules.url"
              />
            </div>
          </div>
        </div>
      </template>

      <DsDivider />

      <!-- Save/Cancel Buttons -->
      <div class="justify-start items-start gap-5 inline-flex">
        <DsButton id="cancel-button" :color="Colors.white" :text-color='Colors.gray' variant="subtle" @click="$emit('cancel');">
          Cancel
        </DsButton>
        <DsButton id="save-button" :color="Colors.primary" @click="onSave">
          Save
        </DsButton>
      </div>
    </div>
  </DsForm>

  <!-- New Linked App Modal-->
  <NewLinkedAppModal
    :show="state.showNewLinkedAppModal"
    :auth-method="AppUtils.dyanmicAuthMethodsToSoFAppTokenAuthMethod(app.token_endpoint_auth_method)"
    :user-type="app.soFAppUserType"
    :linked-apps="linkedApps.map(x => LinkedAppUtils.soFAppBaseDtoToNewLinkedApp(x))"
    :fhir-api-providers="fhirApiProviders"
    :current-linked-apps="linkedApps.map(x => LinkedAppUtils.soFAppBaseDtoToNewLinkedApp(x))"
    :organization-id="organizationId"
    @add-linked-app="$emit('addLinkedApp', $event)"
    @close="state.showNewLinkedAppModal = false"
  />

  <!-- Edit Linked App Modal-->
  <EditLinkedApp
    v-if="state.selectedLinkedApp"
    :model-value="state.selectedLinkedApp"
    :show="!!state.selectedLinkedApp"
    :fhir-api-providers="fhirApiProviders"
    :current-linked-apps="linkedApps.map(x => LinkedAppUtils.soFAppBaseDtoToNewLinkedApp(x))"
    :organization-id="organizationId"
    @save="onSaveLinkedApp"
    @close="state.selectedLinkedApp=undefined"
  />

  <!-- Error Messages -->
  <div v-if="!!state.errorMessage">
    <div class="p-4">
      <DsText size="sm" color="text-fire">
        {{ state.errorMessage }}
      </DsText>
    </div>
  </div>

</template>
