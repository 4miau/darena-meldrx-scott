<script setup lang="ts">
useHead({ title: 'Component Gallery | MeldRx' });
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        User Menu
      </DsText>
      <div class="pb-5" />

      <UserMenu
        email="patrick@darenasolutions.com"
        version="v1.0.3"
        environment="stg"
      />
      <DsDivider />
    </div>
  </DsContainer>
</template>
