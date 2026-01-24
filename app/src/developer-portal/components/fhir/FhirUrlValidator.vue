<script setup lang="ts">
import type SmartConfigurationResponse from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Fhir/SmartOnFhir/SmartConfigurationResponse';
import { Colors } from "~/types/ui/colors";
type ValidationStatus = 'none' | 'success' | 'fail';
const { $api } = useNuxtApp();

const {
    debounce: debounceValidation,
    debouncing: debouncingValidation
} = useDebounce(validateFhirUrl, 500);

const props = defineProps<{
    required?: boolean;
    fhirUrl?: string;
}>();

const emit = defineEmits<{
  'update:fhirUrl': [fhirUrl: string];
}>();

const isLoading = ref<boolean>(false);
const validationStatus = ref<ValidationStatus>('none');
const fhirUrlBuffer = ref(props.fhirUrl);

watch(()=> props.fhirUrl, (v) => {
    if (fhirUrlBuffer.value !== v){
        fhirUrlBuffer.value = v
        validateFhirUrl(v)
    }
})

// Add DsForm validation support...
const { dirty, validationResult } = useValidation(
    () => validationStatus.value,
    () => [
        [v => v !== 'none', 'Please enter a valid FHIR API URL.'],
        [v => v !== 'fail', 'Please enter a valid FHIR API URL.'],
        [_ => !debouncingValidation.value, ''],
    ],
    () => props.required ?? false
);

function updateFhirUrlBuffer(value?: string) {
    dirty.value = true;
    fhirUrlBuffer.value = value ?? ''
    emit('update:fhirUrl', fhirUrlBuffer.value)
    debounceValidation(value)
}

async function validateFhirUrl(fhirUrl? : string) {
    if(!fhirUrl){
        validationStatus.value = 'none'
        return;
    }

    if (isLoading.value) {
        return
    }
    isLoading.value = true;

    try {
        const smartConfig: SmartConfigurationResponse = await $api.fhirProviders.validate(fhirUrl)
        validationStatus.value = smartConfig.isSuccess ? 'success' : 'fail'
    } catch (error) {
        handleApiError(error, 'FHIR Url is invalid.');
        validationStatus.value = 'fail';
    } finally {
        isLoading.value = false;
    }
}

validateFhirUrl(props.fhirUrl)
</script>

<template>
  <div>
    <DsTextInput
      :model-value="fhirUrlBuffer"
      label="FHIR API URL"
      :required="required"
      class="pb-2"
      @update:model-value="updateFhirUrlBuffer"
    />

    <div v-if="!debouncingValidation" class="flex flex-row items-center">
      <!-- Loading -->
      <div v-if="isLoading" class="flex">
        <DsLoadingSpinner :loading="true" />
      </div>

      <!-- Success -->
      <div v-if="(!isLoading) && (validationStatus == 'success')" class="flex items-center">
        <DsIcon name="heroicons:check-circle" size='sm' :color='Colors.primary'/>
        <div class="px-1" />
        <DsText size="sm">
          Validated successfully!
        </DsText>
      </div>

      <!-- Failure -->
      <div v-if="(!isLoading) && ((validationStatus == 'fail') || (!validationResult.isValid))" class="flex items-center">
        <DsIcon name="heroicons:exclamation-circle" size='sm' :color='Colors.fire' />
        <div class="px-1" />
        <DsText id="fhir-url-error" size="sm" :color="Colors.fire">
          {{ validationResult.error ?? 'Failed to validate FHIR API URL' }}
        </DsText>
      </div>
    </div>
  </div>
</template>
