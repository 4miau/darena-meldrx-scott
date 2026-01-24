<script setup lang="ts">
import type { WorkspaceType } from '~/types/meldrx-api/workspaces';
import type { IDsSingleSelectButtonListItem } from '~/types/ui/DsSingleSelectButtonList';

defineProps<{
    modelValue?: WorkspaceType;
}>();

defineEmits<{
  'update:modelValue': [value?: WorkspaceType]
}>();

const options: IDsSingleSelectButtonListItem<WorkspaceType>[] = [
    { value: 'standalone', title: 'Standalone Workspace', subTitle: 'Blank FHIR-Compliant Workspace', iconTop: 'heroicons:fire' },
    { value: 'linked', title: 'Linked Workspace', subTitle: 'Connect to an existing EHR', iconTop: 'heroicons:link' }
];
</script>

<template>
  <DsSingleSelectButtonList
    :model-value="modelValue"
    :options="options"
    label="Workspace Type"
    required
    @update:model-value="(e) => $emit('update:modelValue', e)"
  />
</template>
