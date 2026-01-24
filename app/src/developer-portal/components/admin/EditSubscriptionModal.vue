<script setup lang="ts">
import type SubscriptionDto from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/SubscriptionDto";
import { Colors } from "~/types/ui/colors";
import { MeldRxSubscriptionConfig } from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/MeldRxSubscription";

const props = defineProps<{
  subscriptionDto: SubscriptionDto;
  showModal: boolean;
}>();

const emit = defineEmits<{
  'close': [];
}>();

const { $api } = useNuxtApp()


const formRef = ref<FormRef>();
const state = reactive<{
  isLoading: boolean,
  form: SubscriptionDto
}>({
    isLoading: false,
    form: {
        id: props.subscriptionDto.id,
        organizationName: props.subscriptionDto.organizationName,
        resellerId: props.subscriptionDto.resellerId,
        status: props.subscriptionDto.status,
        subscriptionType: props.subscriptionDto.subscriptionType,
        includedWorkspaces: props.subscriptionDto.includedWorkspaces,
        includedVirtualWorkspaces: props.subscriptionDto.includedVirtualWorkspaces,
        includedApiCalls: props.subscriptionDto.includedApiCalls,
        includedProviders: props.subscriptionDto.includedProviders,
        includedDataStorage: props.subscriptionDto.includedDataStorage,
        allowOverage: props.subscriptionDto.allowOverage,
        allowLiteWorkspaces: props.subscriptionDto.allowLiteWorkspaces,
        allowNpiOverride: props.subscriptionDto.allowNpiOverride,
        validateCcdaProvider: props.subscriptionDto.validateCcdaProvider,
        autoAddProvider: props.subscriptionDto.autoAddProvider,
        allowPopulationTrigger: props.subscriptionDto.allowPopulationTrigger,
    }
})

async function onUpdate() {
    if (!formRef.value?.validate() || state.isLoading) {
        return;
    }

    state.isLoading = true

    try {
        await $api.admin.subscriptions.update(props.subscriptionDto.id, state.form)
    } catch (error) {
        handleApiError(error, 'Unable to update subscription')
    }
    state.isLoading = false

    emit('close')
}

watch(() => props.subscriptionDto, (v) => {
    state.form = {
        id: v.id,
        organizationName: v.organizationName,
        resellerId: v.resellerId,
        status: v.status,
        subscriptionType: v.subscriptionType,
        includedWorkspaces: v.includedWorkspaces,
        includedVirtualWorkspaces: v.includedVirtualWorkspaces,
        includedApiCalls: v.includedApiCalls,
        includedProviders: v.includedProviders,
        allowOverage: v.allowOverage,
        allowLiteWorkspaces: v.allowLiteWorkspaces,
        includedDataStorage: v.includedDataStorage,
        allowNpiOverride: v.allowNpiOverride,
        validateCcdaProvider: v.validateCcdaProvider,
        autoAddProvider: v.autoAddProvider,
        allowPopulationTrigger: v.allowPopulationTrigger,
    }
})
</script>

<template>
  <DsModal :model-value="showModal" @close="$emit('close')">
    <DsModalProgressCard :total-steps='1' :current-step='1'>
        <div class="flex w-full py-6 bg-space justify-center items-center">
        <div class="flex w-full items-center justify-center">
          <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center">
            <MeldRxApp />
          </div>
        </div>
        <div class="flex-1" />
      </div>
      <DsLoadingOverlay :loading="state.isLoading" />
      <DsForm ref="formRef">
        <div class="flex w-full py-4 px-8">
          <div class="flex flex-col w-full">
            <div class="w-full flex-col justify-center items-center gap-5 flex">
              <DsText size="2xl" weight="light">
                Edit Subscription - {{ state.form.organizationName }}
              </DsText>
              <div class="w-full grid grid-cols-1 gap-4">
                <DsTextInput
                  v-model="state.form.includedWorkspaces"
                  required
                  type="number"
                  label="Included Workspaces"
                />
                <DsTextInput
                  v-model="state.form.includedVirtualWorkspaces"
                  required
                  type="number"
                  label="Included Virtual Workspaces"
                />
                <DsTextInput
                  v-model="state.form.includedApiCalls"
                  required
                  type="number"
                  label="Included API Calls"
                />
                <DsTextInput
                  v-model="state.form.includedProviders"
                  required
                  type="number"
                  label="Included Providers"
                />
                <DsTextInput
                  v-model="state.form.includedDataStorage"
                  required
                  type="number"
                  label="Included Data Storage"
                />
                <DsSelect
                  v-model="state.form.subscriptionType"
                  label="Subscription Type"
                  :items="MeldRxSubscriptionConfig"
                  required
                />
                <DsText size="md" weight="light">
                  Allow Overage
                  <DsToggle
                    id="toggle-overage-button"
                    v-model="state.form.allowOverage"
                  />
                </DsText>
                <DsText size="md" weight="light">
                  Allow Lite Workspaces
                  <DsToggle
                      id="toggle-lite-button"
                      v-model="state.form.allowLiteWorkspaces"
                  />
                </DsText>
                <DsText size="md" weight="light">
                  Allow NPI Override
                  <DsToggle
                      id="toggle-npi-override-button"
                      v-model="state.form.allowNpiOverride"
                  />
                </DsText>
                <DsText size="md" weight="light">
                  Validate CCDA Providers
                  <DsToggle
                      id="toggle-validate-ccda-providers-button"
                      v-model="state.form.validateCcdaProvider"
                  />
                </DsText>
                <DsText size="md" weight="light">
                  Auto Add Providers
                  <DsToggle
                      id="toggle-auto-add-providers-button"
                      v-model="state.form.autoAddProvider"
                  />
                </DsText>
                <DsText size="md" weight="light">
                  Allow Population Trigger
                  <DsToggle
                      id="toggle-allow-population-trigger-button"
                      v-model="state.form.allowPopulationTrigger"
                  />
                </DsText>
              </div>
              <DsDivider />
              <div class="flex justify-center w-full gap-5">
                <DsButton :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" @click="$emit('close')">
                    Cancel
                </DsButton>
                <DsButton :color="Colors.secondary" @click="onUpdate">
                    Save
                </DsButton>
              </div>
            </div>
          </div>
        </div>
      </DsForm>
    </DsModalProgressCard>
  </DsModal>
</template>
