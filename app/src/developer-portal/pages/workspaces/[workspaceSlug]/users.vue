<script setup lang="ts">
import { Colors } from '~/types/ui/colors'
import type { WorkspaceUserPermissionDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/UserPermissionDto';
import type { OrganizationUserRelationDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/OrganizationUserRelationDto';
import type { WorkspaceUserModificationModel } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Organization/OrganizationUserModificationModel';
import type { CreateModifyOrganizationUserRelationDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Organization/CreateModifyOrganizationUserRelationDto';
import { type OrganizationRoles, roleConfig } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/OrganizationRoles';

definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    allowLinked: true,
})
useHead({ title: 'Workspace Users | MeldRx' });

const { $api } = useNuxtApp()
const route = useRoute()
const confirmation = useConfirmation();

const { permissions, isAdmin } = useAuth()
const openModal = ref(false);
const isLoading = ref<boolean>(false)
const workspaceSlug = ref<string>(route.params.workspaceSlug as string)
const workspacePermissions = ref<WorkspaceUserPermissionDto | null>();
const workspaceUsers = ref<OrganizationUserRelationDto[] | null>();
const tableHeaders = ['Name', 'Email', 'Role', 'Member Since', 'Actions'];

const canManageUsers = computed(() => {
    return permissions.value.developerPermissionsDto?.canManageUsers || workspacePermissions.value?.canManageUsers || isAdmin();
});

async function loadWorkspacePermissions(): Promise<void> {
    if (permissions.value.isDeveloper) {
        return;
    }
    isLoading.value = true;
    try {
        workspacePermissions.value = await $api.workspaces.permissions(workspaceSlug.value);
    } catch (error) {
        handleApiError(error, 'Unable to get user permissions');
    } finally {
        isLoading.value = false;
    }
}

async function loadUsersByWorkspaceSlug(workspaceSlug: string): Promise<OrganizationUserRelationDto[] | null> {
    isLoading.value = true;
    try {
        return await $api.workspaces.listUsers(workspaceSlug);
    } catch (error) {
        handleApiError(error, 'Unable to load workspace users');
        return null;
    } finally {
        isLoading.value = false;
    }
}

async function removeUser(user: OrganizationUserRelationDto) {
    const {isCancelled} = await confirmation("Are you sure you want to remove this user? This action cannot be undone.", "Remove User")
    if(isCancelled){
        return;
    }

    const orgUserModificationModel: WorkspaceUserModificationModel = {
        usersToRemove: [user.userId],
        workspaceSlug: workspaceSlug.value,
        usersToAddOrModify: []
    };

    isLoading.value = true;
    try {
        await $api.workspaces.updateUsers(workspaceSlug.value, orgUserModificationModel);
        workspaceUsers.value = await loadUsersByWorkspaceSlug(workspaceSlug.value);
    } catch (error) {
        handleApiError(error, 'Unable to remove member');
    } finally {
        isLoading.value = false;
    }
}

async function updateUserRole(userId:string, userRole: OrganizationRoles, organizationId: string) {
    const createModifyOrganizationUserRelationDto: CreateModifyOrganizationUserRelationDto = {
        applicationUserId: userId,
        organizationRole: userRole,
        organizationId
    };
    const orgUserModificationModel: WorkspaceUserModificationModel = {
        usersToRemove: [],
        workspaceSlug: workspaceSlug.value,
        usersToAddOrModify: [createModifyOrganizationUserRelationDto]
    };
    isLoading.value = true;
    try {
        await $api.workspaces.updateUsers(workspaceSlug.value, orgUserModificationModel);
    } catch (error) {
        handleApiError(error, 'Unable to update user role');
    } finally {
        workspaceUsers.value = await loadUsersByWorkspaceSlug(workspaceSlug.value);
        isLoading.value = false;
    }
}

async function closeInviteModal() {
    openModal.value = false;
    workspaceUsers.value = await loadUsersByWorkspaceSlug(workspaceSlug.value);
}

loadWorkspacePermissions()
    .then(async() => {
        if (workspaceSlug.value) {
            workspaceUsers.value = await loadUsersByWorkspaceSlug(workspaceSlug.value);
        }
    });
</script>

<template>
  <DsContainer class="space-y-4">
    <DsLoadingOverlay :loading="isLoading" />

    <InviteWorkspaceUserModal
      v-if="canManageUsers"
      :show-modal="openModal"
      :workspace-id="workspaceSlug"
      @close="closeInviteModal"
    />

    <!-- Header Text -->
    <DsText size="2xl" weight="light">
      Workspace Users
    </DsText>
    <DsText size="sm" weight="light" class="block">
      Manage the users for this workspace.
    </DsText>

    <!-- No Users -->
    <div v-if="workspaceUsers && workspaceUsers.length === 0">
      <div class="flex flex-col items-center justify-center h-full py-20 border-2 border-silver space-y-5">
        <DsText size="2xl" weight="light">
          No Workspace Users
        </DsText>
        <DsText size="sm" weight="light">
          This workspace has no invited users. It is currently only accessible by Team Members.
        </DsText>
        <DsButton id="invite-workspace-users" :disabled="!canManageUsers" :color="Colors.primary" variant="filled" @click="(openModal=true)">
          <DsIcon name="heroicons:plus" size='sm' />
          Invite Workspace Users
        </DsButton>
      </div>
    </div>

    <div v-else class="space-y-5">

      <!-- Modal for Invite New Members -->
      <DsButton id="invite-workspace-users" :disabled="!canManageUsers" :color="Colors.primary" variant="filled" class="mx-1" @click="openModal=true" >
        <DsIcon name="heroicons:plus" size='sm' />
        Invite Workspace Users
      </DsButton>

      <!-- Table for Current Team Members -->
      <DsTable
        v-if="workspaceUsers"
        :headers="tableHeaders"
        :items="workspaceUsers"
        :id-selector="item => item.userId"
      >
        <template #default="{item}">
          <DsText size="xs">
            {{ item.firstName + ' ' + item.lastName }}
          </DsText>

          <DsText size="xs">
            {{ item.email }}
          </DsText>

          <div>
            <DsSelect v-if="canManageUsers" :model-value="item.organizationRole" :items="roleConfig" @update:model-value="(e) => updateUserRole(item.userId, e, item.organizationId)" />
            <DsText v-else size="xs">
              {{ item.organizationRole }}
            </DsText>
          </div>

          <DsText size="xs">
            {{ new Intl.DateTimeFormat('en-US', { year: 'numeric', month: 'long', day: 'numeric' }).format(new Date(item.createdAt)) }}
          </DsText>

          <DsButton :disabled="!canManageUsers" :color="Colors.fire" :text-color='Colors.fire' variant="outline" @click="removeUser(item)">
            <DsIcon name="heroicons:x-mark" />
            Remove
          </DsButton>
        </template>
      </DsTable>
    </div>
  </DsContainer>
</template>
