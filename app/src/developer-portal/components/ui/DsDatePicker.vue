<!--
    DsDatePicker
    Usage:
        <DsDatePicker v-model="formModel.dateOfBirth" />
-->
<script setup lang="ts">
import { Colors } from "~/types/ui/colors";

const props = withDefaults(defineProps<{
  modelValue?: string;
  pastOnly?: boolean;
  required?: boolean;
  rules?: ValidationRule<string>[];
}>(), {
    modelValue: '',
    pastOnly: false,
    required: false,
    rules: undefined
});

const _emit = defineEmits<{
  'update:modelValue': [string?]
}>();

const maxDate = DateTime.nowDate('iso');

const { validationResult } = useValidation(
    () => props.modelValue ?? "",
    () => props.rules,
    () => props.required ?? false
);
</script>

<template>
  <div class="justify-start items-start w-full flex-col">
    <input
      id="date-picker"
      :value="modelValue"
      type="date"
      class="rounded-none border border-silver p-2"
      :required="props.required"
      :max="props.pastOnly ? maxDate : undefined"
      @input="($event) => {
        const rawDateValue = ($event.target as HTMLInputElement).value;
        _emit('update:modelValue',rawDateValue);
      }"
    >
    <div v-if="!validationResult.isValid" class="relative mb-6">
      <DsText size="xs" :color="Colors.fire" class="absolute top-0">
        {{ validationResult.error }}
      </DsText>
    </div>
  </div>
</template>
