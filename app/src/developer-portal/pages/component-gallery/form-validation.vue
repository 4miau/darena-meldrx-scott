<script setup lang="ts">
import type { INewLinkedApp } from '~/types/ui/apps/NewLinkedApp';
import { defaultScopes } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Scopes';
useHead({ title: 'Component Gallery | MeldRx' });

const dummyForm = ref({
    one: '',
    two: '',
    three: '',
    secret: ''
})
const formRef = ref<FormRef>()
const isFormValid = ref(true)
function validateForm() {
    isFormValid.value = formRef.value!.validate()
}

const scopes = ref<string>('');
const scopeSelectorFormRef = ref<FormRef>();
const isScopeSelectorFormValid = ref(true);
function validateScopeSelectorForm() {
    isScopeSelectorFormValid.value = scopeSelectorFormRef.value!.validate();
}

const privateKey = ref<FileList>();
const privateKeyFormRef = ref<FormRef>();
const isPrivateKeyFormValid = ref(true);
function validatePrivateKeyForm() {
    isPrivateKeyFormValid.value = privateKeyFormRef.value!.validate();
}

const dsCredentialsFormRef = ref<FormRef>();
const isDsCredentialsFormValid = ref(true);
function validateCredentialsInputForm() {
    isDsCredentialsFormValid.value = dsCredentialsFormRef.value!.validate();
}

const linkedApp = ref<INewLinkedApp>({
    clientId: '',
    clientName: '',
    clientSecret: '',
    ehr: 'Other',
    fhirApiProviderMeldRxIdentifier: '',
    jwksAlg: '',
    jwksKid: '',
    scopes: '',
    secretType: undefined,
    soFAppTokenAuthMethod: undefined,
    soFAppUserType: undefined
});
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        Form Validation
      </DsText>
      <div class="pb-5" />

      <!-- Text Inputs -->
      <DsText size="xl">
        Validation on Text Inputs (DsTextInput, DsClientSecretInput)
      </DsText>

      <DsForm ref="formRef">
        <DsTextInput v-model="dummyForm.one" class="mb-4" label="No validation" />
        <DsTextInput v-model="dummyForm.two" class="mb-4" label="Cannot be empty" required :rules="[[(v) => !!v, 'not empty']]" />
        <DsTextInput v-model="dummyForm.three" class="mb-4" label="At leat 10 characters" required :rules="[[(v) => !!v, 'not empty'], [(v) => v!.length > 10, 'at least 10 characters']]" />
        <DsClientSecretInput v-model="dummyForm.secret" class="mb-4" label="Cannot be empty" required :rules="[[(v) => !!v, 'not empty']]" />
        <DsButton @click="validateForm">
          Validate
        </DsButton>
        <div class="pb-4" />
        <pre>Form Values: {{ dummyForm }}
Is Form Valid?: {{ isFormValid }}
        </pre>
      </DsForm>
      <DsDivider />

      <!-- Scope Selector -->
      <DsText size="xl">
        ScopeSelector
      </DsText>

      <DsForm ref="scopeSelectorFormRef">
        <ScopeSelector
          required
          :rules="[[(v?: string[]) => { return (!!v) && (v.length > 0); }, 'At least one scope is required']]"
          :model-value="scopes"
          :scopes="defaultScopes"
          @update:model-value="scopes = $event"
        />
        <div class="pb-4" />
        <DsButton @click="validateScopeSelectorForm">
          Validate
        </DsButton>
        <div class="pb-4" />
        <pre>Is Form Valid?: {{ isScopeSelectorFormValid }}</pre>
      </DsForm>
      <DsDivider />

      <!-- Private Key Input -->
      <DsText size="xl">
        DsPrivateKeyInput
      </DsText>

      <DsForm ref="privateKeyFormRef">
        <DsFileSelector
          v-model="privateKey"
          label="Private Key"
          placeholder-text="Upload a .pem file up to 5MB"
          :rules="[[(v?: string) => { return !!v; }, 'Certificate is required']]"
        />
        <div class="pb-4" />
        <DsButton @click="validatePrivateKeyForm">
          Validate
        </DsButton>
        <div class="pb-4" />
        <pre>Is Form Valid?: {{ isPrivateKeyFormValid }}</pre>
      </DsForm>
      <DsDivider />

      <!-- DsCredentialsInput -->
      <DsText size="xl">
        EhrAppForm
      </DsText>

      <DsForm ref="dsCredentialsFormRef">
        <div style="width:50%;">
          <EhrAppForm
            v-model="linkedApp"
            :fhir-api-providers="[]"
            :organization-id="'my-org'"
          />
        </div>

        <div class="pb-4" />
        <DsButton @click="validateCredentialsInputForm">
          Validate
        </DsButton>
        <div class="pb-4" />
        <pre>Is Form Valid?: {{ isDsCredentialsFormValid }}
Form Values: {{ linkedApp }}
        </pre>
      </DsForm>
      <DsDivider />
    </div>
  </DsContainer>
</template>
