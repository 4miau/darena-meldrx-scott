<!--
    Input for a client secret.
    Just a wrapper of DsTextInput.

    Usage:
        <DsClientSecretInput
            v-model="clientSecret"
        />
-->

<script setup lang="ts">

defineProps<{
    modelValue?: string;
    required?: boolean;
    disabled?: boolean;
    rules?: ValidationRule<string>[];
}>();

defineEmits<{
  'update:modelValue': [value?: string]
}>();

</script>

<template>
  <DsTextInput
    type="password"
    label="Client Secret"
    placeholder=""
    :model-value="modelValue"
    :required="required"
    :disabled="disabled"
    :rules="rules"
    @update:model-value="($event: string | undefined) => $emit('update:modelValue', $event)"
  />
</template>
