<script setup lang="ts">
import type { IDsSingleSelectButtonListItem } from '~/types/ui/DsSingleSelectButtonList';
import { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod';
import AppUtils from '~/utils/AppUtils';

defineProps<{
    modelValue?: SoFAppTokenAuthMethod;
    label?: string;
    required?: boolean;
}>();

defineEmits<{
  'update:modelValue': [value?: SoFAppTokenAuthMethod]
}>();

const options: IDsSingleSelectButtonListItem<SoFAppTokenAuthMethod>[] = [
    { value: SoFAppTokenAuthMethod.Public, title: AppUtils.authMethodDisplayString(SoFAppTokenAuthMethod.Public) },
    { value: SoFAppTokenAuthMethod.ClientSecretPost, title: AppUtils.authMethodDisplayString(SoFAppTokenAuthMethod.ClientSecretPost) }
];
</script>

<template>
  <DsSingleSelectButtonList
    :model-value="modelValue"
    :options="options"
    :label="label"
    :required="required"
    @update:model-value="(e) => $emit('update:modelValue', e)"
  />
</template>
