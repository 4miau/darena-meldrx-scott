<!--
    This is the UI to show when a user has created an application.
    It would normally be a page, but we don't want to pass client secrets through the URL.
-->

<script setup lang="ts">
import { Colors } from "~/types/ui/colors";


defineProps<{
    clientId: string;
    clientSecret?: string;
}>();

defineEmits<{
    'backToApps': [];
}>();
</script>

<template>
  <DsContainer>
    <!-- Header -->
    <div class="justify-start items-center gap-5 inline-flex">
      <div class="relative">
        <PartyPopper />
      </div>
      <div class="flex-col justify-start items-start inline-flex">
        <DsText size="2xl" weight="light">
          Application Successfully Registered
        </DsText>
        <div class="pb-2" />

        <DsText size="sm" weight="light" class="block">
          Congratulations on registering your app- you're on your way to doing something great!
        </DsText>

        <DsText size="sm" weight="light" class="block">
          Here are the important details on what to do next.
        </DsText>
      </div>
    </div>

    <DsDivider />

    <!-- Application Details -->
    <div>
      <DsText size="md">
        Application Details
      </DsText>
      <div class="pb-5" />

      <DsTextInputWithCopyButton
        id="app-id"
        disabled
        label="App ID / Client ID"
        :model-value="clientId"
        @copy="notification({
          title: 'Copied!',
          description: 'Your App ID / Client ID has been copied to your clipboard.',
          displayTime: 3000,
          variant: 'success'
        })"
      />
      <div class="pb-5" />

      <template v-if="clientSecret">
        <DsTextInputWithCopyButton
          disabled
          label="Client Secret"
          :model-value="clientSecret"
          @copy="notification({
            title: 'Copied!',
            description: 'Your Client secret has been copied to your clipboard.',
            displayTime: 3000,
            variant: 'success'
          })"
        />
        <div class="pb-2" />

        <DsText size="xs" :color="Colors.fire" class="block">
          Copy your secret above and store it securely.
        </DsText>
        <DsText size="xs" :color="Colors.fire" class="block">
          You will not be able to access this secret again after you leave this page.
        </DsText>
      </template>
    </div>

    <DsDivider />

    <!-- Installation Instructions -->
    <div>
      <DsText size="md">
        Get Started
      </DsText>
      <div class="pb-5" />

      <CreateMeldRxInstructions
        :client-id="clientId"
      />
    </div>

    <DsDivider />

    <!-- Back to Apps -->
    <div>
      <DsButton id='back-to-apps-button' :color='Colors.primary' @click="$emit('backToApps')">
        Back to Apps
      </DsButton>
    </div>
  </DsContainer>
</template>
