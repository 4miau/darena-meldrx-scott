<script setup lang="ts">

import {Colors} from "~/types/ui/colors";
import type {WorkspaceDto} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto";
import type {IDsSelectItem} from "~/types/ui/DsSelect";
import type {PublishedAppDetails} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/PublishedAppDetails";
import type {AddWorkspaceEhrAppCommand} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/AddWorkspaceEhrAppCommand";


const { $api } = useNuxtApp();
const formRef = ref<FormRef>()

const props = defineProps<{
    show: boolean
    app: PublishedAppDetails
}>();

const emits = defineEmits<{
  'close': [];
}>();

const state = reactive<{
  isLoading: boolean;
  workspaces: WorkspaceDto[];
  currentStep: number;
  selectedWorkspaceId: string,
}>({
    isLoading: false,
    workspaces: [],
    currentStep: 1,
    selectedWorkspaceId: ""
});

const selectedWorkspace = computed(() => state.workspaces.find(x => x.id === state.selectedWorkspaceId));
const optionsWorkspaces = computed<IDsSelectItem<string>[]>(
    () => state.workspaces.map(x => ({ value: x.id, title: x.name }))
);
async function loadWorkspaces() {
    state.isLoading = true;

    try {
        state.workspaces = await $api.marketplace.getWorkspacesForActivation(props.app.appId);
    } catch (error) {
        handleApiError(error, 'Unable to load workspaces');
    }

    state.isLoading = false;
}


function onNextClick(){
    if (!formRef.value || !formRef.value) {
        return;
    }

    const isFormValid = formRef.value.validate();
    if (!isFormValid) {
        return;
    }
    state.currentStep = 2
}

async function addExtensionToWorkspace() {
    if(!selectedWorkspace.value){
        return
    }

    state.isLoading = true;
    try {
        const orgUserModificationModel: AddWorkspaceEhrAppCommand = { appId: props.app.appId, workspaceId:selectedWorkspace.value.id };
        await $api.ehr.create(selectedWorkspace.value.fhirDatabaseDisplayName, orgUserModificationModel);
        notification({
            title: 'Success',
            description: 'Extension has been added to your workspace',
            displayTime: 5000,
            variant: 'success',
            links: [{ title: 'Go To Workspace', link: `workspaces/${selectedWorkspace.value.fhirDatabaseDisplayName}/extensions` }]
        })
    } catch (error) {
        handleApiError(error, 'Unable to add extension to workspace');
    } finally {
        state.isLoading = false;
        emits('close');
    }
}

onMounted(() => {
    loadWorkspaces();
});
</script>

<template>
  <DsModal :model-value="show" @close="$emit('close')">
    <DsModalProgressCard :total-steps="2" :current-step="state.currentStep"/>
    <div class="flex w-full py-8 bg-space justify-center items-center">
      <div class="flex w-full items-center justify-center">
        <div class="w-14 h-14 rounded-full flex justify-center items-center pt-2">
          <MeldRxApp class="mb-2" />
        </div>
      </div>
    </div>
    <div class="p-4 space-y-4">
      <!-- Step 1 select a workspace -->
      <div v-if="state.currentStep === 1" class="space-y-4">
        <div v-if="state.workspaces.length > 0" class="w-full flex-col justify-center items-center gap-5 flex">
            <DsText size="2xl" weight="light">
              Add Extension to Workspace
            </DsText>
    
            <DsText size="sm" weight="light" class="text-center">
              Select a workspace to add this extension to.
            </DsText>
        </div>
        <DsForm v-if="state.workspaces.length > 0" ref="formRef">
          <DsSelect
              v-model="state.selectedWorkspaceId"
              searchable
              :items="optionsWorkspaces"
              placeholder="Select a workspace"
              search-placeholder="Search Workspaces"
              required
             :rules="[
                 [v => !!v, 'Please select a workspace.'],
                 [v => v!.trim().length > 0, 'Please select a workspace.'],
                 ]"
          />
        </DsForm>
        <div v-else class="flex items-center">
          <DsBanner icon="i-heroicons-information-circle" icon-size="md" :color="Colors.onyx">
            There are no workspaces that this extension can be added to.
          </DsBanner>
        </div>
      </div>

      <!-- Step 2 consent screen -->
      <div v-if="state.currentStep === 2 && selectedWorkspace" class="space-y-4">
        <div class="w-full flex-col justify-center items-center gap-5 flex">
          <DsText size="2xl" weight="light">
            Add Extension to Workspace
          </DsText>


          <div class="flex gap-3 items-center">
            <div>
              <img v-if="props.app.logoUrl" :src="props.app.logoUrl" class="w-[50px] h-[50px]" alt="The logo of the organization">
              <DefaultAppCatalogLogo v-else/>
            </div>
            <div class="flex flex-col">
              <div>
                <DsText size="xl" weight="light">
                  {{ props.app.appName }}
                </DsText>
              </div>
              <div class="flex space-x-1">
                <DsText size="md" weight="light">
                  By:
                </DsText>
                <DsLink :href="props.app.publisherUrl" target="_blank">
                  <DsText size="md" weight="light">
                    {{ props.app.organizationName }}
                  </DsText>
                </DsLink>
              </div>
            </div>
          </div>
          
          <DsText size="md">
            Selected Workspace: {{ selectedWorkspace.name }}
          </DsText>
        </div>
        
        <div>
         <DsText size="sm">
           By clicking <strong>Accept</strong>, I agree to the provider's
           <DsLink :href="props.app.termsOfServiceUrl" class="text-lapis-lazuli">
             terms of use
           </DsLink>
           and
           <DsLink :href="props.app.privacyPolicyUrl" class="text-lapis-lazuli">
            privacy policy
           </DsLink>
           and understand that the rights to use this product do not come from Darena Solutions, unless Darena Solutions is the provider.<br>
           Use of MeldRx is governed by separate
           <DsLink href="https://www.darenasolutions.com/terms-of-service" class="text-lapis-lazuli">
             terms
           </DsLink>
           and
           <DsLink href="https://www.darenasolutions.com/privacy-policy" class="text-lapis-lazuli">
             privacy
           </DsLink>
           .
         </DsText>
        </div>
      </div>
      
      <DsDivider/>
      
      <div class="flex justify-center gap-3">
        <DsButton id="cancel-button" :color="Colors.secondary" :text-color='Colors.secondary' variant="subtle" @click="$emit('close')">
          Cancel
        </DsButton>
        <DsButton v-if="state.currentStep === 2" id="previous-button" :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" @click="state.currentStep = state.currentStep - 1">
          Previous
        </DsButton>
        <DsButton v-if="state.currentStep === 1" id="next-button" :disabled="state.workspaces.length == 0" :color="Colors.secondary" variant="filled" @click="onNextClick">
          Next
        </DsButton>
        <DsButton v-if="state.currentStep === 2" id="accept-button" :color="Colors.secondary" variant="filled" @click="addExtensionToWorkspace">
          Accept
        </DsButton>
      </div>
    </div>
  </DsModal>
</template>
