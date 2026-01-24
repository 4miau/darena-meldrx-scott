<script setup lang="ts">
import type { CreateUserInWorkspaceCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CreateApplicationUserDto';
import type { OrganizationRoles} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/OrganizationRoles';
import { roleConfig } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/OrganizationRoles'
import { Colors } from '~/types/ui/colors';

const props = defineProps<{
  workspaceId: string;
}>();

const emit = defineEmits<{
    'closeModal': [];
}>();

const { $api } = useNuxtApp();


const isLoading = ref<boolean>(false);
const formRef = ref<FormRef>();
const errorMessage = ref<string>('');
const inviteMemberForm = ref<{
  firstName: string,
  lastName: string,
  email: string,
  role?: OrganizationRoles,
}>({
    firstName: '',
    lastName: '',
    email: ''
})

async function submitInviteUser() {
    if (!formRef.value?.validate()) {
        return;
    }

    isLoading.value = true;
    try {
        const orgUserModificationModel: CreateUserInWorkspaceCommand = {
            firstName: inviteMemberForm.value.firstName,
            lastName: inviteMemberForm.value.lastName,
            email: inviteMemberForm.value.email,
            organizationRole: inviteMemberForm.value.role!,
            workspaceSlug: props.workspaceId
        };
        await $api.workspaces.createUsers(props.workspaceId, orgUserModificationModel);
        emit('closeModal');
    } catch (error) {
        handleApiError(error, 'Unable to send invitation');
    }

    isLoading.value = false;
}
</script>

<template>
    <div class="flex flex-col w-full">
      <!-- Header -->
      <div class="w-full flex-col justify-center items-center gap-5 flex">
        <DsText size="2xl" weight="light">
          Invite Workspace User
        </DsText>
      </div>
      <div class="pb-5" />

      <!-- Text Inputs -->
      <DsText size="sm">
        Provide details for the user to invite to this workspace
      </DsText>
      <div class="pb-5" />

      <DsForm ref="formRef">
        <!-- Main Form -->
        <DsTextInput v-model="inviteMemberForm.firstName" class="mb-4" label="First Name" required :rules="[ValidationRules.describeNotEmpty('Please provide a first name')]" />
        <DsTextInput v-model="inviteMemberForm.lastName" class="mb-4" label="Last Name" required :rules="[ValidationRules.describeNotEmpty('Please provide a last name')]" />
        <DsTextInput v-model="inviteMemberForm.email" class="mb-4" label="Email Address" required :rules="ValidationRules.email" />
        <DsSelect v-model="inviteMemberForm.role" placeholder="Select a Role" :items="roleConfig" label="Role" required :rules="[ValidationRules.describeNotEmpty('Please select a role')]" />

        <!-- Error Messages -->
        <div v-if="!!errorMessage">
          <div class="p-4">
            <DsText size="sm" :color="Colors.fire">
              {{ errorMessage }}
            </DsText>
          </div>
        </div>
      </DsForm>
      <DsDivider />
      <div class="flex justify-center w-full gap-5">
        <DsButton :color="Colors.white" :text-color='Colors.gray' variant="subtle" @click="$emit('closeModal')">
          Cancel
        </DsButton>
        <DsButton :color="Colors.secondary" variant="filled" @click="submitInviteUser">
          Invite Workspace User
        </DsButton>
      </div>
    </div>
</template>
