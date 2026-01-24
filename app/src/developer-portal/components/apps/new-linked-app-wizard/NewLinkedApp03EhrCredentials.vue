<script setup lang="ts">
import type { INewLinkedApp } from '~/types/ui/apps/NewLinkedApp';
import { Colors } from '~/types/ui/colors';
import type FhirApiProviderDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';

const props = defineProps<{
  initialForm: INewLinkedApp;
  fhirApiProviders: FhirApiProviderDto[];
  currentLinkedApps: INewLinkedApp[];
  organizationId: string;
}>();

const localAppDetails = ref<INewLinkedApp>({ ...props.initialForm });

const emit = defineEmits<{
  'update:modelValue': [value: INewLinkedApp];
  'addLinkedApp': [appDetails: INewLinkedApp];
  'goBack': [];
  'close': [];
}>();

// "Add Linked App" button...
function onAddLinkedApp() {
    // Validate all inputs...
    if (!formRef.value) { return; }
    const isValid = formRef.value.validate();
    if (!isValid) { return; }

    // Emit the add linked app event...
    emit('addLinkedApp', localAppDetails.value);
}

const formRef = ref<FormRef>();

</script>

<template>
  <DsModalProgressCard :total-steps="3" :current-step="2">
    <!-- Lock -->
    <div class="flex w-full py-8 bg-space justify-center items-center">
      <div class="flex w-full items-center justify-center">
        <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center pt-2">
          <MeldRxLock />
        </div>
      </div>
      <div class="flex-1" />
    </div>
    <!-- App Details -->
    <div class="flex flex-col w-full py-4 px-8">
      <DsForm ref="formRef">
        <EhrAppForm
          v-model="localAppDetails"
          :fhir-api-providers="fhirApiProviders"
          :current-linked-apps="currentLinkedApps"
          :organization-id="organizationId"
        />
      </DsForm>

      <!-- Buttons -->
      <DsDivider />
      <div class="flex justify-center w-full gap-5 pb-4">
        <DsButton :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" @click="emit('goBack')">
          Previous Step
        </DsButton>
        <DsButton :color="Colors.secondary" variant='filled' @click='onAddLinkedApp'>
          Add Linked App
        </DsButton>
      </div>
    </div>
  </DsModalProgressCard>
</template>