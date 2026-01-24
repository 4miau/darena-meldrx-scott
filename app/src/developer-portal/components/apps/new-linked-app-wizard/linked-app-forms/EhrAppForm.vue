<script setup lang="ts">
import EpicSystemAppCredentialsForm from './EpicSystemAppCredentialsForm.vue';
import CernerSystemAppCredentialsForm from './CernerSystemAppCredentialsForm.vue';
import AthenaSystemAppCredentialsForm from './AthenaSystemAppCredentialsForm.vue';
import GenericPublicAppCredentialsForm from './GenericPublicAppCredentialsForm.vue';
import GenericConfidentialAppCredentialsForm from './GenericConfidentialAppCredentialsForm.vue';
import LinkedAppEditForm from './LinkedAppEditForm.vue';
import { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';
import type { INewLinkedApp } from '~/types/ui/apps/NewLinkedApp';
import { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod';

const props = defineProps<{
  organizationId: string;
  modelValue: INewLinkedApp;
}>();

const emit = defineEmits<{
  'update:modelValue': [value: INewLinkedApp];
}>();

const localAppDetails = ref<INewLinkedApp>({ ...props.modelValue });

watch(
    () => props.modelValue,
    (n) => {
        localAppDetails.value = { ...n }
    },
    { deep: true }
)

watch(
    () => localAppDetails.value,
    (newValue: INewLinkedApp, oldValue: INewLinkedApp) => {
        if (newValue === oldValue) {
            emit('update:modelValue', { ...newValue });
        }
    },
    { deep: true }
);

const formComponent = computed(() => {
    if (localAppDetails.value.soFAppUserType === SoFAppUserType.System) {
        if (localAppDetails.value.ehr === 'Epic') {
            return EpicSystemAppCredentialsForm;
        }
        if (localAppDetails.value.ehr === 'Cerner') {
            return CernerSystemAppCredentialsForm;
        }
        if (localAppDetails.value.ehr === 'AthenaHealth') {
            return AthenaSystemAppCredentialsForm;
        }
        return LinkedAppEditForm;
    }

    if (localAppDetails.value.soFAppTokenAuthMethod === SoFAppTokenAuthMethod.ClientSecretPost) {
        return GenericConfidentialAppCredentialsForm;
    }

    if (localAppDetails.value.soFAppUserType && [SoFAppUserType.Provider, SoFAppUserType.Patient].includes(localAppDetails.value.soFAppUserType)) {
        return GenericPublicAppCredentialsForm;
    }

    return LinkedAppEditForm;
});

</script>

<template>
  <component
    :is="formComponent"
    v-model:ehr="localAppDetails.ehr"
    v-model:fhir-api-provider-meld-rx-identifier="localAppDetails.fhirApiProviderMeldRxIdentifier"
    v-model:sof-app-user-type="localAppDetails.soFAppUserType"
    v-model:sof-app-token-auth-method="localAppDetails.soFAppTokenAuthMethod"
    v-model:app-name="localAppDetails.clientName"
    v-model:client-id="localAppDetails.clientId"
    v-model:client-secret="localAppDetails.clientSecret"
    v-model:scopes="localAppDetails.scopes"
    v-model:jwks-alg="localAppDetails.jwksAlg"
    v-model:jwks-kid="localAppDetails.jwksKid"
    v-model:secret-type="localAppDetails.secretType"
    v-model:is-shared-credential-type="localAppDetails.isSharedCredentialType"
    :organization-id="organizationId"

    :show-ehr="localAppDetails.ehr !== 'Other'"
  />
</template>
