<script setup lang="ts">
import type DynamicRegistrationDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/DynamicRegistrationDto';
import { CdsAppType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/CdsAppType';
import { Colors } from '~/types/ui/colors';
import type { UpdateCdsHooksAppCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/UpdateCdsHooksAppCommand';

const { $api } = useNuxtApp();


const props = defineProps<{
  app: DynamicRegistrationDto
}>();

defineEmits<{
  'cancel': [];
}>();

const state = reactive<{
  isLoading: boolean;
  isLoadingCdsHookDetails: boolean;
  form: UpdateCdsHooksAppCommand;
}>({
    isLoading: false,
    isLoadingCdsHookDetails: false,
    form: {
        id: props.app.client_id,
        name: props.app.client_name,
        cdsHookServiceUrl: props.app.cdsHookServiceUrl,
        cqlEditorArtifact: props.app.cqlEditorArtifact ? JSON.parse(props.app.cqlEditorArtifact) : null
    }
});

async function save () {
    if (state.isLoading) {
        return;
    }

    state.isLoading = true;
    try {
        const formData = new FormData();
        formData.append('Id', props.app.client_id);
        formData.append('Name', state.form.name!);

        if (state.form.cdsHookServiceUrl) {
            formData.append('CdsHookServiceUrl', state.form.cdsHookServiceUrl);
        }

        if (state.form.cdsHook) {
            formData.append('CdsHook.Title', state.form.cdsHook.title);
            formData.append('CdsHook.Hook', state.form.cdsHook.hook);
            formData.append('CdsHook.Description', state.form.cdsHook.description);
            formData.append('CdsHook.UsageRequirements', state.form.cdsHook.usageRequirements);
        }

        state.form.cards?.forEach((card, index) => {
            formData.append(`Cards[${index}].Condition`, card.condition);
            formData.append(`Cards[${index}].Detail`, card.detail);
            formData.append(`Cards[${index}].Indicator`, card.indicator);
            formData.append(`Cards[${index}].Summary`, card.summary);
        });

        if (state.form.cqlEditorArtifact) {
            formData.append('CqlEditorArtifact', JSON.stringify(state.form.cqlEditorArtifact));
        }

        await $api.apps.updateCdsHookApp(formData);

        navigateTo('/apps');
    } catch (error) {
        handleApiError(error, 'Unable to save app');
    } finally {
        state.isLoading = false;
    }
}

async function loadCdsHookDetails () {
    state.isLoadingCdsHookDetails = true
    try {
        const { hook, cards } = await $api.apps.getCdsHookDetails(props.app.client_id);
        state.form.cdsHook = hook;
        state.form.cards = cards;
    } catch (error) {
        handleApiError(error, 'Failed to load cds hook details');
    } finally {
        state.isLoadingCdsHookDetails = false
    }
}

if(props.app.cdsAppType === CdsAppType.Hosted || props.app.cdsAppType === CdsAppType.HostedCustomCql){
    loadCdsHookDetails();
}

</script>

<template>
  <DsForm ref="formRef">
    <div class="space-y-5">
      <!-- App Details -->
      <div class="grid md:grid-cols-12 gap-8">
        <div class="col-span-4">
          <DsLabeledText
            label="App Details"
            text="Define the basic information about your application that will be visible to others."
          />
        </div>

        <div class="col-span-8 space-y-5">
          <DsLabeledText v-if="!!app.client_id" label="App ID / Client ID">
            <DsTextWithCopyButton
              id="client-id"
              size="xs"
              :text="app.client_id"
              :text-to-copy="app.client_id"
              show-toast-on-copy
              toast-message-on-copy="Copied App ID / Client ID to clipboard."
            />
          </DsLabeledText>

          <DsTextInput
            v-model="state.form.name"
            type="text"
            label="App Name"
            required
            :rules="ValidationRules.appName"
          />
        </div>
      </div>

      <DsDivider />
      <!-- User Information -->
      <div class="grid md:grid-cols-12 gap-8">
        <div class="col-span-4">
          <DsLabeledText
            label="User Information"
            text="The primary user base of this app / the app type."
          />
        </div>
        <div class="col-span-8">
          <DsLabeledText
            label="App Type"
            :text="app.soFAppUserType"
          />
        </div>
      </div>

      <template v-if="app.cdsAppType === CdsAppType.External">

        <DsDivider />

        <div class="grid md:grid-cols-12 gap-8">
          <div class="col-span-4">
            <DsLabeledText
              label="CDS Hook Service URL"
              text="Specify the URL for the CDS Hook Service"
            />
          </div>

          <div class="col-span-8 space-y-5">
            <DsTextInput
              v-model="state.form.cdsHookServiceUrl"
              type="text"
              label="CDS Hook Service URL"
              placeholder="https://mycoolapp.com/launch"
              required
              :rules="ValidationRules.url"
            />
          </div>
        </div>

      </template>
      <template v-else-if="props.app.cdsAppType === CdsAppType.HostedCustomCql">

        <DsDivider />

        <DsLoadingSpinner v-if="state.isLoadingCdsHookDetails" loading />
        <CdsHookAppCqlEditorForm
          v-else
          v-model="state.form"
          updating
        />
      </template>

      <DsDivider />

      <!-- Save/Cancel Buttons -->
      <div class="justify-start items-start gap-5 inline-flex">
        <DsButton id='cancel-button' :color="Colors.white" :text-color='Colors.gray' variant="subtle" @click="$emit('cancel')">
          Cancel
        </DsButton>
        <DsButton id='save-button' :color="Colors.primary" @click="save">
          Save
        </DsButton>
      </div>
    </div>
  </DsForm>
</template>

