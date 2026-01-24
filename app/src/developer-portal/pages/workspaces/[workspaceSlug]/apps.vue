<script setup lang="ts">
import { Colors } from '~/types/ui/colors'
import type { WorkspaceAppPermissionDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/FhirRecordGrantDto';

definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    allowLinked: true,
})
useHead({ title: 'Workspace System Apps | MeldRx' });

const { $api } = useNuxtApp()
const route = useRoute()


const { permissions } = useAuth()
const openModal = ref(false);
const isLoading = ref<boolean>(false)
const workspaceSlug = ref<string>(route.params.workspaceSlug as string)
const workspaceApps = ref<WorkspaceAppPermissionDto[] | null>();

const canManageApps = computed(() => {
    return !!permissions.value?.developerPermissionsDto?.canCreateApps;
});

async function loadAppsByWorkspaceSlug(workspaceSlug: string) {
    isLoading.value = true;
    try {
        workspaceApps.value = await $api.workspaces.listAppsAccess(workspaceSlug);
    } catch (error) {
        handleApiError(error, 'Unable to load system apps.')
    } finally {
        isLoading.value = false;
    }
}

async function removeApp(permissionId: string) {
    if (!canManageApps.value) {
        notification({ title: 'Error', description: "You don't have permissions to manage apps.", displayTime: 3000, variant: 'error' });
        return
    }
    isLoading.value = true;
    try {
        await $api.workspaces.deleteAppAccess(workspaceSlug.value, permissionId);
    } catch (error) {
        handleApiError(error, 'Unable to remove app access');
    } finally {
        await loadAppsByWorkspaceSlug(workspaceSlug.value);
        isLoading.value = false;
    }
}

async function closeInviteModal() {
    openModal.value = false;
    await loadAppsByWorkspaceSlug(workspaceSlug.value);
}

loadAppsByWorkspaceSlug(workspaceSlug.value);
</script>

<template>
  <DsContainer class="space-y-5">
    <DsLoadingOverlay :loading="isLoading" />

    <AddSystemAppModal
      v-if="canManageApps"
      :show-modal="openModal"
      :workspace-id="workspaceSlug"
      @close="closeInviteModal"
    />

    <!-- Header Text -->
    <DsText size="2xl" weight="light">
      Workspace System Apps
    </DsText>
    <DsText size="sm" weight="light" class="block">
      Manage the system apps allowed to access this workspace.
    </DsText>

    <!-- No System Apps -->
    <div v-if="workspaceApps && workspaceApps.length === 0">
      <div class="flex flex-col items-center justify-center h-full py-20 border-2 border-silver space-y-5">
        <DsText size="2xl" weight="light">
          No System Apps
        </DsText>
        <DsText size="sm" weight="light">
          There are no system apps tied to this workspace. Click the button below to add a system app.
        </DsText>
        <DsButton
          id="add-workspace-app-button"
          :disabled="!canManageApps"
          :color="Colors.primary"
          variant="filled"
          @click="openModal=true"
        >
          <DsIcon name='heroicons:plus' size='sm'/>
          Add System App
        </DsButton>
      </div>
    </div>

    <div v-else class="pb-4">
      <DsButton
        id="add-workspace-app-button"
        :disabled="!canManageApps"
        :color="Colors.primary"
        variant="filled"
        @click="openModal=true"
      >
        <DsIcon name='heroicons:plus' size='sm'/>
        Add System App
      </DsButton>

      <div class="py-2"/>

      <ManageAppsTable
        v-if="workspaceApps"
        :accesible-apps="workspaceApps"
        :can-manage-apps="canManageApps"
        @remove="removeApp"
      />
    </div>
  </DsContainer>
</template>
