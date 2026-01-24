<!--
    Input for JWKS credentials.

    Usage:
        <DsJwksCredentialInput
            v-model:jwks-alg="jwksAlg"
            v-model:jwks-kid="jwksKid"
            v-model:client-secret="clientSecret"
        />
 -->

<script setup lang="ts">

defineProps<{
    jwksAlg?: string;
    jwksKid?: string;
    clientSecret?: string;
    required?: boolean;
}>();

const emit = defineEmits<{
    'update:jwksAlg': [value?: string];
    'update:jwksKid': [value?: string];
    'update:clientSecret': [value?: string];
}>();

const state = reactive<{
  secretFile?: FileList
}>({})

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

</script>

<template>
  <div class="w-full">
    <DsTextInput
      :model-value="jwksAlg"
      label="Signing Algorithm (alg)"
      :required="required"
      :rules="[[(v?: string) => !!v, 'Signing Algorithm is required']]"
      @update:model-value="$emit('update:jwksAlg', $event as string)"
    />
    <div class="py-2" />

    <DsTextInput
      :model-value="jwksKid"
      label="Key Id (kid)"
      :required="required"
      :rules="[[(v?: string) => !!v, 'Key Id is required']]"
      @update:model-value="$emit('update:jwksKid', $event as string)"
    />
    <div class="py-2" />

    <DsFileSelector
      :model-value="state.secretFile"
      :required="required"
      label="Private Key"
      placeholder-text="Upload a .pem file up to 5MB"
      :rules="[[(v?: string) => !!v, 'Certificate is required']]"
      @update:model-value="handleFileListEvent"
    />
  </div>
</template>
