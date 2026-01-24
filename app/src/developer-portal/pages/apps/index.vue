<script setup lang="ts">
import type DynamicRegistrationDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/DynamicRegistrationDto';
import { Colors } from '~/types/ui/colors';
useHead({ title: 'Apps | MeldRx' });

const { $api } = useNuxtApp();

const { permissions } = useAuth()

interface IState { apps: DynamicRegistrationDto[]; }
const state = useState<IState>('apps', () => ({
    apps: []
}));

const tableHeaders = ['App Name', 'User Type', 'Actions'];
const canCreateApps = computed(() =>
    !!permissions.value.developerPermissionsDto?.canCreateApps
)
async function loadApps (): Promise<DynamicRegistrationDto[]> {
    isLoading.value = true;

    let apps: DynamicRegistrationDto[] = [];
    try {
        apps = await $api.apps.list();
        state.value.apps = apps;
    } catch (error) {
        handleApiError(error, 'Unable to load apps');
    }

    isLoading.value = false;
    return apps;
}

async function onCreateNewApp() {
    await navigateTo('/apps/new');
}
function onManageApp(clientId: string) {
    navigateTo(`/apps/${clientId}`);
}

const isLoading = ref<boolean>(false);
loadApps();
</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="isLoading"/>

    <div>
      <DsText size="2xl" weight="light">
        Apps
      </DsText>
      <div class="pb-5"/>

      <DsText size="sm" weight="light">
        Your app is a powerful platform designed to connect SMART on FHIR applications, making the integration with various EHR and other systems effortless and
        secure. By provding a streamlined process for entering linked app details and setting up authentication methods, it ensures a seamless setup experience.
      </DsText>
      <div class="pb-5"/>

      <DsButton
          :color="Colors.primary"
          :disabled="!canCreateApps"
          variant="filled"
          @click="onCreateNewApp"
      >
         <DsIcon name='heroicons:plus' />
        Register App
      </DsButton>
      <div class="pb-5"/>
    </div>

    <DsTable
        v-if="state.apps"
        :headers="tableHeaders"
        :items="state.apps"
        :id-selector="item => item.client_name"
    >
      <template #default="{item}">
        <div class="flex-col">
          <DsLink underline="always" :href="`apps/${item.client_id}`">
            <DsText size="lg" weight="light">
              {{ item.client_name }}
            </DsText>
          </DsLink>
          <DsTextWithCopyButton
              size="xs"
              :text="`App ID / Client ID: ${item.client_id}`"
              :text-to-copy="item.client_id"
              :show-toast-on-copy="true"
              toast-message-on-copy="Copied App ID / Client ID to clipboard."
          />
        </div>

        <DsText size="md" weight="normal">
          {{ item.soFAppUserType }}
        </DsText>
        <div class="flex space-x-2">
          <DsButton
              :id="`manage-${item.client_name.replaceAll(' ', '-').toLowerCase()}-button`"
              :color="Colors.white"
              :text-color='Colors.gray'
              variant="outline"
              @click="onManageApp(item.client_id)"
          >
            Manage
            <DsIcon name="heroicons:arrow-small-right"/>
          </DsButton>
        </div>
      </template>
    </DsTable>
  </DsContainer>
</template>