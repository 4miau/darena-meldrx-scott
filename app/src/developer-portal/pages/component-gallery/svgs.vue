<script setup lang="ts">
useHead({ title: 'Component Gallery | MeldRx' });
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        SVGs
      </DsText>
      <div class="pb-5" />

      <DsText size="xl">
        MeldRxButterfly
      </DsText>
      <MeldRxButterfly />
      <DsDivider />

      <DsText size="xl">
        ThreeFlowers
      </DsText>
      <ThreeFlowers />
      <DsDivider />

      <DsText size="xl">
        MeldRxCloud
      </DsText>
      <MeldRxCloud background="black" />
      <DsDivider />

      <DsText size="xl">
        MeldRxLock
      </DsText>
      <MeldRxLock />
      <DsDivider />

      <DsText size="xl">
        MeldRxPlayground
      </DsText>
      <MeldRxPlayground />
      <DsDivider />
    </div>
  </DsContainer>
</template>
