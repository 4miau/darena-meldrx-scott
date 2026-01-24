<script setup lang="ts">
import {Colors} from '~/types/ui/colors';
import type {
    CreateWorkspaceExternalApp,
    UpdateWorkspaceExternalApp,
    WorkspaceExternalApp
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceExternalApp";
import {defaultScopes} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Scopes";
import {SecretType} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/SecretType";
import {
    SoFAppTokenAuthMethod
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod";
import {SoFAppUserType} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType";

const props = defineProps<{
  show: boolean,
  externalApp?: WorkspaceExternalApp;
  organizationId: string;
}>();

function createDefaultExternalApp(): CreateWorkspaceExternalApp | UpdateWorkspaceExternalApp{
  
    if(props.externalApp) {
        return {...props.externalApp}
    }
    
    return {
        soFAppUserType: SoFAppUserType.Provider,
        soFAppTokenAuthMethod: SoFAppTokenAuthMethod.Public,
        clientName: '',
        clientId: '',
        scopes: '',
        secretType: SecretType.None,
    }
}

const state = reactive<{
  form: CreateWorkspaceExternalApp | UpdateWorkspaceExternalApp;
  authType: string,
}>({
    form: createDefaultExternalApp(),
    authType: 'Public'
});

// Watch for changes in `props.externalApp` and update `state.form`
watch(() => props.show, (value) => {
    if(value){
        state.form = createDefaultExternalApp()
    }
});

const emit = defineEmits<{
  'editExternalApp': [value: UpdateWorkspaceExternalApp];
  'addExternalApp': [appDetails: CreateWorkspaceExternalApp];
  'close': [];
}>();

function onSubmit() {
    if (!formRef.value) return;
    const isValid = formRef.value.validate();
    if (!isValid) return;

    if (props.externalApp) {
        emit('editExternalApp', state.form as UpdateWorkspaceExternalApp); // Editing existing app
    } else {
        emit('addExternalApp', state.form as CreateWorkspaceExternalApp); // Creating new app
    }
}

function updateAuthType(value?:string | string[]){
    if(typeof value !== 'string'){
        return
    }
    
    if(value === "Public")
    {
        state.form.soFAppTokenAuthMethod = SoFAppTokenAuthMethod.Public
        state.form.secretType = SecretType.None
    }
    else if (value === "Confidential")
    {
        state.form.soFAppTokenAuthMethod = SoFAppTokenAuthMethod.ClientSecretPost
        state.form.secretType = SecretType.ClientSecret
    }
    state.authType = value
}

const formRef = ref<FormRef>();

</script>

<template>
  <DsModal :model-value="props.show" @close="$emit('close')">
    <DsModalProgressCard :total-steps="1" :current-step="1">
    <div class="flex w-full py-8 bg-space justify-center items-center">
      <div class="flex w-full items-center justify-center">
        <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center">
          <MeldRxApp/>
        </div>
      </div>
      <div class="flex-1"/>
    </div>
    <div class="flex flex-col w-full py-5 px-10">
      
      <div class="w-full flex-col justify-center items-center gap-5 flex">
        <DsText size="2xl" weight="light">
          External App Details
        </DsText>

        <DsText size="sm" weight="light">
          Provide the details as they appear in the external system.
        </DsText>
      </div>
      <!-- App Details -->
      <DsForm ref="formRef">
        <div  class="flex flex-col space-y-2">

          <!-- Authentication Client Type -->
          <DsLabeledInput label="Authentication Client Type" required>
            <DsButtonGroup
                :model-value="state.authType"
                :active-color="Colors.primary"
                :active-text-color='Colors.onyx'
                :rounded="false"
                @update:model-value="updateAuthType"
            >
              <DsButton value="Public" size="sm" variant="outline" :text-color='Colors.onyx'>
                Public
              </DsButton>
              <DsButton value="Confidential" size="sm" variant="outline" :text-color='Colors.onyx'>
                Confidential
              </DsButton>
            </DsButtonGroup>
          </DsLabeledInput>

          <!-- Client Name -->
          <DsTextInput
              v-model="state.form.clientName"
              required
              :rules="[[v => !!v, 'Client Name is required']]"
              type="text"
              label="Client Name"
          />

          <!-- Client Id -->
          <DsTextInput
              v-model="state.form.clientId"
              required
              :rules="[[v => !!v, 'Client Id is required']]"
              type="text"
              label="Client Id"
          />

          <!-- Credentials -->
          <DsCredentialInput
              v-if="(state.form.secretType === SecretType.ClientSecret)"
  
              v-model:client-secret="state.form.clientSecret"
              v-model:jwks-alg="state.form.jwksAlg"
              v-model:jwks-kid="state.form.jwksKid"
              v-model:secret-type="state.form.secretType"
              :organization-id="props.organizationId"
  
              :show-none="false"
              :show-client-secret="false"
              :show-jwks-credential="false"
              :show-private-key="false"
              :show-hosted-jwks="false"
          />
          
          <!-- Scopes -->
          <ScopeSelector
              v-model="(state.form.scopes as string)"
              required
              :rules="[[(v?: string[]) => !!v && v.length > 0, 'Scopes are required']]"
              :scopes="defaultScopes"
          />
        </div>
      </DsForm>

      <DsDivider/>

      <!-- Buttons -->
      <div class="flex justify-center w-full gap-5">
        <DsButton variant="outline" :color="Colors.secondary" :text-color='Colors.secondary' @click="$emit('close')">
          Close
        </DsButton>
        <DsButton :color="Colors.secondary" @click="onSubmit">
          {{ props.externalApp ? 'Update App' : 'Add External App' }}
        </DsButton>
      </div>
    </div>
    </DsModalProgressCard>
  </DsModal>
</template>
