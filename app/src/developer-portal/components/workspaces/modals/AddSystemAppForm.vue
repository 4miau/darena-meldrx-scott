<script setup lang="ts">
import type { CreateAppPermissionCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CreateFhirServerGrantDto';
import type { AppRole} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/AppRole';
import { appRoleConfig } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/AppRole'
import type { IDsSelectItem } from '~/types/ui/DsSelect';
import { Colors } from '~/types/ui/colors';
import ValidationRules from '~/utils/ValidationRules'

const props = defineProps<{
  workspaceId: string;
}>();

const emit = defineEmits<{
    'closeModal': [];
}>();

const { $api } = useNuxtApp();


const isLoading = ref<boolean>(false);
const appsMap = ref<IDsSelectItem<string>[]>([]);
const formRef = ref<FormRef>();

const addAppForm = reactive<{
  inMyOrganization?: boolean,
  clientId?: string,
  appRole?: AppRole
}>({
    inMyOrganization: true,
});

async function loadApps (): Promise<void> {
    isLoading.value = true;
    try {
        const apps = await $api.apps.list();
        appsMap.value = apps
            .filter(app => app.soFAppUserType === 'System')
            .map(x => ({
                value: x.client_id,
                title: x.client_name
            }));
    } catch (error) {
        handleApiError(error, 'Unable to load apps');
    }

    isLoading.value = false;
}

async function addWorkspaceApp() {
    if (!formRef.value?.validate()) {
        return
    }

    isLoading.value = true;
    try {
        const orgUserModificationModel: CreateAppPermissionCommand = {
            clientId: addAppForm.clientId!,
            appRole: addAppForm.appRole!,
            workspaceSlug: props.workspaceId
        };
        await $api.workspaces.createAppAccess(props.workspaceId, orgUserModificationModel);
        emit('closeModal');
    } catch (error) {
        handleApiError(error, 'Unable to add App Permissions');
    }

    isLoading.value = false;
}

loadApps();

</script>

<template>
  <DsForm v-if="appsMap" ref="formRef">
    <div class="flex flex-col w-full space-y-4">
      <!-- Header -->
      <div class="w-full flex-col justify-center items-center gap-5 flex">
        <DsText size="2xl" weight="light">
          Add Workspace System App
        </DsText>

        <DsText size="sm" weight="light" class="text-center">
          Choose a System App to allow access to the workspace
        </DsText>
      </div>
      <!-- Main Form -->
      <div class="grid grid-cols-1 gap-4 px-[60px]">

        <DsLabeledInput label="System App Location" required>
          <DsButtonGroup
              v-model="addAppForm.inMyOrganization"
              :active-color="Colors.primary"
              :active-text-color="Colors.primary"
              :inactive-color="Colors.white"
              :rounded="false"
              space-between
          >
            <DsButton :value="true" size="sm" variant="outline" @click="addAppForm.clientId = ''">
              In my Organization
            </DsButton>
            <DsButton :value="false" size="sm" variant="outline" @click="addAppForm.clientId = ''">
              Not in my Organization
            </DsButton>
          </DsButtonGroup>
        </DsLabeledInput>
        <DsSelect
          v-if="addAppForm.inMyOrganization"
          v-model="addAppForm.clientId"
          :items="appsMap"
          label="Available Apps"
          required
          :rules="[ValidationRules.describeNotEmpty('Please select an app')]"
          searchable
          placeholder="Select an App"
        />
        <DsTextInput
          v-else
          v-model="addAppForm.clientId"
          label="System App Client Id"
          placeholder="Client Id"
          required
        />
        <DsSelect
          v-model="addAppForm.appRole"
          :items="appRoleConfig"
          label="Role"
          required
          :rules="[ValidationRules.describeNotEmpty('Please select a role')]"
          placeholder="Select a Role"
        />
      </div>

      <DsDivider />

      <div class="flex justify-center w-full gap-5">
        <DsButton :color="Colors.white" :text-color='Colors.gray' variant="subtle" @click="$emit('closeModal')">
          Cancel
        </DsButton>
        <DsButton id="add-new-workspace-app-button" :color="Colors.secondary" variant="filled" @click="addWorkspaceApp">
          Add System App
        </DsButton>
      </div>
    </div>
  </DsForm>
</template>
