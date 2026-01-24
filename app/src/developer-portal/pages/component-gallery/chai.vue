<script setup lang="ts">

import type {ChaiModelCardGroup} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/ChaiModelCardForm";
const dummyData:ChaiModelCardGroup[] = [];
</script>

<template>
  <DsContainer>
    <ChaiModelCard 
       :model-card-form="dummyData"
    />
  </DsContainer>
</template>

<style scoped>

</style>