<!--
    Text input.
    You can put a label above it.
    You can mark an input as required.

    Usage:
        <DsLabeledText
            label="Text Input Label"
            text="Your text here"
        />
 -->

<script setup lang="ts">

defineProps<{
      label?: string;
      text?: string;
}>();

</script>

<template>
  <div>
    <DsText size="sm">
      {{ label }}
    </DsText>
    <div class="pb-2" />
    <DsText :id="label?.replaceAll(' ','-').toLowerCase() + '-label'" size="xs" weight="light">
      <template v-if="text">
        {{ text }}
      </template>
      <slot v-else />
    </DsText>
  </div>
</template>
