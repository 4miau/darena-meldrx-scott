<script setup lang="ts">

useHead({ title: 'Component Gallery | MeldRx' });
const isLoading = ref<boolean>(false);
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        Loaders
      </DsText>
      <div class="pb-5" />

      <DsText size="xl">
        DsLoadingSpinner
      </DsText>

      <DsLoadingSpinner :loading="isLoading" />

      <div class="pb-4" />
      <DsButton @click="() => isLoading = !isLoading">
        Toggle Loading State
      </DsButton>
      <pre>Is Loading?: {{ isLoading }}</pre>
      <DsDivider />
    </div>
  </DsContainer>
</template>
