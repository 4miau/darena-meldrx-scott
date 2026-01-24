<!--
    Overlay to show when something is loading.
    Disables the entire screen.
    It's good if you don't want the user to do anything while something is loading.

    Usage:
        <DsLoadingOverlay :loading="true" />
 -->

<script setup lang="ts">
defineProps<{
    loading?: boolean;
}>();
</script>

<template>
  <div v-if="loading" class="w-full h-full fixed block top-0 left-0 bg-white opacity-75 z-50">
    <div class="opacity-75 top-1/2 my-0 mx-auto block relative w-0 h-0" style="top: 50%;">
      <div class="flex flex-col items-center gap-2">
        <DsLoadingSpinner :loading="true" />
        Loading...
      </div>
    </div>
  </div>
</template>
