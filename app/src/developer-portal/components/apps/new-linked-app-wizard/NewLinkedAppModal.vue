<script setup lang="ts">
import type { EHRs } from '~/types/ehrs';
import type FhirApiProviderDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';
import type { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod';
import type { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';
import type { INewLinkedApp } from '~/types/ui/apps/NewLinkedApp';
import type SharedEhrCredentialView from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SharedEhrCredentialDto';
const { $api } = useNuxtApp()
type PageType = 'ehr-select' | 'select-credential-mode' | 'linked-app-details' | 'summary';

const props = defineProps<{
    show: boolean;
    authMethod: SoFAppTokenAuthMethod;
    userType: SoFAppUserType;
    linkedApps: INewLinkedApp[];
    fhirApiProviders: FhirApiProviderDto[];
    currentLinkedApps: INewLinkedApp[];
    organizationId: string;
}>();

const emit = defineEmits<{
    'addLinkedApp': [appDetails?: INewLinkedApp];
    'close': [];
}>();

const currentPage = ref<PageType>('ehr-select');
const sharedCredentials = ref<SharedEhrCredentialView []>();
const isLoading = ref<boolean>(false);
const fhirProviderId = ref<string>();

// Loads the MeldRx Shared Credentials from backend and looks for a matching credential
async function loadSharedEhrCredentials(): Promise<void> {
    isLoading.value = true;
    sharedCredentials.value = await $api.apps.listSharedCredentials();
    isLoading.value = false;
}

onBeforeMount(async () => {
    await loadSharedEhrCredentials();
})

// Determine if we have Shared Credential for selected ehr
const sharedCredential = computed(() => {
    const matchingCredentials = sharedCredentials.value?.filter(x =>
        linkedAppForm.value.fhirApiProviderMeldRxIdentifier &&
        x.chplId.includes(linkedAppForm.value.fhirApiProviderMeldRxIdentifier) &&
        x.soFAppUserType === linkedAppForm.value.soFAppUserType
    );
    // Should there be a check to see if more than one credential is found.????
    if (matchingCredentials && matchingCredentials.length > 0) {
        return matchingCredentials[0];
    }
    return undefined;
})

function newLinkedApp(ehr: EHRs, fhirApiProviderMeldRxIdentifier: string) {
    const app : INewLinkedApp = {
        ehr,
        soFAppUserType: props.userType,
        soFAppTokenAuthMethod: props.authMethod,
        clientId: '',
        fhirApiProviderMeldRxIdentifier,
        clientName: '',
        scopes: '',
        secretType: undefined,
        isSharedCredentialType: undefined
    };

    return app;
}

const linkedAppForm = ref<INewLinkedApp>(newLinkedApp('Epic', EHRUtils.getFhirApiProviderMeldRxIdForEhr('Epic')));

// From Step 1, when user selects an pre-configured EHR...
function ehrSelected (ehr: EHRs) {
    linkedAppForm.value = newLinkedApp(ehr, EHRUtils.getFhirApiProviderMeldRxIdForEhr(ehr));
    openCredentialModeSelect();
}

// From Step 1, when user selects an 'Other' EHR...
function otherEhrSelected(fhirApiProviderMeldRxIdentifier: string) {
    linkedAppForm.value = newLinkedApp(EHRUtils.getEhrFromFhirApiProviderMeldRxId(fhirApiProviderMeldRxIdentifier), fhirApiProviderMeldRxIdentifier);
    openCredentialModeSelect();
}
function onUpdateFhirProviderId(newFhirProviderId: string) {
    fhirProviderId.value = newFhirProviderId;
}
// From Step 2, when user select client-credentials mode...
function selectOwnCredentials () {
    linkedAppForm.value.isSharedCredentialType = false
    currentPage.value = 'linked-app-details';
}

// From Step 2, when user select shared-credentials mode...
function selectSampleCredentials () {
    linkedAppForm.value.isSharedCredentialType = true
    currentPage.value = 'linked-app-details';
}

// From Step 3, when user clicks "Add Linked App"...
function addLinkedApp (linkedApp: INewLinkedApp) {
    currentPage.value = 'summary';
    emit('addLinkedApp', linkedApp);

}

// From Step 3, when user clicks "Previous" or Step 1, after selecting an EHR
function openCredentialModeSelect () {
    currentPage.value = 'select-credential-mode'
}

// From Step 4, when user clicks "Add Another Linked App" or going back from Step 2...
function openEhrSelect () {
    currentPage.value = 'ehr-select';
}

// From Step 5, when user clicks "Save and Close"...
function saveAndClose () {
    emit('close')
}

// Whenever "show" changes to true, switch back to the ehr-select page. Otherwise, it might reappear on the 3rd page
watch(() => props.show, () => {
    if (props.show) { currentPage.value = 'ehr-select'; }
});

</script>

<template>
  <DsModal :model-value='show' @close="$emit('close')">
    <DsLoadingOverlay :loading="isLoading" />
    <!-- Page 1 (Select an EHR) -->
    <NewLinkedApp01EhrSelect
      v-if="currentPage === 'ehr-select'"
      :current-linked-apps="linkedApps"
      @ehr-selected="ehrSelected"
      @other-selected="otherEhrSelected"
      @fhir-provider-id="onUpdateFhirProviderId"
      @close="saveAndClose"
    />

    <!-- Page 2 (Credentials Choice) -->
    <NewLinkedApp02SelectCredentialMode
      v-if="currentPage === 'select-credential-mode'"
      :current-linked-apps="currentLinkedApps"
      :is-shared-credential-found="!!sharedCredential"
      @select-own-credentials="selectOwnCredentials"
      @select-sample-credentials="selectSampleCredentials"
      @go-back="openEhrSelect"
      @close="saveAndClose"
    />

    <!-- Page 3 (EHR Credentials) -->
    <NewLinkedApp03EhrCredentials
      v-if="currentPage === 'linked-app-details'"
      :initial-form="linkedAppForm"
      :fhir-api-providers="fhirApiProviders"
      :current-linked-apps="currentLinkedApps"
      :organization-id="organizationId"
      @add-linked-app="addLinkedApp"
      @go-back="openCredentialModeSelect"
      @close="saveAndClose"
    />

    <!-- Page 4 (Linked App Added) -->
    <NewLinkedApp04LinkedAppAdded
      v-if="currentPage === 'summary'"
      @close="saveAndClose"
      @add-another-linked-app="openEhrSelect"
    />
  </DsModal>
</template>
~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SharedEhrCredentialDto