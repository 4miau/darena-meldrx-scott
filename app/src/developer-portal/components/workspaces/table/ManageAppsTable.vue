<!--
    Table displaying a list of Workspace-Apps
-->

<script setup lang="ts">
import type { WorkspaceAppPermissionDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/FhirRecordGrantDto';

defineProps<{
    accesibleApps: WorkspaceAppPermissionDto[];
    canManageApps: boolean;
}>();

defineEmits<{
  'remove': [permissionId: string]
}>();

</script>

<template>
  <div class="relative border border-silver p-2 rounded">
    <table class="w-full text-sm text-left">
      <thead class="bg-bliss">
        <tr>
          <th scope="col" class="px-6 py-3">
            <DsText size="md" weight="normal">
              App Name
            </DsText>
          </th>
          <th scope="col" class="px-6 py-3">
            <DsText size="md" weight="normal">
              App ID / Client ID
            </DsText>
          </th>
          <th scope="col" class="px-6 py-3">
            <DsText size="md" weight="normal">
              Role
            </DsText>
          </th>
          <th scope="col" class="px-6 py-3">
            <DsText size="md" weight="normal">
              Date Added
            </DsText>
          </th>
          <th scope="col" class="px-6 py-3">
            <DsText size="md" weight="normal">
              Actions
            </DsText>
          </th>
        </tr>
      </thead>
      <tbody>
        <ManageAppsTableRow
          v-for="app in accesibleApps"
          :id="app.appId"
          :key="app.id"
          :date-added="app.createdAt?.toLocaleString() ?? ''"
          :role="app.role"
          :app-name="app.name"
          :client-id="app.appId"
          :can-manage-apps="canManageApps"
          @remove="$emit('remove', app.id)"
        />
      </tbody>
    </table>
  </div>
</template>
