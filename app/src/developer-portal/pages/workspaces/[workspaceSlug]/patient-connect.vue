<script setup lang="ts">
import type {
    CreateWorkspaceExternalApp,
    UpdateWorkspaceExternalApp,
    WorkspaceExternalApp
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceExternalApp";
import type {FhirServerSettings} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/FhirServerSettings";
import { Colors } from '~/types/ui/colors';


definePageMeta({layout: 'workspace', middleware: ['require-workspace']});
useHead({title: 'Patient Connect | MeldRx'});

const {$api} = useNuxtApp();
const confirmation = useConfirmation()
const route = useRoute();
const {workspace} = useWorkspace();
const workspaceSlug = ref<string>(route.params.workspaceSlug as string);
const inviteUrl = location.origin + `/invite/workspace/${workspaceSlug.value}`

const state = reactive<{
  workspaceSettings?: FhirServerSettings;
  externalApps?: WorkspaceExternalApp[];
  selectedExternalApp?: WorkspaceExternalApp;
  loading: boolean;
  showExternalAppModal: boolean;
  settingsSaved: boolean;
}>({
    loading: false,
    showExternalAppModal: false,
    settingsSaved: false,
})

const formRef = ref<FormRef>();

async function loadExternalApps(workspaceSlug: string)
{
    state.loading = true;
    try {
        state.externalApps = await $api.workspaces.externalApps.getWorkspaceExternalApps(workspaceSlug)
    } catch (error) {
        handleApiError(error)
    } finally {
        state.loading = false;
    }
}

async function createExternalApp(externalApp: CreateWorkspaceExternalApp)
{
    if (!state.externalApps) {
        return;
    }

    state.loading = true;

    try {
        await $api.workspaces.externalApps.createWorkspaceExternalApp(workspaceSlug.value, externalApp)
        state.showExternalAppModal = false;
    }
    catch (error) {
        handleApiError(error)
    }
    finally {
        state.loading = false;
    }
    await loadExternalApps(workspaceSlug.value)
}

async function editExternalApp(externalApp: UpdateWorkspaceExternalApp)
{
    if (!state.externalApps) {
        return;
    }

    state.loading = true;

    try {
        await $api.workspaces.externalApps.updateWorkspaceExternalApp(workspaceSlug.value, externalApp)
        state.showExternalAppModal = false;
    }
    catch (error) {
        handleApiError(error)
    }
    finally {
        state.loading = false;
    }
    await loadExternalApps(workspaceSlug.value)
}

async function deleteExternalApp(index: number)
{
    const { isCancelled } = await confirmation(
        'Are you sure you want to delete this External App? This action cannot be undone.',
        'Delete External App'
    )
    if (isCancelled) {
        return
    }
    if (!state.externalApps) {
        return;
    }

    state.loading = true;

    try {
        const appToDelete = state.externalApps[index]
        await $api.workspaces.externalApps.deleteWorkspaceExternalApp(workspaceSlug.value, appToDelete.id)
    }
    catch (error) {
        handleApiError(error)
    }
    finally {
        state.loading = false;
    }
    await loadExternalApps(workspaceSlug.value)
}

function selectExternalApp (index: number) {
    if(state.externalApps == null){
        return
    }
    if (index == -1){
        state.selectedExternalApp = undefined;
        state.showExternalAppModal = true
        return;
    }
    state.selectedExternalApp = state.externalApps[index]
    state.showExternalAppModal = true
}


async function loadSettings () {
    state.workspaceSettings = await $api.workspaces.getSettings(workspaceSlug.value)
}

async function saveSettings () {
  
    if(!state.workspaceSettings){
        return
    }
    
    state.loading = true
  
    try {
        await $api.workspaces.updateSettings(workspaceSlug.value, state.workspaceSettings)
        state.settingsSaved = true
    }
    catch (error) {
        handleApiError(error, 'Unable to save workspace settings');
    }
    finally {
        state.loading = false
    }
}

loadSettings()
loadExternalApps(workspaceSlug.value)
</script>

<template>
  <DsContainer v-if="workspace">
    
    <div class="grid grid-cols-12 gap-14">
      <div class="col-span-8 flex flex-col">
        <DsText size="2xl" weight="light">
          Patient Connect
        </DsText>
        <DsText size="xs" weight="light">
          {{ workspace.name }}
        </DsText>
  
        <DsText size="md" weight="light" class="mt-5">
          View and modify Patient Connect settings for this workspace.
        </DsText>
      </div>
    </div>
    
    <DsDivider/>
    
    <!-- Anonymous invite url -->
    <div class="grid grid-cols-12 gap-14">
      <div class="col-span-4 flex flex-col">
        <DsText class="pb-2" size="sm">
          Anonymous Patient Invite URL
        </DsText>

        <DsText size="xs" weight="light">
          This URL allows patients to send their data for this workspace without first receiving an invitation.
        </DsText>
      </div>
      
      <DsText size="sm" class="col-span-8">
        Invite URL
        <DsTextWithCopyButton
            id="patient-invite-url"
            size="xs"
            :text="inviteUrl"
            :text-to-copy="inviteUrl"
            :show-toast-on-copy="true"
            toast-message-on-copy="Copied Patient Invite URL to clipboard."
        />
      </DsText>
    </div>

    <DsDivider/>

    <!-- Thank You Page -->
    <div v-if="state.workspaceSettings" class="grid grid-cols-12 gap-14">
      <div class="col-span-4 flex flex-col">
        <DsText class="pb-2" size="sm">
          Thank You / Confirmation Page
        </DsText>

        <DsText size="xs" weight="light">
          Set the URL where the patient should be redirected after sharing their data.
          If left blank, the default MeldRx redirect will be used.
        </DsText>
      </div>

      <div  class="col-span-8">
        <div class="flex space-x-3 pb-3">
          <div class="grow">
            <DsForm ref="formRef">
              <DsTextInput
                  v-model="state.workspaceSettings.thankYouPageUrl"
                  label="Thank You Page URL"
                  placeholder="https://app.meldrx.com/thankyou"
                  type="text"
                  :rules="ValidationRules.url"
                  @keydown="state.settingsSaved = false"
              />
            </DsForm>
          </div>
        </div>
        <div v-if="state.settingsSaved && !state.loading" class="flex space-x-1 items-center">
          <DsIcon name="heroicons:check-circle" size='sm' :color='Colors.primary' />
          <DsText size="sm">
            URL saved.
          </DsText>
        </div>
      </div>
    </div>

    <DsDivider/>
    
    <!-- External apps -->
    <div class="grid grid-cols-12 gap-14">
      <div class="col-span-4 flex flex-col">
        <DsText class="pb-2" size="sm">
          External Apps
        </DsText>
  
        <DsText size="xs" weight="light">
          Configure the external apps to be used for this workspace when requesting patient data.
        </DsText>
      </div>
  
      <div v-if="state.externalApps" class="col-span-6">
        <ExternalAppList
            :external-apps="state.externalApps"
            @add-external-app="selectExternalApp(-1)"
            @edit-external-app="(i:number) => selectExternalApp(i)"
            @delete="(i:number) => deleteExternalApp(i)"
        />
      </div>
    </div>

    <DsDivider/>

    <DsButton size="md" @click="saveSettings">
      Save
    </DsButton>

    <ExternalAppModal
        :show="state.showExternalAppModal"
        :external-app="state.selectedExternalApp"
        :organization-id="workspace.id"
        @add-external-app="(v) => createExternalApp(v)"
        @edit-external-app="(v) => editExternalApp(v)"
        @close="state.showExternalAppModal = false;"
    />

  </DsContainer>
</template>