<!--
    Text input.
    You can put a label above it.
    You can mark an input as required.

    Usage:
        <DsTextInput
            v-model="title"
            type="text"
            label="Text Input Label"
            placeholder="DsTextInput"
            :required="true"
            :rules="[...]"
            :disabled="false"
        />

    validation:
      :rules=[
        syntax:
          [(modelValue) => modelValue === 'check', "Validation Error"],
        examples:
          [(v) => !!v, "Name cannot be empty."],
          [(v) => v.length < 100, "Name cannot be more than 100 characters."]
      ]
      - if check on the left is true - validation successful
      - if validation is false - error message is displayed
      - evaluation of rules is short circuited at first error
      - by default validation errors aren't shown
 -->

<script setup lang="ts" generic="T">
import { Colors } from '~/types/ui/colors';

const prop = defineProps<{
     id?: string;
     modelValue?: T;
     label?: string;
     placeholder?: string;
     required?: boolean;
     rules?: ValidationRule<T>[];
     disabled?: boolean;
     type?: string;
 }>();

defineEmits<{
  'update:modelValue': [value?: T];
  'enterPressed': [value?: T];
}>();

const { dirty, validationResult } = useValidation(
    () => prop.modelValue,
    () => prop.rules,
    () => prop.required ?? false
);
</script>

<template>
  <DsLabeledInput :label="label" :required="required">
    <template v-if="$slots.popoverarea" #popoverarea>
      <slot name="popoverarea" />
    </template>
    <template v-if="$slots.popovercontent" #popovercontent>
      <slot name="popovercontent" />
    </template>
    <div class="flex flex-col">
      <!-- Text Input / Right Component -->
      <div class="flex flex-row">
        <!-- Textbox, Grow to fill remaining space -->
        <div class="grow">
          <input
            :id="prop.id ? prop.id : (prop.label?.replaceAll(' ', '-').toLowerCase())"
            :value="modelValue"
            :placeholder="placeholder"
            :required="required"
            :disabled="disabled"
            :type="type ?? 'text'"
            class="border border-silver text-onyx text-sm focus-visible:outline-0 block w-full p-2.5 placeholder-silver"
            :class="{'bg-bliss': disabled}"
            @input="$emit('update:modelValue', (dirty = true, ($event.target as HTMLInputElement).value as T))"
            @keyup.enter="$emit('enterPressed')"
          >
        </div>

        <!-- Content to the right of the textbox -->
        <div  class="flex items-stretch border-silver">
          <slot name="right" />
        </div>
      </div>
      <div v-if="!validationResult.isValid" class="relative mb-6">
        <DsText :id="prop.label?.replaceAll(' ', '-').toLowerCase()+'-error'" size="xs" :color="Colors.fire" class="absolute top-0">
          {{ validationResult.error }}
        </DsText>
      </div>
    </div>
  </DsLabeledInput>
</template>