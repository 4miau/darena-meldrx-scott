<script setup lang="ts">
useHead({ title: 'Component Gallery | MeldRx' });
</script>

<template>
  <CreateMeldRxInstructions
    :client-id="'123'"
    :fhir-database-display-name="'abc'"
    :fhir-server-endpoint="'https://fhir.server'"
  />
</template>
