<script setup lang="ts">
useHead({ title: 'Component Gallery | MeldRx' });
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        EHR Logos
      </DsText>
      <div class="pb-5" />

      <EpicLogo style="width:150px" />
      <DsDivider />

      <CernerLogo style="width:150px" />
      <DsDivider />

      <AthenaHealthLogo style="width:150px" />
      <DsDivider />

      <NextGenLogo style="width:150px" />
      <DsDivider />

      <VeradigmLogo style="width:150px" />
      <DsDivider />
    </div>
  </DsContainer>
</template>
