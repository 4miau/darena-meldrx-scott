<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import type {WorkspaceExternalApp} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceExternalApp";

defineProps<{
  externalApp: WorkspaceExternalApp;
}>();

defineEmits<{
  'edit': [];
  'delete': [];
}>();
</script>

<template>
  <div class="flex flex-row items-center border-b border-bliss">
    <div class="flex flex-row items-center cursor-pointer w-full" @click="$emit('edit')">
      <DsText size="sm" weight="light" class="grow underline">
        {{ externalApp.clientName }}
      </DsText>
    </div>
    <DsButton variant="subtle" :color="Colors.white" :text-color='Colors.gray' @click="$emit('delete');">
      x
    </DsButton>
  </div>
</template>
