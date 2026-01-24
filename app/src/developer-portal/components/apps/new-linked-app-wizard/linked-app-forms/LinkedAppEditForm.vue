<script setup lang="ts">
import type { EHRs } from '~/types/ehrs';
import type FhirApiProviderDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';
import { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod';
import { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';
import type { SecretType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/SecretType';
import type { INewLinkedApp } from '~/types/ui/apps/NewLinkedApp';
import { defaultScopes, systemLinkedAppScopes } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Scopes';
import type SharedEhrCredentialView from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SharedEhrCredentialDto';
import type { IDsSelectItem } from '~/types/ui/DsSelect';

interface IProps {
    showEhr?: boolean;
    showSofAppUserType?: boolean;
    showSofAppTokenAuthMethod?: boolean;
    showAppName?: boolean;
    showClientId?: boolean;
    showNone?: boolean;
    showClientSecret?: boolean;
    showJwksCredential?: boolean;
    showPrivateKey?: boolean;
    showHostedJwks?: boolean;
    showScopes?: boolean;
    fhirApiProviders?: FhirApiProviderDto[];
    currentLinkedApps: INewLinkedApp[];
    sharedCredential?: SharedEhrCredentialView;
    isSharedCredentialType?: boolean;

    ehr?: EHRs;
    fhirApiProviderMeldRxIdentifier?: string;
    sofAppUserType?: SoFAppUserType;
    sofAppTokenAuthMethod?: SoFAppTokenAuthMethod;
    appName?: string;
    clientId?: string;
    clientSecret?: string;
    scopes?: string;
    jwksAlg?: string;
    jwksKid?: string;
    secretType?: SecretType;
    organizationId?: string;
};

// Set default properties...
const props = withDefaults(defineProps<IProps>(), {
    showEhr: true,
    showSofAppUserType: true,
    showSofAppTokenAuthMethod: true,
    showAppName: true,
    showClientId: true,
    showNone: true,
    showClientSecret: true,
    showJwksCredential: true,
    showPrivateKey: true,
    showHostedJwks: true,
    showScopes: true,
    fhirApiProviders: () => [],
    currentLinkedApps: () => [],
    sharedCredential: undefined,
    isSharedCredentialType: false,

    ehr: undefined,
    fhirApiProviderMeldRxIdentifier: undefined,
    sofAppUserType: undefined,
    sofAppTokenAuthMethod: undefined,
    appName: undefined,
    clientId: undefined,
    clientSecret: undefined,
    scopes: undefined,
    jwksAlg: undefined,
    jwksKid: undefined,
    secretType: undefined,
    organizationId: ''
});

defineEmits<{
    'update:ehr': [value?: EHRs];
    'update:fhirApiProviderMeldRxIdentifier': [value?: string];
    'update:sofAppUserType': [value?: SoFAppUserType];
    'update:sofAppTokenAuthMethod': [value?: SoFAppTokenAuthMethod];
    'update:appName': [value?: string];
    'update:clientId': [value?: string];
    'update:clientSecret': [value?: string];
    'update:scopes': [value?: string];
    'update:jwksAlg': [value?: string];
    'update:jwksKid': [value?: string];
    'update:secretType': [value?: SecretType];
}>();

const useOwnCredential = computed(() => {
    return !props.isSharedCredentialType;
})
const authOptions = ref<IDsSelectItem<SoFAppTokenAuthMethod>[]>([
    { value: SoFAppTokenAuthMethod.Public, title: 'Public' },
    { value: SoFAppTokenAuthMethod.ClientSecretPost, title: 'Confidential Client' }
]);

</script>

<template>
  <div class="flex flex-col w-full">
    <!-- Heading / EHR Logo -->
    <div class="w-full flex-col justify-center items-center gap-5 flex">
      <DsText size="2xl" weight="light">
        Linked App Details
      </DsText>
      <div v-if="showEhr" class="place-self-center">
        <EHRLogo :ehr-type="ehr" class="w-20" />
      </div>

      <DsText v-if="isSharedCredentialType" size="sm" weight="light" class="text-center">
        Provide the details to establish a secure connection using MeldRx sample credentials.
      </DsText>
      <DsText v-else size="sm" weight="light">
        Provide the details to establish a secure connection.
      </DsText>
    </div>

    <!-- Fhir Api Provider -->
    <DsLabeledText
      label="FHIR API Provider"
      :text="FhirApiProviderUtils.getDisplayStringFromId(fhirApiProviders, fhirApiProviderMeldRxIdentifier!)"
    />

    <!-- Auth Method / User Type -->
    <div class="py-4 grid grid-cols-2 gap-4">
      <div v-if="sofAppUserType !== SoFAppUserType.System && !isSharedCredentialType">
        <DsSelect
          label="Authentication Method"
          :model-value="sofAppTokenAuthMethod"
          :items="authOptions"
          @update:model-value="(e) => $emit('update:sofAppTokenAuthMethod', e)"
        />
      </div>
      <div>
        <DsLabeledInput v-if="showSofAppUserType" label="User Type">
          <DsText size="sm" weight="light">
            {{ AppUtils.userTypeDisplayString(sofAppUserType) }}
          </DsText>
        </DsLabeledInput>
      </div>
    </div>

    <div class="grid grid-cols-1 gap-4">
      <!-- Name -->
      <DsTextInput
        v-if="showAppName"
        :required="showAppName"
        :rules="[[v => !!v, 'Connection Name is required']]"
        :model-value="appName"
        type="text"
        label="Connection Name"
        @update:model-value="$emit('update:appName', $event ?? '')"
      >
        <template #popoverarea>
          <DsIcon name="heroicons:information-circle" size="sm" />
        </template>
        <template #popovercontent>
          <DsText size="sm">
            This is the display name of your linked application.
          </DsText>
        </template>
      </DsTextInput>

      <!-- Client Id -->
      <DsTextInput
        v-if="showClientId && useOwnCredential"
        :required="showClientId"
        :rules="[[v => !!v, 'Client Id is required']]"
        :model-value="clientId"
        type="text"
        label="Client Id"
        @update:model-value="$emit('update:clientId', $event ?? '')"
      >
        <template #popoverarea>
          <DsIcon name="heroicons:information-circle" size="sm" />
        </template>
        <template #popovercontent>
          <DsText size="sm">
            This is the Client Id of the application that you are linking with.
          </DsText>
        </template>
      </DsTextInput>

      <!-- Credentials -->
      <DsCredentialInput
        v-if="(showNone || showClientSecret || showJwksCredential || showPrivateKey) && useOwnCredential"
        :show-none="showNone"
        :show-client-secret="showClientSecret"
        :show-jwks-credential="showJwksCredential"
        :show-private-key="showPrivateKey"
        :show-hosted-jwks="showHostedJwks"

        :secret-type="secretType"
        :client-secret="clientSecret"
        :jwks-alg="jwksAlg"
        :jwks-kid="jwksKid"
        :organization-id="organizationId"

        @update:secret-type="$emit('update:secretType', $event)"
        @update:client-secret="$emit('update:clientSecret', $event ?? '')"
        @update:jwks-alg="$emit('update:jwksAlg', $event ?? '')"
        @update:jwks-kid="$emit('update:jwksKid', $event ?? '')"
      />

      <!-- Scopes -->
      <ScopeSelector
        v-if="showScopes"
        :model-value="(scopes as string)"
        required
        :rules="[[(v?: string[]) => !!v && v.length > 0, 'Scopes are required']]"
        :scopes="sofAppUserType === SoFAppUserType.System ? systemLinkedAppScopes : defaultScopes"
        @update:model-value="$emit('update:scopes', $event)"
      />
    </div>
  </div>
</template>
