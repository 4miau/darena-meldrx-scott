<script setup lang="ts">
import { Colors } from '~/types/ui/colors';

const props = defineProps<{
      modelValue?: string;
      label?: string;
      placeholder?: string;
      required?: boolean;
      rules?:ValidationRule<string>[];
      disabled?: boolean;
      type?: string;
}>();

const emit = defineEmits<{
   'update:modelValue': [value?: string];
   'copy': [];
}>();

function onCopy() {
    copyToClipboard(props.modelValue ?? '');
    emit('copy');
}
</script>

<template>
  <DsTextInput
    :model-value="modelValue"
    :label="label"
    :placeholder="placeholder"
    :required="required"
    :rules="rules"
    :disabled="disabled"
    :type="type"
    @update:model-value="$emit('update:modelValue', $event)"
  >
    <template #right>
      <DsButton
        class="ml-2"
        :color="Colors.primary"
        :text-color='Colors.primary'
        variant="outline"
        @click="onCopy"
      >
        <DsIcon name='heroicons:document-duplicate'/>
        Copy
      </DsButton>
    </template>
  </DsTextInput>
</template>
