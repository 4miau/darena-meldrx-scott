<script setup lang="ts">
import type { EHRs } from '~/types/ehrs';
import type { INewLinkedApp } from '~/types/ui/apps/NewLinkedApp';

const props = defineProps<{
  currentLinkedApps: INewLinkedApp[];
}>();

defineEmits<{
  'ehrSelected': [ehr: EHRs]
  'otherSelected': [value: string];
  'fhirProviderId': [value: string];
  'close': [];
}>();

const ehr: Ref<EHRs> = ref('Epic');
const hasEpic = computed(() => { return props.currentLinkedApps.some(x => x.ehr === 'Epic') });
const hasCerner = computed(() => { return props.currentLinkedApps.some(x => x.ehr === 'Cerner') });
const hasNextGen = computed(() => { return props.currentLinkedApps.some(x => x.ehr === 'NextGen') });
const hasAthena = computed(() => { return props.currentLinkedApps.some(x => x.ehr === 'AthenaHealth') });
const hasVeradigm = computed(() => { return props.currentLinkedApps.some(x => x.ehr === 'Veradigm') });
const showOtherEhrSelector = ref<boolean>(false);
const fhirApiProviderMeldRxIdentifier = ref<string>();
</script>

<template>
  <DsModalProgressCard :total-steps="3" :current-step="1">
    <div class="w-full">
      <!-- Cloud -->
      <div class="w-full py-8 bg-secondary justify-center items-center inline-flex">
        <MeldRxCloud background="#095E86" />
      </div>

      <!-- Description -->
      <div class="pt-4 w-full flex-col justify-center items-center gap-5 flex">
        <DsText size="2xl" weight="light" class="text-center">
          Choose a Linked App Provider
        </DsText>
        <DsText size="sm" weight="light" class="text-center">
          Select the external system that you would like to connect with.
        </DsText>
      </div>

      <!-- EHR Buttons -->
      <div class="p-4">
        <div class="grid grid-cols-3 gap-4">
          <button
            id="epic-button"
            :disabled="hasEpic"
            :class="hasEpic ? 'opacity-25' : ''"
            class="w-full h-20 px-2 bg-white shadow border border-silver hover:bg-bliss justify-center items-center flex"
            @click="ehr = 'Epic'; $emit('ehrSelected', 'Epic')"
          >
            <EpicLogo class="p-2 object-contain max-w-full max-h-full" />
          </button>

          <button
            id="cerner-button"
            :disabled="hasCerner"
            :class="hasCerner ? 'opacity-25' : ''"
            class="w-full h-20 px-2 bg-white shadow border border-silver hover:bg-bliss justify-center items-center flex"
            @click="ehr = 'Cerner'; $emit('ehrSelected', 'Cerner')"
          >
            <CernerLogo class="p-2 object-contain max-w-full max-h-full" />
          </button>

          <button
            id="nextgen-button"
            :disabled="hasNextGen"
            :class="hasNextGen ? 'opacity-25' : ''"
            class="w-full h-20 px-2 bg-white shadow border border-silver hover:bg-bliss justify-center items-center flex"
            @click="ehr = 'NextGen'; $emit('ehrSelected', 'NextGen')"
          >
            <NextGenLogo class="p-2 object-contain max-w-full max-h-full" />
          </button>

          <button
            id="athena-button"
            :disabled="hasAthena"
            :class="hasAthena ? 'opacity-25' : ''"
            class="w-full h-20 px-2 bg-white shadow border border-silver hover:bg-bliss justify-center items-center flex"
            @click="ehr = 'AthenaHealth'; $emit('ehrSelected', 'AthenaHealth')"
          >
            <AthenaHealthLogo class="p-2 object-contain max-w-full max-h-full" />
          </button>

          <button
            id="veradigm-button"
            :disabled="hasVeradigm"
            :class="hasVeradigm ? 'opacity-25' : ''"
            class="w-full h-20 px-2 bg-white shadow border border-silver hover:bg-bliss justify-center items-center flex"
            @click="ehr = 'Veradigm'; $emit('ehrSelected', 'Veradigm')"
          >
            <VeradigmLogo class="p-2 object-contain max-w-full max-h-full" />
          </button>

          <button
            id="other-button"
            class="w-full h-20 px-2 shadow border border-silver hover:bg-bliss justify-center items-center flex"
            :class="[ showOtherEhrSelector ? 'selected bg-dsgray' : 'bg-white' ]"
            @click="showOtherEhrSelector = true"
          >
            <OtherEhrLogo />
          </button>
        </div>
        <div v-if="showOtherEhrSelector">
          <DsDivider class="my-1" />
          <FhirProviderSelect
            :fhir-api-provider-meld-rx-identifier="fhirApiProviderMeldRxIdentifier"
            :include-link-to-fhir-provider="true"
            :current-linked-apps="currentLinkedApps"
            @update:fhir-api-provider-meld-rx-identifier="$emit('otherSelected', $event)"
            @update:fhir-provider-id="$emit('fhirProviderId', $event)"
          />
        </div>
      </div>
    </div>
  </DsModalProgressCard>
</template>
