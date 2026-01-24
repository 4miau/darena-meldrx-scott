<script setup lang="ts">
import { SecretType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/SecretType';
useHead({ title: 'Component Gallery | MeldRx' });

const clientSecret = ref<string | undefined>('clientSecret');
const jwksAlg = ref<string | undefined>('jwksAlg');
const jwksKid = ref<string | undefined>('jwksKid');
const secretType = ref<SecretType>(SecretType.ClientSecret);

const privateKeyValue = ref<FileList>();

const jwksKeyValue = ref<string>('');
const jwksAlgValue = ref<string>('');
const jwksKidValue = ref<string>('');
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        DsCredentialInput
      </DsText>
      <div class="pb-5" />

      <DsText size="xl">
        All Inputs Allowed
      </DsText>
      <DsCredentialInput
        v-model:client-secret="clientSecret"
        v-model:jwks-alg="jwksAlg"
        v-model:jwks-kid="jwksKid"
        v-model:secret-type="secretType"
        organization-id="organizationId"
        :show-none="true"
        :show-client-secret="true"
        :show-jwks-credential="true"
        :show-private-key="true"
        :show-hosted-jwks="true"
      />
      <div class="py-4" />
      <pre>
        Secret Type: {{ secretType }}
        Client Secret: {{ clientSecret }}
        JWKS Alg: {{ jwksAlg }}
        JWKS KID: {{ jwksKid }}
      </pre>
      <DsDivider />

      <DsText size="xl">
        Client Secret Only
      </DsText>
      <DsCredentialInput
        v-model:client-secret="clientSecret"
        v-model:jwks-alg="jwksAlg"
        v-model:jwks-kid="jwksKid"
        v-model:secret-type="SecretType.ClientSecret"
        organization-id="organizationId"
        :show-none="true"
        :show-client-secret="true"
        :show-jwks-credential="false"
        :show-private-key="false"
        :show-hosted-jwks="false"
      />
      <div class="py-4" />
      <pre>
        Secret Type: {{ secretType }}
        Client Secret: {{ clientSecret }}
        JWKS Alg: {{ jwksAlg }}
        JWKS KID: {{ jwksKid }}
      </pre>
      <DsDivider />

      <DsText size="xl">
        No Inputs Allowed
      </DsText>
      <DsCredentialInput
        v-model:client-secret="clientSecret"
        v-model:jwks-alg="jwksAlg"
        v-model:jwks-kid="jwksKid"
        v-model:secret-type="secretType"
        organization-id="organizationId"
        :show-none="true"
        :show-client-secret="false"
        :show-jwks-credential="false"
        :show-private-key="false"
        :show-hosted-jwks="false"
      />
      <div class="py-4" />
      <pre>
        Secret Type: {{ secretType }}
        Client Secret: {{ clientSecret }}
        JWKS Alg: {{ jwksAlg }}
        JWKS KID: {{ jwksKid }}
      </pre>
      <DsDivider />

      <DsText size="2xl" weight="light">
        DsPrivateKeyInput
      </DsText>
      <div class="pb-5" />

      <DsFileSelector
        v-model="privateKeyValue"
        :required="true"
        label="Private Key"
        placeholder-text="Upload a .pem file up to 5MB"
      />
      <pre>{{ privateKeyValue }}</pre>
      <DsDivider />

      <DsText size="2xl" weight="light">
        DsJwksCredentialInput
      </DsText>
      <div class="pb-5" />

      <DsJwksCredentialInput
        v-model:jwks-alg="jwksAlgValue"
        v-model:jwks-kid="jwksKidValue"
        v-model:client-secret="jwksKeyValue"
        :required="true"
      />
      <pre>
        Alg: {{ jwksAlgValue }}
        Kid: {{ jwksKidValue }}
        Key: {{ jwksKeyValue }}
      </pre>
      <DsDivider />
    </div>
  </DsContainer>
</template>
