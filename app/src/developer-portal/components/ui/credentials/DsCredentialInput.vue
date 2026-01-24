<script setup lang="ts">
import { SecretType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/SecretType'
import type { IDsSingleSelectButtonListItem } from '~/types/ui/DsSingleSelectButtonList'
import { Colors } from "~/types/ui/colors";

interface IRules {
  secretType?: SecretType;
  jwksAlg?: string;
  jwksKid?: string;
  clientSecret?: string;
}

const props = defineProps<{
    showNone: boolean;
    showClientSecret: boolean;
    showJwksCredential: boolean;
    showPrivateKey: boolean;
    showHostedJwks: boolean;

    secretType?: SecretType;
    jwksAlg?: string;
    jwksKid?: string;
    clientSecret?: string;
    organizationId: string;

    rules?: ValidationRule<IRules>[];
}>();

const emit = defineEmits<{
    'update:secretType': [value: SecretType];
    'update:jwksAlg': [value?: string];
    'update:jwksKid': [value?: string];
    'update:clientSecret': [value?: string];
}>();

const state = reactive<{
  secretFile?: FileList
}>({})

const jwksUrl = computed(() => `${location.origin}/api/jwks/${props.organizationId}`);

// Build out the options...
const credentialTypeOptions = computed<IDsSingleSelectButtonListItem<SecretType>[]>(() => {
    const result = [];

    if (props.showNone) {
        result.push({ value: SecretType.None, title: 'None' });
    }

    if (props.showClientSecret) {
        result.push({ value: SecretType.ClientSecret, title: 'Client Secret' });
    }

    if (props.showJwksCredential) {
        result.push({ value: SecretType.JsonWebKey, title: 'JWKS' });
    }

    if (props.showPrivateKey) {
        result.push({ value: SecretType.PrivateKey, title: 'Certificate' });
    }

    if (props.showHostedJwks) {
        result.push({ value: SecretType.HostedJwks, title: 'Hosted JWKS' });
    }

    return result;
});

// emit initial secret type value
if (props.secretType === undefined) {
    if (props.showNone) {
        onCredentialTypeChange(SecretType.None)
    } else if (props.showClientSecret) {
        onCredentialTypeChange(SecretType.ClientSecret)
    } else if (props.showJwksCredential) {
        onCredentialTypeChange(SecretType.JsonWebKey)
    } else if (props.showPrivateKey) {
        onCredentialTypeChange(SecretType.PrivateKey)
    } else if (props.showHostedJwks) {
        onCredentialTypeChange(SecretType.HostedJwks)
    }
}

async function handleFileListEvent(files?: FileList) {
    state.secretFile = files
    if(!files || files.length === 0)
    {
        emit('update:clientSecret', undefined);
        return;
    }

    const file = files.item(0)!;

    emit('update:clientSecret', await file.text());
}

// Occurs when the credential type (Client Secret, JWKS, Private Key, Hosted Jwks) changes...
function onCredentialTypeChange(event?: SecretType) {
    emit('update:secretType', event ?? SecretType.ClientSecret);
}
</script>

<template>
  <div>
    <!-- Only show the button list if more than one option is being shown -->
    <DsSingleSelectButtonList
      v-if="credentialTypeOptions.length > 1"
      :model-value="secretType"
      :options="credentialTypeOptions"
      label="Credential Type"
      required
      @update:model-value="onCredentialTypeChange"
    />
    <div v-if="credentialTypeOptions.length > 1" class="py-2" />

    <div>
      <DsClientSecretInput
        v-if="secretType === SecretType.ClientSecret"
        :model-value="clientSecret"
        @update:model-value="$emit('update:clientSecret', $event)"
      />

      <DsJwksCredentialInput
        v-if="secretType === SecretType.JsonWebKey"
        :jwks-alg="jwksAlg"
        :jwks-kid="jwksKid"
        :client-secret="clientSecret"
        required
        @update:client-secret="$emit('update:clientSecret', $event)"
        @update:jwks-alg="$emit('update:jwksAlg', $event)"
        @update:jwks-kid="$emit('update:jwksKid', $event)"
      />

      <DsFileSelector
        v-if="secretType === SecretType.PrivateKey"
        :model-value="state.secretFile"
        label="Private Key"
        placeholder-text="Upload a .pem file up to 5MB"
        required
        :rules="[[(v?: string) => !!v, 'Certificate is required']]"
        @update:model-value="handleFileListEvent"
      />

      <div
        v-if="secretType === SecretType.HostedJwks"
        class="text-sm"
      >
        <div class="p2 text-center">
          We will generate a private key and host the JWKS for you at this URL:<br>
        </div>
        <DsLink :href="jwksUrl" underline="hover" target="_blank" class="flex justify-center">
          <DsText size="sm" weight="light" :color="Colors.secondary" class="underline text-center">
            {{ jwksUrl }}
          </DsText>
        </DsLink>
      </div>
    </div>
  </div>
</template>
