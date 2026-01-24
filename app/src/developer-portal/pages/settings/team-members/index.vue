<script setup lang="ts">
import type { OrganizationUserModificationModel } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Organization/OrganizationUserModificationModel';
import type { CreateModifyOrganizationUserRelationDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Organization/CreateModifyOrganizationUserRelationDto';
import type { OrganizationUserRelationDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/OrganizationUserRelationDto';
import { Colors } from '~/types/ui/colors';
import { type OrganizationRoles, roleConfig } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/OrganizationRoles';

useHead({ title: 'Team Members | MeldRx' });
const { $api } = useNuxtApp();

const confirmation = useConfirmation();
const { permissions } = useAuth()

const isLoading = ref<boolean>(false);
const openModal = ref(false);
const teamMembers = ref<OrganizationUserRelationDto[] | null>();
const canManageUsers = computed(() => !!permissions.value?.developerPermissionsDto?.canManageUsers);
const organizationId = computed(() => permissions.value?.developerPermissionsDto?.organizationId ?? '');

const tableHeaders = ['Name', 'Email', 'Role', 'Member Since', 'Actions'];

async function loadTeamMembersByOrganization(organizationId: string) {
    isLoading.value = true;
    try {
        teamMembers.value = await $api.organizations.listUsers(organizationId);
    } catch (error) {
        handleApiError(error, 'Unable to load team members');
    } finally {
        isLoading.value = false;
    }
}


async function removeMember(memberId: string) {
    const {isCancelled} = await confirmation("Are you sure you want to delete this member? This action cannot be undone.", "Remove Member")
    if(isCancelled){
        return;
    }

    const orgUserModificationModel: OrganizationUserModificationModel = {
        usersToRemove: [memberId],
        organizationId: organizationId.value,
        usersToAddOrModify: []
    };

    isLoading.value = true;
    try {
        await $api.organizations.updateUsers(organizationId.value, orgUserModificationModel);
        await loadTeamMembersByOrganization(organizationId.value);
    } catch (error) {
        handleApiError(error, 'Unable to remove member');
    }

    isLoading.value = false;
}

async function updateMemberRole(userId:string, userRole: OrganizationRoles) {
    const createModifyOrganizationUserRelationDto: CreateModifyOrganizationUserRelationDto = {
        applicationUserId: userId,
        organizationRole: userRole,
        organizationId: organizationId.value
    };
    const orgUserModificationModel: OrganizationUserModificationModel = {
        usersToRemove: [],
        organizationId: organizationId.value,
        usersToAddOrModify: [createModifyOrganizationUserRelationDto]
    };
    isLoading.value = true;
    try {
        await $api.organizations.updateUsers(organizationId.value, orgUserModificationModel);
        await loadTeamMembersByOrganization(organizationId.value);
    } catch (error) {
        handleApiError(error, 'Unable to update user role');
    }
    isLoading.value = false;
}

async function onInviteModalClose() {
    openModal.value = false;
    await loadTeamMembersByOrganization(organizationId.value);
}

loadTeamMembersByOrganization(organizationId.value);

</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="isLoading" />

    <!-- Header Text -->
    <DsText size="2xl" weight="light">
      Team Members
    </DsText>
    <div class="pb-5" />

    <DsText size="sm" weight="light" class="block">
      Manage the team members for your organization
    </DsText>

    <!-- Modal for Invite New Members -->
    <div class="pb-5" />
    <DsButton
      :disabled="!canManageUsers"
      :color="Colors.primary"
      variant="filled"
      class="mx-1"
      @click="openModal=true"
    >
      <DsIcon name="heroicons:plus" size='sm' />
      Invite Team Member
    </DsButton>
    <InviteMemberModal
      v-if="canManageUsers"
      :show-modal="openModal"
      :organization-id="organizationId"
      @close="onInviteModalClose"
    />

    <!-- Table for Current Team Members -->
    <div class="pb-8" />

    <DsTable
      v-if="teamMembers"
      :headers="tableHeaders"
      :items="teamMembers"
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
          <DsSelect v-if="canManageUsers" :model-value="item.organizationRole" :items="roleConfig" @update:model-value="(e) => updateMemberRole(item.userId, e)" />
          <DsText v-else size="xs">
            {{ item.organizationRole }}
          </DsText>
        </div>

        <DsText size="xs">
          {{ new Intl.DateTimeFormat('en-US', { year: 'numeric', month: 'long', day: 'numeric' }).format(new Date(item.createdAt)) }}
        </DsText>

        <DsButton
          :disabled="!canManageUsers"
          :color="Colors.fire"
          :text-color='Colors.fire'
          variant="outline"
          @click="removeMember(item.userId)"
        >
          <DsIcon name="heroicons:x-mark" size='sm' />
          Remove
        </DsButton>
      </template>
    </DsTable>
  </DsContainer>
</template>
