<script setup lang="ts">
import type FhirApiProviderDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';
import type { IDsSelectItem } from '~/types/ui/DsSelect';
import type { EHRs } from '~/types/ehrs';
import EpicLogo from '~/components/ehrs/logos/EpicLogo.vue';
import CernerLogo from '~/components/ehrs/logos/CernerLogo.vue';
import NextGenLogo from '~/components/ehrs/logos/NextGenLogo.vue';
import AthenaHealthLogo from '~/components/ehrs/logos/AthenaHealthLogo.vue';
import VeradigmLogo from '~/components/ehrs/logos/VeradigmLogo.vue';
import OtherEhrLogo from '~/components/ehrs/logos/OtherEhrLogo.vue';
import FhirApiProviderUtils from '~/utils/FhirApiProviderUtils';
import type { INewLinkedApp } from '~/types/ui/apps/NewLinkedApp';

type OptionType = IDsSelectItem<string> & {
  // search paramters
  fhirProviderId: string;
  organizationName: string;
  productName: string;
}

const props = defineProps<{
    fhirApiProviderMeldRxIdentifier?: string;
    rules?: ValidationRule<string>[];
    includeEhrQuickButtons?: boolean;
    includeLinkToFhirProvider?: boolean;
    currentLinkedApps: INewLinkedApp[];
}>();

const emit = defineEmits<{
  'update:fhirApiProviderMeldRxIdentifier': [value: string];
  'update:fhirProviderId': [value: string];
}>();

const fhirApiProviders: Ref<FhirApiProviderDto[]> = useFhirApiProviders();
const currentOrg = ref<string>();
const currentEhr = ref<EHRs>(EHRUtils.getEhrFromFhirApiProviderMeldRxId(props.fhirApiProviderMeldRxIdentifier));

// EHRs for the quick buttons...
const ehrOptions : {ehr: EHRs, component: any}[] = [
    { ehr: 'Epic', component: EpicLogo },
    { ehr: 'Cerner', component: CernerLogo },
    { ehr: 'NextGen', component: NextGenLogo },
    { ehr: 'AthenaHealth', component: AthenaHealthLogo },
    { ehr: 'Veradigm', component: VeradigmLogo },
    { ehr: 'Other', component: OtherEhrLogo }
];

// map and cache the initial fhirApiProviders to options
const fhirApiProviderOptions = computed<OptionType[]>(() => {
    if (!fhirApiProviders) { return []; }
    return fhirApiProviders.value.map(x => ({
        fhirProviderId: x.id,
        organizationName: x.organizationName.toLocaleLowerCase(),
        productName: x.productName.toLocaleLowerCase(),
        value: x.meldRxIdentifier,
        title: FhirApiProviderUtils.getDisplayString(x)
    }))
});

const filteredFhirApiProviderOptions = computed(() => {
    // Don't allow user to select any of the FHIR Providers that were already selected by another linked app...
    const alreadyUsedFhirProviderIds: string[] = props.currentLinkedApps
        .map(x => x.fhirApiProviderMeldRxIdentifier ?? '')
        .filter(x => x !== props.fhirApiProviderMeldRxIdentifier);

    // Filter out already-used FHIR Providers...
    const filtered = fhirApiProviderOptions.value.filter(x => !alreadyUsedFhirProviderIds.includes(x.value));

    // Do further filtering based on organization, if needed...
    if (!currentOrg.value) { return filtered; }
    return filtered.filter(x => x.organizationName === currentOrg.value);
});

const fhirProviderId = computed(() => {
    return findFhirProviderId(props.fhirApiProviderMeldRxIdentifier);
});

// Given a FHIR Provider MeldRx ID, returns the ID (GUID) of the FHIR Provider...
function findFhirProviderId(fhirApiProviderMeldRxId?: string): string | undefined {
    if (!fhirApiProviderMeldRxId) { return undefined; }
    const fhirApiProvider = fhirApiProviderOptions.value.find(x => x.value === fhirApiProviderMeldRxId);
    return fhirApiProvider?.fhirProviderId;
}

// Handle clicking an EHR button...
function onEhrClick(ehr: EHRs) {
    currentEhr.value = ehr
    const fhirApiProviderId = EHRUtils.getFhirApiProviderMeldRxIdForEhr(ehr)
    const fhirApiProvider = fhirApiProviderOptions.value.find(x => x.value === fhirApiProviderId)
    if (!fhirApiProvider) {
        currentOrg.value = '';
        emit('update:fhirApiProviderMeldRxIdentifier', '');
        emit('update:fhirProviderId', '');
    } else {
        currentOrg.value = fhirApiProvider.organizationName;
        emit('update:fhirApiProviderMeldRxIdentifier', fhirApiProvider.value);
        emit('update:fhirProviderId', fhirApiProvider.fhirProviderId);
    }
}

// Occurs when FHIR Provider is changed in drop-down...
function onFhirProviderChange(value: string) {
    emit('update:fhirApiProviderMeldRxIdentifier', value);

    const fhirProviderId = findFhirProviderId(value) ?? '';
    emit('update:fhirProviderId', fhirProviderId);
}
</script>

<template>
  <div class="flex flex-col">
    <!-- EHR Buttons -->
    <DsLabeledInput v-if="includeEhrQuickButtons" label="FHIR API Provider" required>
      <template #popoverarea>
        <DsIcon name="heroicons:information-circle" size="sm" />
      </template>
      <template #popovercontent>
        <DsText size="sm">
          Providers that are listed on the Certified Health IT Product List (CHPL)<br>
          that have been certified by the ONC Health IT Certification program.
        </DsText>
      </template>
      <div class="grid grid-cols-3 gap-2">
        <button
          v-for="o in ehrOptions"
          :id="o.ehr.toLowerCase() + '-button'"
          :key="o.ehr"
          class="col-span-1 h-20 px-2 bg-white hover:bg-bliss shadow border border-silver justify-center items-center flex"
          :class="{'shadow-primary': (currentEhr === o.ehr)}"
          @click="onEhrClick(o.ehr)
          "
        >
          <component :is="o.component" class="p-2 object-contain max-w-full max-h-full" />
        </button>
      </div>
    </DsLabeledInput>
    <div v-if="includeEhrQuickButtons" class="pb-2" />

    <!-- FHIR Provider Dropdown -->
    <DsSelect
      :required="true"
      :model-value="fhirApiProviderMeldRxIdentifier"
      :items="filteredFhirApiProviderOptions"
      label="Select a FHIR API Provider"
      placeholder="Select FHIR API Provider"
      search-placeholder="Search FHIR API Providers"
      searchable
      :rules="[[(v) => !!v, 'FHIR API Provider is required']]"
      @update:model-value="onFhirProviderChange($event)"
    />

    <!-- More Info -->
    <div class="self-end p-2">
      <DsText v-if="includeLinkToFhirProvider && !!fhirProviderId" size="sm" class="pt-2">
        <DsLink
          :href="`/tools/fhir-providers?id=${fhirProviderId}`"
          target="_blank"
          underine="hover"
        >
          More Details...
        </DsLink>
      </DsText>
    </div>
  </div>
</template>
