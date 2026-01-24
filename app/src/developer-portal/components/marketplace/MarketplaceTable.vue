<script setup lang="ts">

import type {
    PublishedAppDetailsForMarketplace
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/PublishedAppDetails";

defineProps<{
  apps: PublishedAppDetailsForMarketplace[]
}>();

defineEmits<{
  unpublish: [string]
}>();

</script>

<template>
<div class="grid grid-cols-4 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-5">
  <MarketplaceItem 
    v-for="app in apps"
    :key="app.appId"
    :app="app"
    @unpublish="(e:string) => $emit('unpublish', e)"
  />
</div>
</template>