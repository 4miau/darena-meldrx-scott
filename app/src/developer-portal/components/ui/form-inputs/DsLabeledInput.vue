<!--
    Generic component that holds an input and puts a label above it.
-->

<script setup lang="ts">
defineProps<{
    label?: string;
    required?: boolean;
}>();
</script>

<template>
  <div>
    <div class="flex ">
      <div v-if="label" class="flex self-start">
        <DsText size="sm" class="flex row space-x-1">
          {{ label }}
          <span v-if="required" class="text-fire">*</span>
        </DsText>
      </div>
      <DsPopover v-if="$slots.popovercontent">
        <DsText size="sm" class="flex row space-x-1">
          <slot name="popoverarea" />
        </DsText>
        <template #content>
          <slot name="popovercontent" />
        </template>
      </DsPopover>
    </div>
    <slot />
  </div>
</template>
