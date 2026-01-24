<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import type {WorkspaceExternalApp} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceExternalApp";

defineProps<{
    externalApps: WorkspaceExternalApp[];
}>();

defineEmits<{
    'addExternalApp': [];
    'editExternalApp': [externalAppIndex: number];
    'delete': [externalAppIndex: number];
}>();
</script>

<template>
  <DsLabeledInput label="External Apps" :required="false">
    <ExternalAppListItem
      v-for="(externalApp, index) in externalApps"
      :id="externalApp.clientName.replaceAll(' ', '-').toLowerCase()"
      :key="index"
      :external-app="externalApp"
      @delete="() => $emit('delete', index)"
      @edit="$emit('editExternalApp', index)"
    />
    <DsButton
      id="add-new-external-app-button"
      variant="transparent"
      :color="Colors.fire"
      :text-color='Colors.fire'
      @click="$emit('addExternalApp');"
    >
      + Add External App
    </DsButton>
  </DsLabeledInput>
</template>
