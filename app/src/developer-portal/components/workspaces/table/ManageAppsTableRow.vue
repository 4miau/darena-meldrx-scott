<!--
    Table Rows for displaying a list of Workspace-Apps
-->

<script setup lang="ts">
import type { AppRole } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/AppRole';
import { Colors } from '~/types/ui/colors';

const props = defineProps<{
    id: string;
    dateAdded: string;
    clientId: string;
    role: AppRole;
    appName: string;
    canManageApps:boolean;
}>();

defineEmits<{
  'remove': []
}>();

const formattedDate = new Intl.DateTimeFormat('en-US', { year: 'numeric', month: 'long', day: 'numeric' }).format(new Date(props.dateAdded));
const currentRole = ref<string>(props.role);

</script>

<template>
  <tr class="bg-white border-bliss border-b hover:bg-seafoam">
    <!-- Name -->
    <td class="px-6 py-4">
      <DsText size="xs">
        {{ appName }}
      </DsText>
    </td>

    <!-- App Id / Client Id -->
    <td class="px-6 py-4">
      <DsText size="xs">
        {{ clientId }}
      </DsText>
    </td>

    <!-- Role -->
    <td class="px-6 py-4">
      <DsText size="xs">
        {{ currentRole }}
      </DsText>
    </td>

    <!-- Added On -->
    <td class="px-6 py-4">
      <DsText size="xs">
        {{ formattedDate }}
      </DsText>
    </td>

    <!-- Actions -->
    <td class="px-6 py-4">
      <DsButton
        :disabled="!canManageApps"
        :color="Colors.fire"
        :text-color='Colors.fire'
        variant="outline"
        @click="$emit('remove')"
      >
        <DsIcon name="heroicons:x-mark"/>
        Remove
      </DsButton>
    </td>
  </tr>
</template>
