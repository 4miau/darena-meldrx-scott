<!--
    Step 1 of "Create Workspace"
    Asks the user to choose a "Linked Workspace" or a "Standalone Workspace"
-->

<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import { SoFAppUserType } from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType";
import type { IDsSingleSelectButtonListItem } from "~/types/ui/DsSingleSelectButtonList";

defineProps<{
  appType: SoFAppUserType;
}>();

defineEmits<{
  'update:appType': [value?: SoFAppUserType]
  'next': [];
  'cancel': [];
}>();

const options: IDsSingleSelectButtonListItem<SoFAppUserType>[] = [
    { value: SoFAppUserType.Patient, title: 'Patient', subTitle: 'Patient', iconTop: 'heroicons:user' },
    { value: SoFAppUserType.Provider, title: 'Provider', subTitle: 'Provider/Clinician', iconTop: 'heroicons:building-office-2' },
    { value: SoFAppUserType.System, title: 'System', subTitle: 'System', iconTop: 'heroicons:shield-check' },
    { value: SoFAppUserType.CdsHooks, title: 'CdsHooks', subTitle: 'EHR Apps/Cds Hooks', iconTop: 'heroicons:link' },
];

</script>

<template>
  <div class="grid grid-cols-12 gap-14">
    <!-- App Type Description -->
      <div class="col-span-4">
        <DsText size="sm">
          User Information
        </DsText>
      <div class="pb-2" />

      <DsText size="xs" weight="light">
        Specify the primary user base of your application to ensure appropriate interface and functionality.
        This cannot be modified after the app has been created.
      </DsText>
    </div>

    <!-- App Type Buttons -->
    <div class="col-span-8">

      <DsSingleSelectButtonList
          :model-value="appType"
          :options="options"
          label="App Type"
          required
          @update:model-value="(e) => $emit('update:appType', e)"
      />
    </div>
  </div>

  <!-- Cancel/Next Buttons -->
  <DsDivider />
  <div class="justify-start items-start gap-5 inline-flex">
    <DsButton id='cancel-button' :color="Colors.white" :text-color='Colors.gray' @click="$emit('cancel');">
      Cancel
    </DsButton>
    <DsButton id='next-step-button' :color="Colors.primary" @click="$emit('next');">
      Next Step
    </DsButton>
  </div>
</template>
