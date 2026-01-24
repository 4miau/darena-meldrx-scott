<script setup lang="ts">
import type { CreateUserInOrgCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CreateUserInOrgCommand';
import type { OrganizationRoles} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/OrganizationRoles';
import { roleConfig } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/OrganizationRoles'
import { Colors } from '~/types/ui/colors';

const props = defineProps<{
  organizationId: string;
}>();

const emit = defineEmits<{
    'closeModal': [];
}>();

const { $api } = useNuxtApp();


const isLoading = ref<boolean>(false);
const inviteMemberForm = ref<{
  firstName: string,
  lastName: string,
  email: string,
  role?: OrganizationRoles
}>({
    firstName: '',
    lastName: '',
    email: ''
});

const formRef = ref<FormRef>();
const errorMessage = ref<string>('');

async function submitInviteMember() {
    if (!formRef.value?.validate()) {
        return;
    }

    isLoading.value = true;
    try {
        const orgUserModificationModel: CreateUserInOrgCommand = {
            firstName: inviteMemberForm.value.firstName,
            lastName: inviteMemberForm.value.lastName,
            email: inviteMemberForm.value.email,
            organizationRole: inviteMemberForm.value.role!,
            organizationId: props.organizationId
        };
        await $api.organizations.createUsers(props.organizationId, orgUserModificationModel);
        emit('closeModal');
    } catch (error) {
        handleApiError(error, 'Unable to send invitation');
    }

    isLoading.value = false;
}
</script>

<template>
  <DsLoadingOverlay :loading="isLoading" />
  <div class='w-full'>
    <!-- Header -->
    <div class="flex items-center justify-center">
      <DsText size="2xl" weight="light">
        Invite Team Member
      </DsText>
    </div>
    <div class="pb-5" />

    <!-- Text Inputs -->
    <DsText size="sm">
      Provide details for the member to invite to this organization
    </DsText>
    <div class="pb-5" />

    <DsForm ref="formRef">
      <DsTextInput v-model="inviteMemberForm.firstName" class="mb-4" label="First Name" required :rules="[ValidationRules.describeNotEmpty('Please provide a first name')]" />
      <DsTextInput v-model="inviteMemberForm.lastName" class="mb-4" label="Last Name" required :rules="[ValidationRules.describeNotEmpty('Please provide a last name')]" />
      <DsTextInput v-model="inviteMemberForm.email" class="mb-4" label="Email Address" required :rules="ValidationRules.email" />
      <DsSelect v-model="inviteMemberForm.role" :items="roleConfig" label="Role" required :rules="[ValidationRules.describeNotEmpty('Please select a role')]" />
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
    <div class="flex justify-center gap-4">
      <DsButton :color="Colors.white" variant="subtle" :text-color='Colors.gray' @click="$emit('closeModal')">
        Cancel
      </DsButton>
      <DsButton :color="Colors.secondary" variant="filled" @click="submitInviteMember">
        Invite Team Member
      </DsButton>
    </div>
  </div>
</template>