<script setup lang="ts">

import {Colors} from "~/types/ui/colors";
import type {
    PublishedAppDetails
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/PublishedAppDetails";
import CHAILogo from "~/components/svg/CHAILogo.vue";
import {DsiOption, DsiOptionMap} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/DsiOption";

useHead({ title: 'App Details' });
definePageMeta({ anonymous: true, layout: 'default' });

const { $api } = useNuxtApp()
const route = useRoute()
const appId = ref<string>(route.params.appId as string)

const state = reactive<{
  isLoading: boolean,
  app?: PublishedAppDetails,
  showDsiDrawer: boolean,
  showModelCard: boolean,
  showAddToWorkspace: boolean,
}>({
    isLoading: false,
    showDsiDrawer: false,
    showModelCard: false,
    showAddToWorkspace: false,
})

async function loadApp(){
    state.isLoading = true;
    try {
        state.app = await $api.marketplace.getPublishedAppDetails(appId.value)
    }catch(error){
        handleApiError(error, 'Unable to load app details');
    } finally {
        state.isLoading = false;
    }
}

loadApp()
</script>

<template>
  <DsContainer v-if="state.app">
    <!-- Dsi Source Attributes Viewer/Editor Drawer -->
    <DsDrawer :show="state.showDsiDrawer" @close="state.showDsiDrawer = false">
      <DsSourceAttributeModifier
          v-if="state.app.appName"
          title="View Extension Details"
          :sub-title="state.app.appName"
          :source-attributes="state.app.sourceAttributeGroups ?? []"
          description="View source attributes about this application"
          :form-entry-disabled="true"
          @on-cancel="state.showDsiDrawer = false"
      />
    </DsDrawer>

    <DsViewer v-if="state.showModelCard" @close="state.showModelCard = false">
      <ChaiModelCard
          v-if="state.app.chaiModelCardGroups"
          class="h-full overflow-auto m-5 space-y-5"
          :model-card-form="state.app.chaiModelCardGroups"
      />
    </DsViewer>
    
    <AddToWorkspaceModal
        v-if="state.showAddToWorkspace"
        :show="state.showAddToWorkspace"
        :app="state.app"
        @close="state.showAddToWorkspace = false"
    />
    
    <div class="grid grid-cols-2">
      
      <div class="col-start-1 col-end-2 flex gap-3 items-center">
        <div>
          <img v-if="state.app.logoUrl" :src="state.app.logoUrl" class="w-[50px] h-[50px]" alt="The logo of the organization">
          <DefaultAppCatalogLogo v-else/>
        </div>
        <div class="flex flex-col">
          <div>
            <DsText size="xl" weight="light">
              {{ state.app.appName }}
            </DsText>
          </div>
          <div class="flex space-x-1">
            <DsText size="md" weight="light">
              By:
            </DsText>
            <DsLink :href="state.app.publisherUrl" target="_blank">
              <DsText size="md" weight="light">
                {{ state.app.organizationName }}
              </DsText>
            </DsLink>
          </div>
        </div>
      </div>
      <div class="col-start-2 col-end-3 flex flex-col space-y-2">
        <div>
          <DsText size="sm" weight="normal">
           <strong>Price:</strong> {{ state.app.isPaid ? "Paid" : "Free" }}
          </DsText>
        </div>
        <div class="space-x-4">
          <DsButton
              size="md"
              variant="filled"
              :color="Colors.primary"
              @click="state.showAddToWorkspace = true"
          >
            + Add To Workspace
          </DsButton>
        </div>
      </div>
      <div class="col-start-1 col-end-3 mt-3">
        <DsButton
            size="xs"
            variant="outline"
            :color="Colors.onyx"
            :text-color="Colors.onyx"
            @click="navigateTo(`/marketplace`)"
        >
          <DsIcon name="heroicons-arrow-left" size="xs"/>
          Back To Marketplace
        </DsButton>
      </div>
      
      <DsDivider class="col-start-1 col-end-3"/>
      
      <div class="col-start-1 col-end-3 flex justify-start gap-5">
        <div v-if="state.app.meldRxVerified" class="w-full">
          <DsText size="sm" weight="normal">
            MeldRx Verified
          </DsText>
          <DsText size="md" weight="light" class="flex pr-5 items-center gap-2">
            <DsIcon name="i-heroicons-check-badge-solid" size="xs" class="text-primary"/>
            This app has been verified by Darena Solutions
          </DsText>
          <DsPopover>
            <DsText size="xs" weight="light" underline :color="Colors.lapisLazuli">
              What's this?
            </DsText>
            <template #content>
              <DsText size="xs" weight="light">
                The Darena Solutions team has verified this application.
              </DsText>
            </template>
          </DsPopover>
        </div>
        <div v-if="state.app.meldRxHosted" class="w-full">
          <DsText size="sm" weight="normal">
            MeldRx Hosted
          </DsText>
          <DsText size="md" weight="light" class="flex pr-5 items-center gap-2">
            <DsIcon name="i-heroicons-key" size="xs" class="text-honey"/>
            This app is hosted on MeldRx
          </DsText>
          <DsPopover>
            <DsText size="xs" weight="light" underline :color="Colors.lapisLazuli">
              What's this?
            </DsText>
            <template #content>
              <DsText size="xs" weight="light">
                This app is hosted within the MeldRx ecosystem, ensuring that all data stays within your workspace.
              </DsText>
            </template>
          </DsPopover>
        </div>
      </div>
      
      <DsDivider v-if="state.app.meldRxVerified || state.app.meldRxHosted" class="col-start-1 col-end-3"/>
      

      <div class="col-start-1 col-end-3 flex flex-col gap-5">
        <DsText size="md" weight="normal">
          Description
        </DsText>
        <DsText size="sm" weight="light">
          {{ state.app.description }}
        </DsText>
      </div>
      
      <DsDivider v-if="state.app.dsiType !== DsiOption.None" class="col-start-1 col-end-3"/>


      <DsText v-if="state.app.dsiType !== DsiOption.None" size="md" weight="normal" class="col-start-1 col-end-3 mb-3">
        Clinical Decision Support
      </DsText>

      <div v-if="state.app.dsiType !== DsiOption.None" class="col-start-1 col-end-2 flex flex-col space-y-2">
        <DsText size="sm" weight="normal">
          Classification
        </DsText>
        <div>
          <DsBadge size="sm" :color="Colors.bliss" rounded :text-color="Colors.onyx">
            <DsText size="sm" weight="light" class="px-2">
              {{ DsiOptionMap(state.app.dsiType) }}
            </DsText>
          </DsBadge>
        </div>
      </div>

      <div v-if="state.app.dsiType !== DsiOption.None" class="col-start-2 col-end-3 flex flex-col space-y-2">
        <DsText size="sm" weight="normal">
          Safety
        </DsText>
        <div class="flex gap-3">
          <DsButton
              v-if="state.app.chaiModelCardGroups && state.app.chaiModelCardGroups.length > 0"
              size="md"
              variant="outline"
              :color="Colors.onyx"
              :text-color="Colors.onyx"
              @click.stop="state.showModelCard = true"
          >
            <CHAILogo/>
            View CHAI Model Card
          </DsButton>
          <DsButton
              size="md"
              variant="outline"
              :color="Colors.onyx"
              :text-color="Colors.onyx"
              @click="state.showDsiDrawer = true"
          >
            View Source Attributes
          </DsButton>
        </div>
      </div>

      <DsDivider class="col-start-1 col-end-3"/>


      <DsText size="md" weight="normal" class="col-start-1 col-end-3 mb-3">
        Details
      </DsText>

      <div class="col-start-1 col-end-2 flex flex-col space-y-4">
        <div class="flex flex-col space-y-2">
          <DsText size="sm" weight="normal">
            Category
          </DsText>
          <div>
            <DsBadge size="sm" :color="Colors.bliss" rounded :text-color="Colors.onyx">
              <DsText size="sm" weight="light" class="px-2">
                {{ state.app.ehrIntegration }}
              </DsText>
            </DsBadge>
          </div>
        </div>
        <div class="flex flex-col space-y-2">
          <DsText size="sm" weight="normal">
            Publisher Name
          </DsText>
          <DsText size="sm" weight="light">
            {{ state.app.organizationName }}
          </DsText>
        </div>
        <div class="flex flex-col space-y-2">
          <DsText size="sm" weight="normal">
            Privacy Policy
          </DsText>
          <DsLink :href="state.app.privacyPolicyUrl" class="text-lapis-lazuli">
            <DsText size="sm" weight="light" color="text-lapis-lazuli">
              {{ state.app.privacyPolicyUrl }}
            </DsText>
          </DsLink>
        </div>
        <div class="flex flex-col space-y-2">
          <DsText size="sm" weight="normal">
            Marketplace Listing Last Updated
          </DsText>
          <DsText size="sm" weight="light">
            {{ state.app.lastUpdated }}
          </DsText>
        </div>
      </div>

      <div class="col-start-2 col-end-3 flex flex-col space-y-4">
        <div v-if="state.app.intendedUsers && state.app.intendedUsers.length > 0" class="flex flex-col space-y-2">
          <DsText size="sm" weight="normal">
            Intended Users
          </DsText>
          <div class="flex gap-3">
            <DsBadge v-for="item in state.app.intendedUsers" :key="item" size="sm" :color="Colors.bliss" rounded :text-color="Colors.onyx">
              <DsText size="sm" weight="light" class="px-2">
                {{ item }}
              </DsText>
            </DsBadge>
          </div>
        </div>
        <div class="flex flex-col space-y-2">
          <DsText size="sm" weight="normal">
            Website
          </DsText>
          <DsLink :href="state.app.publisherUrl" class="text-lapis-lazuli">
            <DsText size="sm" weight="light" color="text-lapis-lazuli">
              {{ state.app.publisherUrl }}
            </DsText>
          </DsLink>
        </div>
        <div class="flex flex-col space-y-2">
          <DsText size="sm" weight="normal">
            Terms of Service
          </DsText>
          <DsLink :href="state.app.termsOfServiceUrl" class="text-lapis-lazuli">
            <DsText size="sm" weight="light" color="text-lapis-lazuli">
              {{ state.app.termsOfServiceUrl }}
            </DsText>
          </DsLink>
        </div>
      </div>
  
      <DsDivider class="col-start-1 col-end-3"/>

    </div>
  </DsContainer>
</template>