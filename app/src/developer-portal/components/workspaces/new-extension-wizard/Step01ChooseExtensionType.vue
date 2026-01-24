<script setup lang="ts">
import type { AppSourceType } from '~/types/ui/apps/AppSourceType';
import { Colors } from '~/types/ui/colors';
import type { IDsSingleSelectButtonListItem } from '~/types/ui/DsSingleSelectButtonList';

const { permissions } = useAuth()

defineProps<{
  appSourceType?: AppSourceType;
}>();

defineEmits<{
  'update:appSourceType': [value?: AppSourceType];
  'next': [];
  'cancel': [];
}>();

const options: IDsSingleSelectButtonListItem<AppSourceType>[] =  permissions.value.developerPermissionsDto?.canCreateApps ?
    [
        { value: 'Published', title: 'Public', subTitle: 'Extensions offered on the marketplace', iconTop: 'heroicons:globe-alt' },
        { value: 'Internal', title: 'My Organization', subTitle: 'Extensions made available by my organization', iconTop: 'heroicons:building-library' },
        { value: 'Workspace', title: 'This Workspace', subTitle: 'Extensions made by this workspace', iconTop: 'heroicons:building-office-2' }
    ]: 
    [
        { value: 'Published', title: 'Public', subTitle: 'Extensions offered on the marketplace', iconTop: 'heroicons:globe-alt'},
        { value: 'Ehr', title: 'EHR', subTitle: 'Extensions made available by my EHR', iconTop: 'heroicons:building-library' },
        { value: 'Workspace', title: 'My Organization', subTitle: 'Extensions made available by my organization', iconTop: 'heroicons:building-office-2' }
    ];

</script>

<template>
  <div class="grid grid-cols-12 gap-4 p-4">
    <!-- Workspace Type Description -->
    <div class="col-span-2">
      <DsText size="sm" class="block pb-2">
        Extension Type
      </DsText>

      <DsText size="xs" weight="light" class="block">
        Choose between public extensions or those made available by your organization.
      </DsText>
    </div>

    <div class="col-span-4">
      <!-- Extension Type Selector -->
      <DsSingleSelectButtonList
        :model-value="appSourceType"
        :options="options"
        label="Extension Type"
        required
        @update:model-value="(e) => $emit('update:appSourceType', e)" />
    </div>
  </div>

  <!-- Cancel/Next Buttons -->
  <DsDivider />
  <div class="justify-start items-start gap-5 inline-flex">
    <DsButton :color="Colors.white" :text-color='Colors.gray' size="md" @click="$emit('cancel');">
      Cancel
    </DsButton>
    <DsButton :color="Colors.primary" size="md" @click="$emit('next');">
      Next Step
    </DsButton>
  </div>
</template>
