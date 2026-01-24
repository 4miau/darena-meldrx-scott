<!--
    Step 1 of "Create Workspace"
    Asks the user to choose a "Linked Workspace" or a "Standalone Workspace"
-->

<script setup lang="ts">
import type { WorkspaceType } from '~/types/meldrx-api/workspaces';
import { Colors } from '~/types/ui/colors';

defineProps<{
  workspaceType?: WorkspaceType;
}>();

defineEmits<{
  'update:workspaceType': [value?: WorkspaceType];
  'next': [];
  'cancel': [];
}>();
</script>

<template>
  <div class="grid grid-cols-12 gap-14">
    <!-- Workspace Type Description -->
    <div class="col-span-4">
      <DsText size="sm">
        Workspace Type
      </DsText>
      <div class="pb-2" />

      <DsText size="xs" weight="light">
        Choose between a linked workspace or standalone workspace.
        This cannot be modified after the workspace has been created.
      </DsText>
    </div>

    <!-- Workspace Type Buttons -->
    <div class="col-span-8">
      <WorkspaceTypeSelect
        :model-value="workspaceType"
        @update:model-value="$emit('update:workspaceType', $event)"
      />
    </div>
  </div>

  <!-- Cancel/Next Buttons -->
  <DsDivider />
  <div class="justify-start items-start gap-5 inline-flex">
    <DsButton :color="Colors.white" :text-color='Colors.gray' @click="$emit('cancel');">
      Cancel
    </DsButton>
    <DsButton :color="Colors.primary" @click="$emit('next');">
      Next
    </DsButton>
  </div>
</template>
