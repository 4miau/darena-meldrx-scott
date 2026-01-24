<script setup lang="ts">

defineProps<{
    modelValue: boolean,
    label: string
}>()

defineEmits<{
    'update:modelValue': [value: boolean]
}>()

</script>

<template>
  <div class="flex align-center cursor-pointer" @click="$emit('update:modelValue', !modelValue)">
    <input
      class="cursor-pointer"
      type="checkbox"
      :value="modelValue"
      :checked="modelValue"
    >
    <DsText class="ml-2" size="sm">
      {{ label }}
    </DsText>
  </div>
</template>
