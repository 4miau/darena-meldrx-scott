<script setup lang="ts">
import { Colors } from '~/types/ui/colors';

defineProps<{
  fhirApiProviderMeldRxIdentifier?: string;
  isSharedCredentialFound: boolean;
}>();

const emit = defineEmits<{
  'selectOwnCredentials': [];
  'selectSampleCredentials': [];
  'goBack': [];
  'close': [];
}>();

</script>

<template>
  <DsModalProgressCard :total-steps="3" :current-step="2">
    <!-- Lock -->
    <div class="flex w-full py-8 bg-space justify-center items-center">
      <div class="flex w-full items-center justify-center">
        <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center pt-2">
          <MeldRxLock />
        </div>
      </div>
      <div class="flex-1" />
    </div>

    <!-- Credential Option Select -->
    <div class="flex flex-col items-center m-2.5">
      <DsText size="2xl" weight="light">
        Linked App Credentials
      </DsText>
      <DsText size="sm" weight="light">
        Which credentials would you like to use?
      </DsText>
      <div class="flex justify-between m-3.5">
        <DsButton
          id='use-my-own-credentials-button'
          class="flex w-[220px] text-xs justify-center items-center border border-silver border-solid"
          variant="outline"
          :color="Colors.white"
          :text-color='Colors.gray'
          @click="emit('selectOwnCredentials')"
        >
          Use My Own Credentials
        </DsButton>
        <div class="px-1" />
        <DsButton
          id='use-sample-meldrx-credentials-button'
          :disabled="!isSharedCredentialFound"
          class="flex w-[220px] text-xs justify-center items-center border border-silver border-solid"
          variant="outline"
          :color="Colors.white"
          :text-color='Colors.gray'
          :sub-content="isSharedCredentialFound ? '(Sandbox Access Only)' : '(Currently Unavailable / Coming Soon)'"
          @click="emit('selectSampleCredentials')"
        >
          Use Sample Credentials
        </DsButton>
      </div>
      <DsDivider class="my-1" />
    </div>

    <!-- Buttons -->
    <div class="flex justify-center w-full pb-4">
      <DsButton :text-color='Colors.secondary' right :color="Colors.secondary" variant="outline" @click="emit('goBack')">
        Previous Step
      </DsButton>
    </div>
  </DsModalProgressCard>
</template>