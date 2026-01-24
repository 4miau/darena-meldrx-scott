<script setup lang="ts">
// allow fall-through v-model binding from EhrAppForm

const currentLinkedApps = ref([]);
</script>

<template>
  <LinkedAppEditForm
    :show-none="false"
    :show-client-secret="false"
    :current-linked-apps="currentLinkedApps"
  />
</template>
