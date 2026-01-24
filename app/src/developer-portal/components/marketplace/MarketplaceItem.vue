<script setup lang="ts">

import {Colors} from "~/types/ui/colors";
import type {
    PublishedAppDetailsForMarketplace
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/PublishedAppDetails";

const { isAdmin } = useAuth()

defineProps<{
  app: PublishedAppDetailsForMarketplace
}>();

defineEmits<{
  unpublish: [string]
}>();

function viewApp(appId: string) {
    navigateTo(`marketplace/${appId}`)
}
</script>

<template>
  <div class="w-[360px] h-[460px] grid grid-rows-16 border-silver border rounded p-5">
    <!-- Logo -->
    <div class="row-start-1 row-end-3 flex justify-between">
      <img v-if="app.logoUrl" :src="app.logoUrl" class="w-[50px] h-[50px]" alt="The logo of the organization">
      <DefaultAppCatalogLogo v-else/>

      <div v-if="isAdmin()">
        <DsButton size="sm" :color="Colors.fire" @click="$emit('unpublish', app.appId)">
          Unpublish
        </DsButton>
      </div>
    </div>

    <!-- Extension details -->
    <div class="row-start-3 row-end-4">
      <DsText size="lg" weight="light">
        {{ app.appName }}
      </DsText>
    </div>
    <div class="row-start-4 row-end-5 flex space-x-1">
      <DsText size="sm" weight="light">
        By:
      </DsText>
      <DsLink :href="app.publisherUrl" target="_blank">
        <DsText size="sm" weight="light">
          {{ app.organizationName }}
        </DsText>
      </DsLink>
    </div>
    <div class="row-start-5 row-end-6">
      <DsText size="sm" weight="normal">
        {{ app.isPaid ? "Paid" : "Free" }}
      </DsText>
    </div>
    <div class="row-start-6 row-end-7 flex justify-between">
      <DsText v-if="app.meldRxVerified" size="sm" weight="normal" class="flex pr-5 items-center gap-2">
        <DsIcon name="i-heroicons-check-badge-solid" size="xs" class="text-primary"/>
        MeldRx Verified
      </DsText>
      <DsText v-if="app.meldRxHosted" size="sm" weight="normal" class="flex pr-5 items-center gap-2">
        <DsIcon name="i-heroicons-key" size="xs" class="text-honey"/>
        MeldRx Hosted
      </DsText>
    </div>
    <div class="row-start-7 row-end-12">
      <DsText size="xs" weight="light">
        {{ app.descriptionBrief }}
      </DsText>
    </div>
    <div class="row-start-12 row-end-13">
      <div class=" flex space-x-4">
        
      <DsBadge size="sm" :color="Colors.bliss" rounded :text-color="Colors.onyx">
        <DsText size="sm" weight="light" class="px-2">
          {{ app.ehrIntegration }}
        </DsText>
      </DsBadge>
      </div>
    </div>

    <DsDivider class="row-start-13 row-end-15"/>

    <!-- Actions -->
    <div class="row-start-15 row-end-17 space-x-4">
      <DsButton 
          size="md"
          variant="outline"
          :color="Colors.primary"
          :text-color="Colors.primary"
          @click="viewApp(app.appId)"
      >
        View App
      </DsButton>
    </div>
  </div>
</template>