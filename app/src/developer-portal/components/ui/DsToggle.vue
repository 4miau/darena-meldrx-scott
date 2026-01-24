<script setup lang="ts">
defineProps<{
  modelValue: boolean;
}>();

defineEmits<{
  'update:modelValue': [value: boolean];
}>();
</script>

<template>
  <div
    class="w-9 h-5 rounded-xl p-0.5 relative cursor-pointer select-none transition-colors duration-300"
    :class="{ 'bg-dsgray': !modelValue, 'bg-dsprimary': modelValue }"
    @click="$emit('update:modelValue', !modelValue)"
  >
    <div
      class="h-4 bg-white w-4 rounded-full absolute transition-all duration-200"
      :class="{
        'left-[2px]': !modelValue,
        'left-[calc(100%-(1rem+2px))]': modelValue,
      }"
    />
  </div>
</template>
