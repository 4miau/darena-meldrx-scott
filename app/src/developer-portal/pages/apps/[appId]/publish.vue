<script setup lang="ts">
import {useRoute} from 'vue-router';
import type {
    PublishedAppDto,
    PublishedStatus
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/PublishedAppDto';
import type {SourceAttributeGroup} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/SourceAttribute';
import {DsiOption} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/DsiOption';
import {Colors} from '~/types/ui/colors';
import type {ChaiModelCardGroup} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/ChaiModelCardForm";
import type {IDsSingleSelectButtonListItem} from "~/types/ui/DsSingleSelectButtonList";


useHead({title: 'Publish App | MeldRx'});

const route = useRoute()
const {$api} = useNuxtApp()


const formRef = ref<FormRef>()
const appId = ref<string>(route.params.appId as string)

const state = reactive<{
  app?: PublishedAppDto,
  isLoading: boolean,
  dsiCaptureStatus: string,
  modelCardCaptureStatus: string,
  errorMessage: string,
  showDsiDrawer: boolean,
  showModelCardDrawer: boolean,
  previewModelCard: boolean,
  form: {
    dsi: DsiOption,
    sourceAttributes?: SourceAttributeGroup[]
    chaiModelCard?: ChaiModelCardGroup[]
  }
}>({
    isLoading: false,
    dsiCaptureStatus: '',
    modelCardCaptureStatus: '',
    errorMessage: '',
    showDsiDrawer: false,
    showModelCardDrawer: false,
    previewModelCard: false,
    form: {
        dsi: DsiOption.None,
    }
})

const pricingOptions: IDsSingleSelectButtonListItem<boolean>[] = [
    { value: true, title: 'Paid'},
    { value: false, title: 'Free'}
];

// Load current publish details of the app
async function loadApp() {
    state.isLoading = true;
    try {
        const originalApp = await $api.apps.getPublishedAppForEdit(appId.value);
        if (originalApp) {
            state.app = originalApp;
            state.form.dsi = state.app.dsiType;
            state.form.chaiModelCard = state.app.chaiModelCardGroups;
        }
    } catch (error) {
        handleApiError(error, 'Unable to load app details');
    } finally {
        state.isLoading = false;
    }
}

// When Save Clicked for App Publish related changes
async function onSave() {
    if (!formRef.value || !formRef.value) {
        return;
    }

    const isFormValid = formRef.value.validate();
    if (!isFormValid) {
        state.errorMessage = 'Please fix the errors above.';
        return;
    }
    if (!state.app) {
        return;
    }

    if (state.app.soFAppUserType == 'System') {
        state.errorMessage = 'Can\'t publish system type apps.';
        return;
    }
    try {
        state.isLoading = true;
        await $api.apps.updatePublishedApp(
            state.app.appId,
            {
                ...state.app,
                dsiType: state.form.dsi,
                sourceAttributeGroups: state.form.dsi === DsiOption.None ? undefined : state.app.sourceAttributeGroups,
                chaiModelCardGroups: state.app.chaiModelCardGroups
            }
        );
        notification({ title: 'Success', description: 'App settings saved successfully', displayTime: 3000, variant: 'success' });
    } catch (error) {
        handleApiError(error, 'Unable to save app details');
    } finally {
        state.isLoading = false;
    }
    navigateTo('/apps');
}

watch(
    () => state.form.dsi,
    async (value) => {
        if (value === DsiOption.None) {
            state.dsiCaptureStatus = '';
        }
    }
);

watch(
    () => state.showDsiDrawer,
    async (show) => {
        if (!show) {
            return;
        }

        if (state.form.dsi === state.app!.dsiType && state.app?.sourceAttributeGroups) {
            state.form.sourceAttributes = state.app?.sourceAttributeGroups
        } else if (state.form.dsi === DsiOption.Predictive || state.form.dsi === DsiOption.EvidenceBased) {
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
        if (!show) {
            return;
        }

        if (state.form.dsi === state.app!.dsiType && state.app?.chaiModelCardGroups) {
            state.form.chaiModelCard = state.app?.chaiModelCardGroups
        } else {
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
    if (!state.app) {
        return
    }
    state.showDsiDrawer = false;
    state.dsiCaptureStatus = `Changes Captured! Click below to save.`;
    state.app.dsiType = state.form.dsi;
    state.app.sourceAttributeGroups = sourceAttributes;
    state.form.sourceAttributes = undefined
}

// Save latest CHAI Model Card
function onSaveFromModelCardDrawer(chaiModelCard: ChaiModelCardGroup[]) {
    if (!state.app) {
        return
    }
    state.showModelCardDrawer = false;
    state.modelCardCaptureStatus = `Changes Captured! Click below to save.`;
    state.app.chaiModelCardGroups = chaiModelCard;
    state.form.chaiModelCard = undefined
}

// Close Drawer
function onCancelFromDrawer() {
    state.showDsiDrawer = false;
    state.showModelCardDrawer = false;
    state.form.dsi = state.app!.dsiType;
    state.form.sourceAttributes = undefined;
    state.form.chaiModelCard = undefined;
}

loadApp();
</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="state.isLoading"/>

    <!-- Drawer: Dsi source attribute editor -->
    <DsDrawer :show="state.showDsiDrawer || state.showModelCardDrawer" @close="onCancelFromDrawer">
      <DsSourceAttributeModifier
          v-if="state.app && state.form.sourceAttributes"
          title="Modify Source Attributes"
          :sub-title="state.app.appName"
          :source-attributes="state.form.sourceAttributes"
          :form-entry-disabled="false"
          description="View and modify source attributes about this application"
          @on-cancel="onCancelFromDrawer"
          @on-save="onSaveFromDsiDrawer"
      />
      <ChaiModelCardForm
          v-if="state.app && state.form.chaiModelCard && state.showModelCardDrawer"
          title="Modify CHAI Model Card"
          :sub-title="state.app.appName"
          :model-card-form="state.form.chaiModelCard"
          @on-cancel="onCancelFromDrawer"
          @on-save="onSaveFromModelCardDrawer"
      />
    </DsDrawer>

    <DsViewer v-if="state.previewModelCard" @close="state.previewModelCard = false">
      <ChaiModelCard
          class="h-full overflow-auto m-5 space-y-5"
          :model-card-form="state.app?.chaiModelCardGroups as ChaiModelCardGroup[]"
      />
    </DsViewer>

    <!-- Publish app editor -->
    <DsForm v-if="state.app" ref="formRef">
      <!-- Header Section -->
      <DsText size="2xl" weight="light">
        {{ state.app.appName }} - Publish App
      </DsText>
      <div class="pb-1"/>
      <DsText size="sm" weight="light">
        View and modify published details about this application
      </DsText>
      <DsDivider/>

      <!-- Publish App toggle -->
      <div class="grid md:grid-cols-12 gap-8">
        <div class="col-span-4">
          <DsLabeledText
              label="Publish Your App"
              text="Select to make your app available to all MeldRx Users."/>
        </div>

        <div class="col-span-8">
          <DsLabeledInput label="Publish Status">
            <DsButtonGroup
                :model-value="state.app.publishedStatus"
                :active-color="Colors.secondary"  
                :rounded="false"
                @update:model-value="state.app.publishedStatus = $event as PublishedStatus"
            >
              <DsButton value="NotPublished" :text-color='Colors.gray'>
                Not Published
              </DsButton>
              <DsButton value="Published" :text-color='Colors.gray'>
                Published
              </DsButton>
            </DsButtonGroup>
          </DsLabeledInput>
        </div>
      </div>
      <DsDivider class="my-8"/>

      <!-- App Details -->
      <div class="grid md:grid-cols-12 gap-8">
        <div class="col-span-4">
          <DsLabeledText
              label="App Details"
              text="Define the basic information about your application that will be visible to others."/>
        </div>

        <div class="col-span-8 space-y-5">
          <DsLabeledText
              label="App Name"
              :text="`${state.app.appName}`"
          />
          <DsTextArea
              v-model="state.app.description"
              type="text"
              label="App Description"
              placeholder="Description of your app"
              required
              :rules="[
                            [v => !!v, 'App Description is required'],
                            [v => v!.trim().length > 0, 'App Description is required'],
                            [v => v!.length <= 5000, 'App Name Description must be 5000 characters or less']
                        ]"/>
          
          <DsTextArea
              v-model="state.app.descriptionBrief"
              type="text"
              label="Brief Description"
              placeholder="Describe the app in 200 characters for the marketplace"
              required
              :rules="[
                            [v => !!v, 'Brief Description is required'],
                            [v => v!.trim().length > 0, 'Brief Description is required'],
                            [v => v!.length <= 300, 'Brief Description must be 300 characters or less']
                        ]"/>

          <DsTextInput
              v-model="state.app.publisherUrl"
              type="text"
              label="Publisher URL"
              placeholder="Publisher URL"
              required
              :rules="ValidationRules.url"/>

          <DsTextInput
              v-model="state.app.termsOfServiceUrl"
              type="text"
              label="Terms of Service"
              placeholder="URL to your Terms of Service"
              required
              :rules="ValidationRules.url"/>

          <DsTextInput
              v-model="state.app.privacyPolicyUrl"
              type="text"
              label="Privacy Policy"
              placeholder="URL to your Privacy Policy"
              required
              :rules="ValidationRules.url"/>

          <DsSingleSelectButtonList
              v-model="state.app.isPaid"
              :options="pricingOptions"
              label="Pricing"
              required
              tile
          />
          
          <DsLabeledInput label="Intended Users (select all that apply)">
            <DsButtonGroup
                v-model="state.app.intendedUsers"
                :active-color="Colors.secondary"
                :active-text-color='Colors.white'
                multiple
                :rounded="false"
            >
              <DsButton value="Clinicians" size="sm">
                Clinicians
              </DsButton>
              <DsButton value="Researchers" size="sm">
                Researchers
              </DsButton>
              <DsButton value="Clinical Trials" size="sm">
                Clinical Trials
              </DsButton>
            </DsButtonGroup>
          </DsLabeledInput>
          
          <DsTextInput
              v-model="state.app.logoUrl"
              type="text"
              label="Logo URL"
              placeholder="URL For Your Logo"
              :required="false"
              :disabled="false"
              :rules="ValidationRules.url"/>
        </div>
      </div>
      <DsDivider class="my-8"/>

      <!-- Decision Support Intervention details -->
      <div class="grid md:grid-cols-12 gap-8">
        <div class="col-span-4">
          <DsLabeledText
              label="Decision Support Intervention (DSI)"
              text="Determine if this app meets the DSI criteria as defined by (b)(11)."/>
        </div>

            <div class="col-span-8 space-y-5">
              <DsLabeledInput label="Decision Support Intervention">
                <DsButtonGroup
                    v-model="state.form.dsi"
                    :active-color="Colors.secondary"
                >
                  <DsButton :value="DsiOption.None" :text-color='Colors.gray'>
                    None
                  </DsButton>
                  <DsButton :value="DsiOption.Predictive" :text-color='Colors.gray' @click="state.showDsiDrawer = true">
                    Predictive DSI
                  </DsButton>
                  <DsButton :value="DsiOption.EvidenceBased" :text-color='Colors.gray' @click="state.showDsiDrawer = true">
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
          </div>
          <DsDivider/>

          <!-- CHAI Model Card details -->
          <div class="grid md:grid-cols-12 gap-8">
            <div class="col-span-4">
              <DsLabeledText
                  label="CHAI Model Card"
              >
                <DsText size="xs" weight="light">
                  The CHAI model card builds on the (b)(11) source attributes to further boost the transparency and trust of your application. <br>
                  Add a CHAI Model Card and make it stand out from the rest. <br>
                  You can find out more about <a href="https://chai.org/draft-chai-applied-model-card/" class="underline">CHAI and the model card here </a>
                </DsText>
              </DsLabeledText>
            </div>

            <div class="col-span-8 space-y-5">
              <DsLabeledInput label="CHAI Model Card">
                <div class="flex flex-row space-x-2">
                  <DsButton :disabled="state.app.dsiType !== 'Predictive'" size="md" :color="Colors.secondary" @click="state.showModelCardDrawer = true">
                    <DsIcon :name="state.app.chaiModelCardGroups ? 'heroicons:pencil' : 'heroicons:plus'" />
                    {{ state.app.chaiModelCardGroups ? 'Modify Model Card' : 'Add Model Card' }}
                  </DsButton>
                  <DsButton v-if="state.app.chaiModelCardGroups && state.app.dsiType === 'Predictive'" size="md" @click.stop="state.previewModelCard = true">
                    Preview Model Card
                  </DsButton>
                </div>
                <DsText v-if="state.app.dsiType !== 'Predictive'" size="sm" weight="normal" :color="Colors.tangerine">
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

      <!-- Cancel & Save Action Buttons -->
      <div class="justify-start items-start gap-5 inline-flex">
        <DsButton :color="Colors.white" :text-color='Colors.gray' variant="subtle" @click="navigateTo('/apps')">
          Cancel
        </DsButton>
        <DsButton :color="Colors.primary" @click="onSave">
          Save
        </DsButton>
      </div>
      <div v-if="!!state.errorMessage">
        <div class="p-4">
          <DsText size="sm" :color="Colors.fire">
            {{ state.errorMessage }}
          </DsText>
        </div>
      </div>

    </DsForm>
  </DsContainer>
</template>
