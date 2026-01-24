<script setup lang="ts">

import {Colors} from "~/types/ui/colors";

const prop = withDefaults(
    defineProps<{
        id?: string;
        modelValue: string;
        label?: string;
        placeholder?: string;
        required?: boolean;
        rules?: ValidationRule<string>[];
        disabled?: boolean;
        minRows?: number;
        maxRows?: number;
    }>(),
    {
        id: '',
        label: '',
        placeholder: '',
        minRows: 1,
        maxRows: 13,
    }
);


defineEmits<{
  'update:modelValue': [value?: string];
  'enterPressed': [value?: string];
}>();

const { dirty, validationResult } = useValidation(
    () => prop.modelValue,
    () => prop.rules,
    () => prop.required ?? false
);
</script>

<template>
  <DsLabeledInput :label="prop.label" :required="prop.required">
    <div class="flex flex-col">
      <div class="border border-silver">
        <textarea
          :id="prop.id ? prop.id : (prop.label?.replaceAll(' ', '-').toLowerCase())"
          :value="modelValue"
          :placeholder="placeholder"
          :disabled="disabled"
          class="text-onyx text-sm focus-visible:outline-0 block w-full p-2.5 placeholder-silver"
          :rows="Math.min(Math.max(modelValue?.split('\n').length || minRows, minRows), maxRows)"
          @input="$emit('update:modelValue', (dirty = true, ($event.target as HTMLInputElement).value))"
          @keyup.enter="$emit('enterPressed')"
      />
      </div>
      <div v-if="!validationResult.isValid" class="relative mb-6">
        <DsText :id="prop.label?.replaceAll(' ', '-').toLowerCase()+'-error'" size="xs" :color="Colors.fire" class="absolute top-0">
          {{ validationResult.error }}
        </DsText>
      </div>
    </div>
  </DsLabeledInput>
</template>

