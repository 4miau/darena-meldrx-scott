<script setup lang="ts">
import type {
    CreateCdsHooksAppCommand
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CreateCdsHooksAppCommand";
import {CdsAppType} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/CdsAppType";
import {Colors} from "~/types/ui/colors";
import type {
    CreateDeveloperAppCommandResult
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CreateDeveloperApp";

definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    allowLinked: true,
})
useHead({ title: 'Register New Extenstion | MeldRx' });

const { $api } = useNuxtApp();

const confirmation = useConfirmation();
const route = useRoute()
const workspaceSlug = ref<string>(route.params.workspaceSlug as string);

const pageState = ref<'new-app' | 'app-created'>('new-app');


const formRef = ref<FormRef>()
const state = reactive<{
    createCdsHookAppCommand: CreateCdsHooksAppCommand,
    createdExtension?: CreateDeveloperAppCommandResult,
    cdsAppType: CdsAppType,
    errorMessage: string,
    isLoading: boolean,
    showExtensionCreated: boolean
}>({
    createCdsHookAppCommand: {},
    cdsAppType: CdsAppType.External,
    errorMessage: '',
    isLoading: false,
    showExtensionCreated: false
})

const cdsHookOptions : {value: CdsAppType, title: string}[] = [
    {
        value: CdsAppType.External,
        title: 'Service URL'
    },
    {
        value: CdsAppType.Hosted,
        title: 'Upload CQL (ELM Files)'
    },
    {
        value: CdsAppType.HostedCustomCql,
        title: 'Custom CQL'
    }
]

async function createCdsHookApp () {
    if (state.isLoading) {
        return
    }
    if (!formRef.value?.validate()){
        return
    }

    state.isLoading = true
    try {
        const formData = new FormData()
        formData.append('Name', state.createCdsHookAppCommand.name!)
        if (state.createCdsHookAppCommand.cdsHookServiceUrl) {
            formData.append('CdsHookServiceUrl', state.createCdsHookAppCommand.cdsHookServiceUrl)
        }

        if (state.createCdsHookAppCommand.cdsHook) {
            formData.append('CdsHook.Title', state.createCdsHookAppCommand.cdsHook.title)
            formData.append('CdsHook.Hook', state.createCdsHookAppCommand.cdsHook.hook)
            formData.append('CdsHook.Description', state.createCdsHookAppCommand.cdsHook.description)
            formData.append('CdsHook.UsageRequirements', state.createCdsHookAppCommand.cdsHook.usageRequirements)
        }

        state.createCdsHookAppCommand.cards?.forEach((card, index) => {
            formData.append(`Cards[${index}].Condition`, card.condition)
            formData.append(`Cards[${index}].Detail`, card.detail)
            formData.append(`Cards[${index}].Indicator`, card.indicator)
            formData.append(`Cards[${index}].Summary`, card.summary)
        })

        if(state.createCdsHookAppCommand.elmFiles){
            for (const file of state.createCdsHookAppCommand.elmFiles) {
                formData.append('Elms', file)
            }
        }

        if(state.createCdsHookAppCommand.cqlEditorArtifact){
            formData.append('CqlEditorArtifact', JSON.stringify(state.createCdsHookAppCommand.cqlEditorArtifact))
        }

        state.createdExtension = await $api.workspaces.extensions.createWorkspaceExtension(workspaceSlug.value, formData)
        pageState.value = 'app-created'
        state.showExtensionCreated =true
    } catch (error) {
        handleApiError(error, 'Unable to register extension')
    } finally {
        state.isLoading = false
    }
}

async function onCancel() {
    // Check if user really wants to cancel it first...
    const { isCancelled } = await confirmation(
        'Are you sure you want to stop creating this extension? All of your progress will be permanently lost. This action cannot be undone.',
        'Discard Extension'
    );
    if (isCancelled) { return; }

    navigateTo(`/workspaces/${workspaceSlug.value}/workspace-extensions`);
}
function goToExtensions(appId?:string){
    const url = `/workspaces/${workspaceSlug.value}/workspace-extensions`+ (appId ? `/${appId}` : '')
    navigateTo(url);
}
</script>

<template>

  <DsModal :model-value="state.showExtensionCreated" @close="goToExtensions()">
    <div class="flex flex-col items-center p-5 space-y-5">
      <DsText size="lg">
        Extension Registered
      </DsText>

      <DsText size="sm" weight="light">
        You can edit the extension details and configure source attributes on the manage page.
        <br>The extension can be activated from the `Active Extensions` tab in the `My Organization` option.
      </DsText>

      <div class="space-x-3">
        <DsButton :color="Colors.secondary" @click="goToExtensions()">
          Return to Extensions
        </DsButton>
        <DsButton :color="Colors.primary" @click="goToExtensions(state.createdExtension?.clientId)">
          Continue to Extension
        </DsButton>
      </div>
    </div>
  </DsModal>

  <DsContainer>
    <DsForm ref="formRef">
      <DsLoadingOverlay :loading="state.isLoading" />

      <!-- New Extension -->
      <div v-if="pageState === 'new-app'" class="space-y-2">
        <DsText size="2xl" weight="light">
          Register Extension
        </DsText>

        <div class="space-y-8">
          <!-- Extension Details -->
          <div class="grid md:grid-cols-12 gap-8">
            <div class="col-span-4">
              <DsLabeledText
                  label="Extension Details"
                  text="Define the basic information about your extension that will be visible to others."
              />
            </div>

            <div class="col-span-8">
              <DsTextInput
                  v-model="state.createCdsHookAppCommand.name"
                  type="text"
                  label="Extension Name"
                  placeholder=""
                  :required="true"
                  :disabled="false"
                  :rules="[
                [v => !!v, 'Extension Name is required'],
                [v => v!.trim().length > 0, 'Extension Name is required'],
                [v => v!.length <= 200, 'Extension Name must be 200 characters or less']
              ]"
              />
            </div>
          </div>

        <DsDivider/>

        <div class="grid md:grid-cols-12 gap-8">
          <div class="col-span-4">
            <DsLabeledText
                label="CDS Hook Details"
                text="Specify how the CDS Hook service is accessed. You can share the CDS Service URL. You can also upload ELM files for a CQL to host in MeldRx CDS services."
            />
          </div>

          <div class="col-span-8 space-y-5">
            <DsSingleSelectButtonList
                v-model="state.cdsAppType"
                :options="cdsHookOptions"
            />
          </div>
        </div>

        <div v-if="state.cdsAppType == CdsAppType.External" class="grid md:grid-cols-12 gap-8">
          <div class="col-span-4">
            <DsLabeledText
                label="CDS Hook Service URL"
                text="Specify the Service URL for your CDS Hook Service."
            />
          </div>

          <div class="col-span-8 space-y-5">
            <DsTextInput
                v-model="state.createCdsHookAppCommand.cdsHookServiceUrl"
                type="text"
                label="CDS Hook Service URL"
                placeholder="https://sandbox-services.cds-hooks.org/cds-services/patient-greeting"
                :required="true"
                :disabled="false"
                :rules="ValidationRules.url"
            />
          </div>
        </div>

        <CdsHookAppElmForm
            v-else-if="state.cdsAppType === CdsAppType.Hosted"
            v-model="state.createCdsHookAppCommand"
        />
        <CdsHookAppCqlEditorForm
            v-else-if="state.cdsAppType == CdsAppType.HostedCustomCql"
            v-model="state.createCdsHookAppCommand"
        />
          <DsDivider/>

          <!-- Cancel/Next Buttons -->
          <div class="justify-start items-start gap-5 inline-flex">
            <DsButton :color="Colors.white" :text-color='Colors.gray' variant="subtle" @click="onCancel">
              Cancel
            </DsButton>
            <DsButton :color="Colors.primary" @click="createCdsHookApp">
              Register Extension
            </DsButton>
          </div>
        </div>
      </div>
    </DsForm>
  </DsContainer>
</template>
