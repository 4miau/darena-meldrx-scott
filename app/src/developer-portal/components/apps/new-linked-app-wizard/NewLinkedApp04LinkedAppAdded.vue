<script setup lang="ts">
import { Colors } from '~/types/ui/colors';

defineEmits<{
  'addAnotherLinkedApp': [];
  'close': [];
}>();
</script>

<template>
  <DsModalProgressCard :total-steps="3" :current-step="3">
    <div class="w-full">
      <!-- Cloud -->
      <div class="w-full justify-center items-center inline-flex">
        <MeldRxPlayground />
      </div>

      <!-- Description -->
      <div class="p-4 w-full flex-col justify-center items-center gap-5 flex">
        <DsText size="2xl" weight="light" class="text-center">
          Linked App Successfully Added!
        </DsText>
        <DsText size="sm" weight="light" class="text-center">
          You've successfully provided your linked app credentials!
        </DsText>
      </div>

      <!-- Buttons -->
      <div class="p-4 flex justify-center gap-4">
        <DsButton :color="Colors.white" :text-color='Colors.gray' variant="subtle" @click="$emit('close')">
          Close
        </DsButton>
        <DsButton :color="Colors.secondary" variant="filled" @click="$emit('addAnotherLinkedApp')">
          Add Another Linked App
        </DsButton>
      </div>
    </div>
  </DsModalProgressCard>
</template>
