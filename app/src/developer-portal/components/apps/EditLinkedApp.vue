<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import type { INewLinkedApp } from '~/types/ui/apps/NewLinkedApp';
import type FhirApiProviderDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';

const props = defineProps<{
  modelValue: INewLinkedApp;
  show: boolean;
  fhirApiProviders: FhirApiProviderDto[];
  currentLinkedApps: INewLinkedApp[];
  organizationId: string;
}>();

const emit = defineEmits<{
  'close': []
  'save': [value: INewLinkedApp]
}>();

// "Save"...
function onSave() {
    // Validate all inputs...
    const isValid = formRef?.value?.validate() ?? false;
    if (!isValid) {
        return;
    }
    // Emit the save event...N
    emit('save', { ...linkedAppDetails.value });
}

const formRef = ref<FormRef>();
const linkedAppDetails = ref<INewLinkedApp>({ ...props.modelValue });

function useCredentials() {
    linkedAppDetails.value.isSharedCredentialType = false
}
</script>

<template>
  <DsModal :model-value="show" @close="$emit('close')">
    <DsModalProgressCard :total-steps='1' :current-step='1'>
      <div class="w-full">
        <!-- Lock -->
        <div class="flex w-full py-8 bg-space justify-center items-center">
          <div class="flex-1 flex items-center justify-center">
            <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center pt-2">
              <MeldRxLock />
            </div>
          </div>
        </div>
      </div>

      <!-- App Details -->
      <div class="py-4 px-8">
        <DsForm ref="formRef">
          <EhrAppForm
            v-model="linkedAppDetails"
            :fhir-api-providers="fhirApiProviders"
            :current-linked-apps="currentLinkedApps"
            :organization-id="organizationId"
          />
        </DsForm>
        <!-- Buttons -->
        <div
          v-if="linkedAppDetails.isSharedCredentialType"
          class="flex justify-center m-3.5"
        >
          <DsButton
            class="flex w-[220px] text-xs justify-center items-center border border-silver border-solid"
            variant="outline"
            :color="Colors.white"
            :text-color='Colors.gray'
            @click="useCredentials"
          >
            Use My Own Credentials
          </DsButton>
        </div>
          <DsDivider/>
          <div class="flex justify-center w-full gap-5">
            <DsButton id="cancel-edit-linked-app-button" :color="Colors.white" :text-color="Colors.gray" variant="subtle" @click="emit('close')">
              Cancel
            </DsButton>
            <DsButton id="save-linked-app-button" :color="Colors.secondary" @click="onSave">
              Save
            </DsButton>
          </div>
      </div>
    </DsModalProgressCard>
  </DsModal>
</template>
