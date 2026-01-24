<script setup lang="ts">
import type { INewLinkedApp } from '~/types/ui/apps/NewLinkedApp';
import { Colors } from '~/types/ui/colors';
import EHRUtils from '~/utils/EHRUtils';

defineProps<{
    linkedApps: INewLinkedApp[];
}>();

defineEmits<{
    'addLinkedApp': [];
    'editLinkedApp': [linkedAppIndex: number];
    'delete': [linkedAppIndex: number];
}>();
</script>

<template>
  <DsLabeledInput label="EHR Systems" :required="false">
    <LinkedAppListItem
      v-for="(linkedApp, index) in linkedApps"
      :id="linkedApp.clientName.replaceAll(' ', '-').toLowerCase()"
      :key="index"
      :linked-app="linkedApp"
      :ehr="EHRUtils.getEhrFromFhirApiProviderMeldRxId(linkedApp.fhirApiProviderMeldRxIdentifier)"
      @delete="() => $emit('delete', index)"
      @edit="$emit('editLinkedApp', index)"
    />
    <DsButton
      id="add-new-linked-app-button"
      variant="transparent"
      :color="Colors.fire"
      :text-color='Colors.fire'
      @click="$emit('addLinkedApp');"
    >
      + Add Linked App
    </DsButton>
  </DsLabeledInput>
</template>
