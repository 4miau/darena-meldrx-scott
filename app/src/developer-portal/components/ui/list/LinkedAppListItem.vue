<script setup lang="ts">
import type { EHRs } from '~/types/ehrs';
import type { INewLinkedApp } from '~/types/ui/apps/NewLinkedApp';
import { Colors } from '~/types/ui/colors';

defineProps<{
  linkedApp: INewLinkedApp;
  ehr?: EHRs;
}>();

defineEmits<{
  'edit': [];
  'delete': [];
}>();
</script>

<template>
  <div class="flex flex-row items-center border-b border-bliss">
    <div class="flex flex-row items-center cursor-pointer w-full" @click="$emit('edit')">
      <div class="w-12 mr-2 flex items-center justify-center">
        <EHRLogo :ehr-type="ehr" size="sm" class="h-full" />
      </div>
      <DsText size="sm" weight="light" class="grow underline">
        {{ linkedApp.clientName }}
      </DsText>
    </div>
    <DsButton variant="subtle" :color="Colors.white" :text-color='Colors.gray' title="x" @click="$emit('delete');">
      x
    </DsButton>
  </div>
</template>
