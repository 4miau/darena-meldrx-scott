<script setup lang="ts">
useHead({ title: 'Component Gallery | MeldRx' });

const {
    user,
    authenticated
} = useAuth();
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        User Auth Data
      </DsText>
      <div class="pb-5" />

      <pre>{{ JSON.stringify(user, null, 2) }}</pre>
      <pre>Is Authenticated? {{ authenticated }}</pre>
    </div>
  </DsContainer>
</template>
