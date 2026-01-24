<script setup lang="ts">
useHead({ title: 'Component Gallery | MeldRx' });
const allRedirectUrls = ref<string[]>([]);
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        RedirectUrlList
      </DsText>
      <div class="pb-5" />

      <RedirectUrlList
        v-model="allRedirectUrls"
        :limit="5"
        :rules="[
          [(v?: string[]) => !!v && v.length > 0, 'At least one Redirect URL is required'],
          [(v?: string[]) => !!v && ((new Set(v)).size === v.length), 'Cannot have duplicate Redirect URLs']
        ]"
      />
      <div class="pb-5" />

      Data:
      <pre>{{ allRedirectUrls }}</pre>
    </div>
  </DsContainer>
</template>
