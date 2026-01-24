<script setup lang="ts">
// allow fall-through v-model binding from EhrAppForm

const currentLinkedApps = ref([]);
</script>

<template>
  <LinkedAppEditForm
    :show-none="true"
    :show-client-secret="true"
    :show-jwks-credential="false"
    :show-private-key="false"
    :show-hosted-jwks="false"
    :current-linked-apps="currentLinkedApps"
  />
</template>
