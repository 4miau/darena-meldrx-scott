<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import type {ApiFilter} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter";
import {DsiOption} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/DsiOption";
import type {SourceAttributeGroup} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/SourceAttribute";
import type {WorkspaceExtensionForm} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceExtensionForm";
import { CdsAppType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/CdsAppType';
import type { CreateCdsHooksAppCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CreateCdsHooksAppCommand';
import type {ChaiModelCardGroup} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/ChaiModelCardForm";

definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    allowLinked:true
})
useHead({ title: 'Apps | MeldRx' });

const { $api } = useNuxtApp();

const confirmation = useConfirmation();

const route = useRoute()
const workspaceSlug = ref<string>(route.params.workspaceSlug as string)

const formRef = ref();
const state = reactive<{
  extension?: WorkspaceExtensionForm;
  apiFilter: ApiFilter,
  isLoading: boolean,
  errorMessage: string,
  showDsiDrawer: boolean,
  showModelCardDrawer: boolean,
  previewModelCard: boolean,
  dsiCaptureStatus: string,
  modelCardCaptureStatus: string,
  form: {
    dsi: DsiOption,
    sourceAttributes?: SourceAttributeGroup[],
    cdsHookApp?: CreateCdsHooksAppCommand,
    chaiModelCard?: ChaiModelCardGroup[]
  },
}>({
    apiFilter: {
        page: 1,
        filter: '',
    },
    isLoading: false,
    errorMessage: '',
    showDsiDrawer: false,
    showModelCardDrawer: false,
    previewModelCard:false,
    dsiCaptureStatus: '',
    modelCardCaptureStatus: '',
    form: {
        dsi: DsiOption.None,
    }
})


// Load the app...
async function loadApp() {
    state.isLoading = true;

    try {
        state.extension = await $api.workspaces.extensions.getWorkspaceExtension(workspaceSlug.value, route.params.appId as string);
        state.form.dsi = state.extension.dsiType;

        if(state.extension.cdsAppType === CdsAppType.HostedCustomCql){
            state.form.cdsHookApp = {
                cdsHook: state.extension.cdsHook,
                cards: state.extension.cards,
                cqlEditorArtifact: JSON.parse(state.extension.cqlEditorArtifact),
            }
        }

    } catch (error) {
        handleApiError(error,'Unable to load extension');
    }

    if (!state.extension) {
        throw showError({ statusCode: 404 })
    }

    state.isLoading = false;
}

async function updateApp () {
    if (state.isLoading) {
        return
    }
    if (!formRef.value?.validate() || !state.extension){
        return
    }

    state.isLoading = true
    try {
        const formData = new FormData()

        formData.append('ClientId', state.extension.clientId)
        formData.append('ClientName', state.extension.clientName)
        formData.append('DsiType', state.form.dsi)

        if(state.extension.cdsAppType === CdsAppType.External){

            if (state.extension.cdsHookServiceUrl) {
                formData.append('CdsHookServiceUrl', state.extension.cdsHookServiceUrl)
            }
        }
        else if(state.extension.cdsAppType === CdsAppType.HostedCustomCql && state.form.cdsHookApp){

            if (state.form.cdsHookApp.cdsHook) {
                formData.append('CdsHook.Title', state.form.cdsHookApp.cdsHook.title)
                formData.append('CdsHook.Hook', state.form.cdsHookApp.cdsHook.hook)
                formData.append('CdsHook.Description', state.form.cdsHookApp.cdsHook.description)
                formData.append('CdsHook.UsageRequirements', state.form.cdsHookApp.cdsHook.usageRequirements)
            }

            state.form.cdsHookApp.cards?.forEach((card, index) => {
                formData.append(`Cards[${index}].Condition`, card.condition)
                formData.append(`Cards[${index}].Detail`, card.detail)
                formData.append(`Cards[${index}].Indicator`, card.indicator)
                formData.append(`Cards[${index}].Summary`, card.summary)
            })

            formData.append('CqlEditorArtifact', JSON.stringify(state.form.cdsHookApp.cqlEditorArtifact))
        }

        if(state.extension.dsiType !== DsiOption.None && state.extension.sourceAttributeGroups){
            for (let i = 0; i < state.extension.sourceAttributeGroups.length; i++) {
                const grp = state.extension.sourceAttributeGroups[i];

                formData.append(`SourceAttributeGroups[${i}].Id`, grp.id.toString())
                formData.append(`SourceAttributeGroups[${i}].Description`, '')
                for (let j = 0; j < grp.sourceAttributeItems.length; j++) {
                    const item = grp.sourceAttributeItems[j];

                    formData.append(`SourceAttributeGroups[${i}].SourceAttributeItems[${j}].Id`, item.id.toString())
                    formData.append(`SourceAttributeGroups[${i}].SourceAttributeItems[${j}].Answer`, item.answer)
                }
            }
        }

        if(state.extension.dsiType === DsiOption.Predictive && state.extension.chaiModelCardGroups){
            for (let i = 0; i < state.extension.chaiModelCardGroups.length; i++) {
                const grp = state.extension.chaiModelCardGroups[i];

                formData.append(`ChaiModelCardGroups[${i}].Id`, grp.id.toString())
                for (let j = 0; j < grp.chaiModelCardItems.length; j++) {
                    const item = grp.chaiModelCardItems[j];

                    formData.append(`ChaiModelCardGroups[${i}].ChaiModelCardItems[${j}].Id`, item.id.toString())
                    formData.append(`ChaiModelCardGroups[${i}].ChaiModelCardItems[${j}].Answer`, item.answer)
                }
            }
        }

        await $api.workspaces.extensions.updateWorkspaceExtension(
            workspaceSlug.value,
            route.params.appId as string,
            formData
        );

        navigateTo(`/workspaces/${workspaceSlug.value}/workspace-extensions`);
    } catch (error) {
        handleApiError(error, 'Unable to save extension');
    } finally {
        state.isLoading = false;
    }
}


async function onDeleteApp() {
    // Check if user really wants to delete it first...
    const { isCancelled } = await confirmation(
        'Are you sure you want to delete this extension? This action cannot be undone.',
        'Delete Extension'
    );
    if (isCancelled) { return; }

    state.isLoading = true;

    try {
        await $api.workspaces.extensions.deleteWorkspaceExtension(workspaceSlug.value ,route.params.appId as string);
    } catch (error) {
        handleApiError(error, 'Unable to delete extension');
        state.isLoading = false;
        return;
    }

    notification({ title: 'Success', description: 'Extension deleted', displayTime: 3000, variant: 'success' })
    state.isLoading = false;
    navigateTo(`/workspaces/${workspaceSlug.value}/workspace-extensions`);
}

watch(
    () => state.form.dsi,
    async (value) => {
        if(value === DsiOption.None){
            state.dsiCaptureStatus = '';
        }
    }
);
watch(
    () => state.showDsiDrawer,
    async (show) => {
        if(!show){
            return;
        }

        if(state.form.dsi === state.extension!.dsiType && state.extension?.sourceAttributeGroups){
            state.form.sourceAttributes = state.extension?.sourceAttributeGroups
        }
        else if (state.form.dsi === DsiOption.Predictive || state.form.dsi === DsiOption.EvidenceBased) {
            try {
                state.isLoading = true;
                state.form.sourceAttributes = await $api.apps.getDsiSourceAttributes(state.form.dsi);
            } catch (error) {
                handleApiError(error, 'Unable to load source attributes');
            } finally {
                state.isLoading = false;
            }
        }
    }
);
watch(
    () => state.showModelCardDrawer,
    async (show) => {
        if(!show){
            return;
        }

        if(state.form.dsi === state.extension!.dsiType && state.extension?.chaiModelCardGroups){
            state.form.chaiModelCard = state.extension?.chaiModelCardGroups
        }
        else{
            try {
                state.isLoading = true;
                state.form.chaiModelCard = await $api.apps.getChaiModelCard();
            } catch (error) {
                handleApiError(error, 'Unable to load CHAI model card');
            } finally {
                state.isLoading = false;
            }
        }
    }
);

// Save latest Dsi Source Attributes
function onSaveFromDsiDrawer(sourceAttributes: SourceAttributeGroup[]) {
    if (!state.extension){
        return
    }
    state.showDsiDrawer = false;
    state.dsiCaptureStatus = `Changes Captured! Click below to save.`;
    state.extension.dsiType = state.form.dsi;
    state.extension.sourceAttributeGroups = sourceAttributes;
    state.form.sourceAttributes = undefined
}

// Save CHAI Model Card
function onSaveFromModelCardDrawer(chaiModelCard: ChaiModelCardGroup[]) {
    if (!state.extension){
        return
    }
    state.showModelCardDrawer = false;
    state.modelCardCaptureStatus = `Changes Captured! Click below to save.`;
    state.extension.chaiModelCardGroups = chaiModelCard;
    state.form.chaiModelCard = undefined
}

// Close Drawer
function onCancelFromDsiDrawer() {
    state.showDsiDrawer = false;
    state.showModelCardDrawer = false;
    state.form.dsi = state.extension!.dsiType;
    state.form.sourceAttributes = undefined;
    state.form.chaiModelCard = undefined;
}

loadApp();

</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="state.isLoading" />

    <!-- Drawer: Dsi source attribute editor -->
    <DsDrawer :show="state.showDsiDrawer || state.showModelCardDrawer" @close="onCancelFromDsiDrawer">
      <DsSourceAttributeModifier
          v-if="state.extension && state.form.sourceAttributes"
          title="Modify Source Attributes"
          :sub-title="state.extension.clientName"
          :source-attributes="state.form.sourceAttributes"
          :form-entry-disabled="false"
          description="View and modify source attributes about this application"
          @on-cancel="onCancelFromDsiDrawer"
          @on-save="onSaveFromDsiDrawer"
      />
      <ChaiModelCardForm
          v-if="state.extension && state.form.chaiModelCard && state.showModelCardDrawer"
          title="Modify CHAI Model Card"
          :sub-title="state.extension.clientName"
          :model-card-form="state.form.chaiModelCard"
          @on-cancel="onCancelFromDsiDrawer"
          @on-save="onSaveFromModelCardDrawer"
      />
    </DsDrawer>

    <DsViewer v-if="state.previewModelCard" @close="state.previewModelCard = false">
      <ChaiModelCard
          class="h-full overflow-auto m-5 space-y-5"
          :model-card-form="state.extension?.chaiModelCardGroups as ChaiModelCardGroup[]"
      />
    </DsViewer>

    <DsText size="2xl" weight="light">
      Manage Extension
    </DsText>
    <div class="pb-5" />

    <DsText size="sm" weight="light">
      View and modify details about this extension
    </DsText>
    <div class="pb-5" />

    <!--Extension Actions-->
    <div class="grid grid-cols-12 gap-8">
      <!-- Extension Details -->
      <div class="col-span-4">
        <DsLabeledText
            label="Extension Actions"
            text="Administrative actions for this extension"
        />
      </div>

      <!-- Actions -->
      <div class="col-span-8">
        <DsText size="sm">
          Actions
        </DsText>
        <div class="pb-2" />

        <DsButton :color="Colors.fire" :text-color='Colors.fire' variant="outline" @click="onDeleteApp">
          <DsIcon name='heroicons:x-mark' size='sm' />
          Delete Extension
        </DsButton>
        <div class="py-1" />
      </div>
    </div>
    <DsDivider />

    <DsForm ref="formRef">
      <div v-if="state.extension">
        <!-- Extension Details -->
        <div class="grid md:grid-cols-12 gap-8">
          <div class="col-span-4">
            <DsLabeledText
                label="Extension Details"
                text="Define the basic information about your extension that will be visible to others."
            />
          </div>

          <div class="col-span-8 space-y-5">
            <DsTextInput
                v-model="state.extension.clientName"
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

        <!-- Decision Support Intervention details -->
        <div class="grid md:grid-cols-12 gap-8">
          <div class="col-span-4">
            <DsLabeledText
                label="Decision Support Intervention (DSI)"
                text="Determine if this extension meets the DSI criteria as defined by (b)(11)." />
          </div>

          <div class="col-span-8 space-y-5">
            <DsLabeledInput label="Decision Support Intervention">
              <DsButtonGroup
                  v-model="state.form.dsi"
                  :active-color="Colors.secondary"
              >
                <DsButton :value="DsiOption.None">
                  None
                </DsButton>
                <DsButton :value="DsiOption.Predictive" @click="state.showDsiDrawer = true">
                  Predictive DSI
                </DsButton>
                <DsButton :value="DsiOption.EvidenceBased" @click="state.showDsiDrawer = true">
                  Evidence-Based DSI
                </DsButton>
              </DsButtonGroup>
              <!-- Success -->
              <div v-if="state.dsiCaptureStatus" class="flex space-x-1 items-center">
                <DsIcon name="heroicons:check-circle" size='sm' :color='Colors.primary' />
                <DsText size="sm">
                  {{ state.dsiCaptureStatus }}
                </DsText>
              </div>
            </DsLabeledInput>
          </div>

          <DsDivider class="col-span-12"/>
        </div>

        <!-- CHAI Model Card details -->
        <div class="grid md:grid-cols-12 gap-4">
          <div class="col-span-4">
            <DsLabeledText
                label="CHAI Model Card"
            >
              <DsText size="xs" weight="light">
                The CHAI model card build on the (b)(11) source attributes to further boost the transparency and trust of your application. <br>
                Add a CHAI Model Card and make it stand out from the rest. <br>
                You can find out more about <a href="https://chai.org/draft-chai-applied-model-card/" class="underline">CHAI and the model card here </a>
              </DsText>
            </DsLabeledText>
          </div>

          <div class="col-span-8 space-y-5">
            <DsLabeledInput label="CHAI Model Card">
              <div class="flex flex-row space-x-2">
                <DsButton :disabled="state.extension.dsiType !== 'Predictive'" size="md" :color="Colors.secondary" :text-color='Colors.onyx' @click="state.showModelCardDrawer = true">
                  <DsIcon :name='state.extension.chaiModelCardGroups ? "heroicons:pencil" : "heroicons:plus"' />
                  {{ state.extension.chaiModelCardGroups ? 'Modify Model Card' : 'Add Model Card' }}
                </DsButton>
                <DsButton v-if="state.extension.chaiModelCardGroups && state.extension.dsiType === 'Predictive'" size="md" @click.stop="state.previewModelCard = true">
                  Preview Model Card
                </DsButton>
              </div>
              <DsText v-if="state.extension.dsiType !== 'Predictive'" size="sm" weight="normal" :color="Colors.tangerine">
                * The CHAI Model Card is only available for apps with Predictive DSI Source attributes.
              </DsText>
            </DsLabeledInput>
            <!-- Success -->
            <div v-if="state.modelCardCaptureStatus" class="flex space-x-1 items-center">
              <DsIcon name="heroicons:check-circle" size='sm' :color='Colors.primary' />
              <DsText size="sm">
                {{ state.modelCardCaptureStatus }}
              </DsText>
            </div>
          </div>
          <DsDivider class="col-span-12"/>
        </div>

        <template v-if="state.extension.cdsAppType === CdsAppType.External">
          <div class="grid md:grid-cols-12 gap-8">
            <div class="col-span-4">
              <DsLabeledText
                label="CDS Hook Service URL"
                text="Specify the URL for the CDS Hook Service"
              />
            </div>

            <div class="col-span-8 space-y-5">
              <DsTextInput
                v-model="state.extension.cdsHookServiceUrl"
                type="text"
                label="CDS Hook Service URL"
                placeholder="https://mycoolapp.com/launch"
                required
                :rules="ValidationRules.url"
              />
            </div>
          </div>

          <DsDivider />
        </template>

        <template v-else-if="state.extension.cdsAppType === CdsAppType.HostedCustomCql && state.form.cdsHookApp">
          <CdsHookAppCqlEditorForm
            v-model="state.form.cdsHookApp"
            updating
          />

          <DsDivider />
        </template>

        <!-- Save/Cancel Buttons -->
        <div class="justify-start items-start gap-5 inline-flex">
          <DsButton :color="Colors.white" :text-color='Colors.gray' variant="subtle" @click="navigateTo(`/workspaces/${workspaceSlug}/workspace-extensions`)">
            Cancel
          </DsButton>
          <DsButton :color="Colors.primary" @click="updateApp">
            Save
          </DsButton>
        </div>
      </div>

      <!-- Error Messages -->
      <div v-if="!!state.errorMessage">
        <div class="p-4">
          <DsText size="sm" class="text-red">
            {{ state.errorMessage }}
          </DsText>
        </div>
      </div>
    </DsForm>
  </DsContainer>
</template>
