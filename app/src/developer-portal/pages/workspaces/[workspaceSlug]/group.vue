<script setup lang="ts">
import type { Patient, Group } from 'fhir/r4';
import { Colors } from '~/types/ui/colors';
import ResourceType from '~/types/fhir/ResourceType'

definePageMeta({ layout: 'workspace', middleware: ['require-workspace'] });
useHead({ title: 'Manage Group | MeldRx' });

const { $api } = useNuxtApp()
const route = useRoute();
const confirmation = useConfirmation();

const isLoading = ref<boolean>(false);
const workspaceSlug = ref<string>(route.params.workspaceSlug as string);
const groupId = ref<string>(route.query.id as string);

interface IState {
  group: Group | null,
  patients: Patient[];
}

const state = useState<IState>('patients', () => ({
    group: null,
    patients: []
}));

const tableHeaders = ['', 'Patient Name', 'Sex', 'Age', 'Date of Birth', 'Actions'];

// Loads the current group...
async function loadGroup(groupId: string): Promise<Group | null> {
    isLoading.value = true;
    const group = await $api.fhir.getGroupById(workspaceSlug.value, groupId);
    if (!group) { isLoading.value = false; return null; }
    isLoading.value = false;

    return group;
}

// Load all patients for the current workspae...
async function loadPatientsForGroup(workspaceSlug: string, group: Group | null): Promise<Patient[]> {
    if (!group) { return []; }

    isLoading.value = true;

    // Retrieve patients...
    const patientIds: string[] = FHIRUtils.getPatientIdsFromGroup(group);
    if (patientIds.length === 0) {
        isLoading.value = false;
        return []
    }

    const bundlePatients = await $api.fhir.searchPatientsByIds(workspaceSlug, patientIds);
    const patients: Patient[] = FHIRUtils.filterBundleByType<Patient>(bundlePatients, ResourceType.Patient);

    isLoading.value = false;
    return patients as any;
}

// Delete Patient...
async function onRemovePatientFromGroup(patient: Patient){
    const {isCancelled} = await confirmation("Are you sure you want to remove this patient from this group?", "Remove Patient from Group")
    if(isCancelled){
        return;
    }

    if (!state.value.group) { return; }
    if (!state.value.group.member) { return; }

    // Remove this patient from the group...
    state.value.group.member = state.value.group.member.filter((member) => member.entity?.reference !== `Patient/${patient.id}`);
    if (state.value.group.member.length === 0) {
        delete state.value.group.member;
    }

    // Update the group...
    isLoading.value = true;
    try {
        await $api.fhir.updateGroup(workspaceSlug.value, groupId.value, state.value.group);
    }
    catch (error) {
        handleApiError(error, 'Unable to remove patient from group');
    }
    isLoading.value = false;

    // Update the local model...
    state.value.group = await loadGroup(groupId.value);
    state.value.patients = await loadPatientsForGroup(workspaceSlug.value, group);
}

// "Go to Patients button..."
async function onGoToPatientsClick() {
    await navigateTo(`/workspaces/${workspaceSlug.value}/patients`);
}

const group = await loadGroup(groupId.value);
state.value.group = group;
const patients = await loadPatientsForGroup(workspaceSlug.value, group);
state.value.patients = patients;

</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="isLoading" />

    <div>
      <DsText size="2xl" weight="light" class="block">
        Manage Group
      </DsText>
      <DsText size="sm" weight="light">
        {{ (state.group) ? state.group.name : '' }}
      </DsText>
      <div class="pb-5" />
      <DsText size="md" weight="light">
        Manage the people for this group.
      </DsText>
    </div>
    <div class="pb-5" />
    <!-- No Patients -->
    <div v-if="state.patients.length === 0" class="space-y-5">
      <div class="flex flex-col items-center justify-center h-full py-20 border-2 border-silver space-y-5">
        <DsText size="2xl" weight="light">
          This Group is Empty
        </DsText>
        <DsText size="sm" weight="light">
          There are no people in this group.<!-- Click the button below to add a patient. -->
        </DsText>
        <div class="flex space-x-5">
          <DsButton :color="Colors.secondary" variant="filled" @click="onGoToPatientsClick">
            Go to Patients
          </DsButton>
        </div>
        <!-- <DsButton title="Add Patient" variant="filled" @click="onAddPatientModalClick" /> -->
      </div>
    </div>

    <div v-else class="space-y-5">
      <!-- Add Patient -->
      <!-- <DsButton title="Add Patient" variant="filled" @click="onAddPatientModalClick" /> -->

      <!-- Patients -->
      <DsTable
        v-if="state.patients"
        :headers="tableHeaders"
        :items="state.patients"
        :id-selector="item => item.id!"
      >
        <template #default="{item}">
          <DsAvatar
            :alt="PatientUtils.formatName(item)[0].toUpperCase()"
            size="sm"
            class="bg-bliss border border-silver text-silver"
          />

          <DsText size="md" weight="light">
            {{ PatientUtils.formatName(item) }}
          </DsText>

          <DsText size="md" weight="light" class="capitalize">
            {{ PatientUtils.formatSex(item) }}
          </DsText>

          <DsText size="md" weight="light">
            {{ PatientUtils.formatAge(item) }}
          </DsText>

          <DsText size="md" weight="light">
            {{ PatientUtils.formatDateOfBirth(item) }}
          </DsText>

          <div class="px-2">
            <!-- Remove Button -->
            <DsButton :color="Colors.fire" :text-color="Colors.fire" variant="outline" @click="onRemovePatientFromGroup(item)">
              <DsIcon name="heroicons:x-mark"/>
              Remove
            </DsButton>
          </div>
        </template>
      </DsTable>
    </div>
  </DsContainer>
</template>
