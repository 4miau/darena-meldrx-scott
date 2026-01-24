<script setup lang="ts">
import { formatDistanceToNowStrict } from 'date-fns';
import type { SynapsePackageDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/SynapsePackageDto';

definePageMeta({ layout: 'workspace', middleware: ['require-workspace'] });
useHead({ title: 'Received Patient Data | MeldRx' });
const { $api } = useNuxtApp();

const route = useRoute();
const confirmation = useConfirmation();

const isLoading = ref<boolean>(false);
const workspaceSlug = ref<string>(route.params.workspaceSlug as string);
const tableHeaders = ['Description', 'Date Received', 'Total Entries', 'Actions'];
const actions = [
    { value: 'Import', title: 'Import' },
    { value: 'Delete', title: 'Delete' }
];
const linkedActions = [
    { value: 'Create Patient And Import', title: 'Create Patient And Import' },
    { value: 'Delete', title: 'Delete' }
];

interface IState {
  synapsePackages: SynapsePackageDto[];
}

const state = useState<IState>('synapsePackages', () => ({
    synapsePackages: []
}));

// Delete Synapse Package..
async function onDeleteSynapsePackage(packageId: string) {
    const {isCancelled} = await confirmation("Are you sure you want to delete this package? This action cannot be undone.", "Delete Package")
    if(isCancelled){
        return;
    }

    isLoading.value = true;
    try {
        await $api.synapse.deletePackage(workspaceSlug.value, packageId);
    } catch (error) {
        handleApiError(error, 'Unable to delete package');
    }

    isLoading.value = false;
    // Update the local model...
    state.value.synapsePackages = await loadPackagesForWorkspace(workspaceSlug.value);
}
const onImportSynapsePackage = async (synapsePackage: SynapsePackageDto) => {
    isLoading.value = true;
    try {
        await $api.synapse.importPackage(workspaceSlug.value, {
            id: synapsePackage.id,
        });
    } catch (error) {
        handleApiError(error, 'Unable to import package');
    }

    isLoading.value = false;

    // Update the local model...
    state.value.synapsePackages = await loadPackagesForWorkspace(workspaceSlug.value);
}

// Load all packages for the current workspae...
async function loadPackagesForWorkspace(workspaceSlug: string): Promise<SynapsePackageDto[]> {
    isLoading.value = true;

    // Retrieve patients...
    const packagesList = await $api.synapse.getPackages(workspaceSlug);
    if (!packagesList) { isLoading.value = false; return []; }
    isLoading.value = false;
    return packagesList;
}

function formatDate(value:string):string {
    const date = new Date(value + 'Z');
    return formatDistanceToNowStrict(date, { addSuffix: true });
}

function onActionSelected(value: string, synapsePackage: SynapsePackageDto) {
    if (value === 'Import') {
        onImportSynapsePackage(synapsePackage);
    } else if (value === 'Create Patient And Import') {
        onImportSynapsePackage(synapsePackage);
    } else if (value === 'Delete') {
        return onDeleteSynapsePackage(synapsePackage.id)
    }
}

function actionsList(linkedPatientId:string) {
    if (linkedPatientId) {
        return actions
    }
    return linkedActions
}

const packages = await loadPackagesForWorkspace(workspaceSlug.value);
state.value.synapsePackages = packages;

</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="isLoading" />

    <div>
      <DsText size="2xl" weight="light" class="block">
        Received Patient Data
      </DsText>
      <div class="pb-5" />
      <DsText size="sm" weight="light">
        View the latest patient data received.
      </DsText>
    </div>
    <div class="pb-5" />

    <!-- No Patients -->
    <div v-if="state.synapsePackages.length === 0">
      <div class="flex flex-col items-center justify-center h-full py-20 border-2 border-silver space-y-5">
        <DsText size="2xl" weight="light">
          No Packages
        </DsText>
        <DsText size="sm" weight="light">
          There are no packages avaialable. You must first send a request to a patient.
        </DsText>
      </div>
    </div>

    <div v-else>
      <!-- Synapse Table -->
      <DsTable
        v-if="state.synapsePackages"
        :headers="tableHeaders"
        :items="state.synapsePackages"
        :id-selector="item => item.id"
      >
        <template #default="{item}">
          <DsText size="md" weight="light">
            {{ item.description }}
          </DsText>

          <DsText size="md" weight="light">
            {{ formatDate(item.createdAt) }}
          </DsText>

          <DsText size="md" weight="light">
            {{ item.downloadInfo.downloadTotal }}
          </DsText>

          <div v-if="item.downloadInfo.wasDownloaded">
            <DsDropdown
              :options="actionsList(item.linkedPatientId)"
              label="Actions"
              @select="(v) => { onActionSelected(v,item) }"
            />
          </div>
        </template>
      </DsTable>
    </div>
  </DsContainer>
</template>
