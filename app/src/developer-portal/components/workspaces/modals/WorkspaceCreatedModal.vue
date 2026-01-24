<script setup lang="ts">
import { Colors } from '~/types/ui/colors';


defineProps<{
    show: boolean;
    fhirServerEndpoint: string;
}>();

defineEmits<{
    'close': [];
    'goToWorkspace': [];
}>();

</script>

<template>
  <DsModal :model-value="show" @close="$emit('close')">
    <div class="flex flex-col items-center p-5">
      <DsText size="lg">
        Workspace Created
      </DsText>
      <div class="pb-5" />

      <DsText size="xs" weight="light">
        Your workspace has been successfully created!
      </DsText>
      <div class="pb-5" />

      <DsText size="xs" weight="light">
        Workspace URL:
      </DsText>
      <DsTextInputWithCopyButton
        label="Workspace URL"
        disabled
        :model-value="fhirServerEndpoint"
        class="w-full"
        @copy="notification({
          title: 'Copied!',
          description: 'Your Workspace URL has been copied to your clipboard.',
          displayTime: 3000,
          variant: 'success',
          })"
        />
      <div class="pb-5" />

      <DsButton :color="Colors.primary" @click="$emit('goToWorkspace')">
        Go to Workspace
      </DsButton>
    </div>
  </DsModal>
</template>
