<script setup lang="ts">
import type {Bundle, Group, Patient} from 'fhir/r4';
import {getDefaultPatientDto, type PatientDto} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PatientDto';
import PatientUtils from '~/utils/PatientUtils';
import {Colors} from '~/types/ui/colors';
import {
    type CreateInviteDto,
    type InviteDto,
    InviteType,
    type SendInviteDto
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Invites';
import {SelectActions} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/SelectAction';
import ResourceType from '~/types/fhir/ResourceType'
import type {UploadDocumentTypes} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/UploadDocumentTypes';
import type {UploadDocumentDto} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/UploadDocumentDto';

definePageMeta({ layout: 'workspace', middleware: ['require-workspace'] });
useHead({ title: 'Patients | MeldRx' });
const { $api } = useNuxtApp();
const route = useRoute();


const confirmation = useConfirmation()

const {workspace} = useWorkspace()
const workspaceSlug = ref<string>(route.params.workspaceSlug as string);
const addEditPatientMode = ref<'add' | 'edit'>('add');
const currentPatient = ref<Patient | null>(null);
const isSynapseInvite = ref<boolean>(false);
const searchPatientEntry = ref<string>('');
const searchPatientResultEmpty = ref<boolean>(false);

const initialBundle: Bundle = {
    resourceType: 'Bundle',
    type: 'searchset',
    entry: []
};

const state = reactive<{
  loading: boolean;
  patients: Patient[];
  allGroups: Group[];
  invites: InviteDto[];
  bundle: Bundle;
  token: string | null
  modals: {
      openInVirtualWorkspace: boolean
  }
}>({
    loading: false,
    patients: [],
    allGroups: [],
    invites: [],
    bundle: initialBundle,
    token: '',
    modals: {
        openInVirtualWorkspace: false
    }
});

const allowedFileImports = ref<UploadDocumentTypes[]>([
    { fileExtension: ['.xml'], fileDescription: 'CCDA (.xml)' },
    { fileExtension: ['.json'], fileDescription: 'FHIR R4 Bundle (.json)' }
]);

const allowedDocumentUploads = ref<UploadDocumentTypes[]>([
    { fileExtension: ['.jpg','.png','.jpeg'], fileDescription: 'Profile Pictures' },
    { fileExtension: ['.pdf', '.json', '.xml', '.txt', '.csv', '.doc', '.docx', '.bmp', '.tif', '.rtf', '.mp4', '.mpg', '.avi'], fileDescription: 'Clinical Documents' }
]);

async function loadPatientsForWorkspace(workspaceSlug: string, searchValue?: string, page: number = -1) {
    state.loading = true;

    try {
        let bundlePatients;
        if (state.token && page > 0) {
            bundlePatients = await $api.fhir.searchPatientsByPage(workspaceSlug, page, state.token);
        } else if (searchValue) {
            bundlePatients = await $api.fhir.searchPatientsByName(workspaceSlug, searchValue);
        } else {
            bundlePatients = await $api.fhir.searchPatients(workspaceSlug);
        }

        if (!bundlePatients) {
            state.loading = false;
            return [];
        }

        if (!bundlePatients.entry && searchValue) {
            state.loading = false;
            return [];
        }
        state.bundle = bundlePatients

        const nextLink = bundlePatients.link?.find(link => link.relation === 'next');
        if (nextLink) {
            const url = new URL(nextLink.url);
            state.token = url.searchParams.get('token');
        }

        // Filter entries to just the patients...
        const patients: Patient[] = FHIRUtils.filterBundleByType<Patient>(bundlePatients, ResourceType.Patient);
        await loadPatientInvites(patients);
        state.patients = patients;
    } catch (error) {
        handleApiError(error)
    } finally {
        state.loading = false;
    }
}
function handleGoToPage(page: number) {
    loadPatientsForWorkspace(workspaceSlug.value, searchPatientEntry.value, page);
}

// Add patient...
const openAddPatientModal = ref(false);
const openAddToGroupModal = ref(false);
const patientDto = ref<PatientDto>(getDefaultPatientDto());
const openSendInviteModal = ref(false);
const openImportDataModal = ref<boolean>(false);
const openUploadDocumentModal = ref<boolean>(false);
const email = ref('')

const onActionSelected = (action: SelectActions, patient: Patient) => {
    if (action === SelectActions.addToGroup) {
        onAddPatientToGroupMenuClick(patient)
    }
    else if (action === SelectActions.edit) {
        onEditPatientMenuClick(patient)
    }
    else if (action === SelectActions.openInVirtualWorkspace) {
        onOpenInVirtualWorkspace(patient)
    }
    else if (action === SelectActions.delete) {
        return onDeletePatient(patient.id!)
    }
    else if (action === SelectActions.sendRequest) {
        onSendInviteModal(true, patient)
    }
    else if (action === SelectActions.sendInvite) {
        onSendInviteModal(false, patient)
    }
    else if (action === SelectActions.copyInvite) {
        onCopyInviteURL(patient)
    }
    else if (action === SelectActions.revokeInvite) {
        return onRevokeInvite(patient.id!)
    }
    else if (action === SelectActions.uploadDocument) {
        onUploadDocuments(patient)
    }
}
async function onAddPatient(patientDto: PatientDto) {
    state.loading = true;
    try {
        const fhirPatient = PatientUtils.patientDtoToPatient(patientDto);
        await $api.fhir.createPatient(workspaceSlug.value, fhirPatient);
        openAddPatientModal.value = false;
        await loadPatientsForWorkspace(workspaceSlug.value);
        notification({
            title: 'Success',
            description: 'The patient has been added.',
            displayTime: 3000,
            variant: 'success'
        });
    } catch (error) {
        handleApiError(error, 'Unable to create patient')
    } finally {
        state.loading = false;
    }
}

async function onEditPatient(patientDto: PatientDto) {
    state.loading = true;

    try {
        const fhirPatient = PatientUtils.patientDtoToPatient(patientDto, state.patients.find(x => x.id === patientDto.id));
        await $api.fhir.updatePatient(workspaceSlug.value, fhirPatient);
        openAddPatientModal.value = false;
        await loadPatientsForWorkspace(workspaceSlug.value);
        notification({
            title: 'Success',
            description: 'The patient has been edited.',
            displayTime: 3000,
            variant: 'success'
        });
    } catch (error) {
        handleApiError(error, 'Unable to edit patient')
    } finally {
        state.loading = false;
    }
}

async function onDeletePatient(patientId: string) {
    const {isCancelled} = await confirmation("Are you sure you want to delete this patient? This action cannot be undone.", "Delete Patient")
    if(isCancelled){
        return;
    }

    // Delete the patient...
    state.loading = true;
    try {
        await $api.fhir.deletePatient(workspaceSlug.value, patientId);
        notification({
            title: 'Success',
            description: 'The patient has been deleted.',
            displayTime: 3000,
            variant: 'success'
        });
    } catch(error){
        handleApiError(error, "Unable to delete patient")
    }
    state.loading = false;

    // Update the local model...
    await loadPatientsForWorkspace(workspaceSlug.value);
}

// Find all invites for displayed patients
const loadPatientInvites = async (patients: Patient[]) => {
    if (patients.length === 0) {
        state.invites = []
        return;
    }
    state.loading = true;
    state.invites = await $api.invites.search(workspaceSlug.value, patients.map(x => x.id as string))
    state.loading = false;
}

const onUploadDocuments = async(patient: Patient) => {
    openUploadDocumentModal.value = true;
    currentPatient.value = patient;
}

// Send Invite...
const onSendInviteModal = function(isSynapse: boolean, patient: Patient) {
    openSendInviteModal.value = true;

    isSynapseInvite.value = isSynapse
    currentPatient.value = patient;
    email.value = patient.telecom?.find(x => x.system === 'email')?.value as string
}

const onSendInvite = async () => {
    const fhirPatient = state.patients.find(x => x.id === currentPatient.value?.id)

    if (!fhirPatient) {
        return
    }
    const invId = state.invites.find(x => x.accessiblePatientId === fhirPatient.id)
    const sendInvite: SendInviteDto = {
        email: email.value
    }

    if (!invId) {
        const inviteDto:CreateInviteDto =
        {
            inviteType: InviteType.Organization,
            accessiblePatientId: fhirPatient?.id as string,
            userEmail: email.value,
            isSynapseRole: isSynapseInvite.value
        };

        await $api.invites.create(workspaceSlug.value, inviteDto)
        openSendInviteModal.value = false;
        await loadPatientInvites(state.patients)
        notification({
            title: 'Success',
            description: `The patient ${isSynapseInvite.value ? 'request' : 'invite'} has been sent.`,
            displayTime: 3000,
            variant: 'success'
        });
        return;
    }

    await $api.invites.send(workspaceSlug.value, invId.id, sendInvite)
    notification({
        title: 'Success',
        description: 'The patient invite has been sent.',
        displayTime: 3000,
        variant: 'success'
    });
    openSendInviteModal.value = false;
    await loadPatientInvites(state.patients)
}

async function onRevokeInvite(patientId: string) {
    const { isCancelled } = await confirmation('Are you sure you want to revoke this invitation?', 'Revoke Invitation')
    if (isCancelled) {
        return
    }

    const findInv = state.invites.find(x => x.accessiblePatientId === patientId)
    if (!findInv) {
        return
    }

    try {
        await $api.invites.delete(workspaceSlug.value, findInv.id)
        notification({
            title: 'Success',
            description: 'The patient invite has been revoked.',
            displayTime: 3000,
            variant: 'success'
        });
    } catch (error) {
        handleApiError(error, 'Unable to revoke invite')
    }

    await loadPatientInvites(state.patients)
}

const onCopyInviteURL = async (patient: Patient) => {
    const findInv = state.invites.find(x => x.accessiblePatientId === patient.id)

    if (!findInv) {
        const fhirPatient = state.patients.find(x => x.id === patient.id)

        const inviteDto:CreateInviteDto =
      {
          inviteType: InviteType.Organization,
          accessiblePatientId: fhirPatient?.id as string
      };

        const invite = await $api.invites.create(workspaceSlug.value, inviteDto)

        await loadPatientInvites(state.patients)

        copyToClipboard(window.location.origin + '/invite/code/' + invite.code).toString()

        notification({
            title: 'Copied',
            description: 'Created Invite and Copied URL to clipboard.',
            displayTime: 3000,
            variant: 'success'
        });
    } else {
        copyToClipboard(window.location.origin + '/invite/code/' + findInv.invitationCode).toString()
        notification({
            title: 'Copied',
            description: 'Copied Invite URL to clipboard.',
            displayTime: 3000,
            variant: 'success'
        });
    }
}

// "Add Patient button..."
const onAddPatientModalClick = function() {
    addEditPatientMode.value = 'add';
    patientDto.value = { ...getDefaultPatientDto() }; // Reset model so we start with a blank form...
    openAddPatientModal.value = true;
}

// "Edit Patient" button from the drop-down...
const onEditPatientMenuClick = function(patient: Patient) {
    addEditPatientMode.value = 'edit';
    patientDto.value = PatientUtils.patientToPatientDto(patient);
    openAddPatientModal.value = true;
}

// "Open in Virtual Workspace" button from the drop-down...
function onOpenInVirtualWorkspace(patient: Patient) {
    state.modals.openInVirtualWorkspace = true
    patientDto.value = PatientUtils.patientToPatientDto(patient);
}

// "Add to Group" button from the drop-down...
const onAddPatientToGroupMenuClick = async function(patient: Patient) {
    state.loading = true;

    // Cache the patient we are editing...
    currentPatient.value = patient;

    // Load all groups...
    const bundleGroups = await $api.fhir.getGroups(workspaceSlug.value);
    const groups = FHIRUtils.filterBundleByType<Group>(bundleGroups, ResourceType.Group);
    if (groups.length === 0) {
        notification({ title: 'No Groups', description: 'Create a group to start adding patients.', displayTime: 3000 });
        state.loading = false;
        return;
    }

    // Filter out all groups patient is already a member of. If no groups remain, show a message and exit...
    const filteredGroups = groups.filter((group: Group) => !FHIRUtils.isPatientInGroup(group, patient.id ?? ''));
    if (filteredGroups.length === 0) {
        notification({ title: 'No Available Groups', description: 'This patient is already a member of all available groups.', displayTime: 3000 });
        state.loading = false;
        return;
    }

    // Set groups and show modal...
    state.allGroups = filteredGroups;
    state.loading = false;
    openAddToGroupModal.value = true;
}

// "Add to Group" from the Modal...
const onAddPatientToGroup = async function(group?: Group) {
    // Close modal...
    openAddToGroupModal.value = false;

    // If either group or patient is not set, return...
    if (!group) { return; }
    if (!currentPatient.value) { return; }
    if (!currentPatient.value.id) { return; }

    // Add patient to the group...
    state.loading = true;
    const newGroup = FHIRUtils.addPatientToGroup(group, currentPatient.value.id);
    await $api.fhir.updateGroup(workspaceSlug.value, newGroup.id ?? '', newGroup);
    notification({
        title: 'Success',
        description: `Patient has been added to group ${group.name}.`,
        displayTime: 3000,
        variant: 'success'
    });

    state.loading = false;
}
// "Import Data" button clicked...
function onImportDataButtonClick() {
    openImportDataModal.value = true;
}
// After the user submits the modal...
async function onImportData(value: { fileContents: string, fileExtension: string }) {
    state.loading = true;
    try {
        openImportDataModal.value = false;
        const { fileContents, fileExtension } = value;

        if (fileExtension === 'xml') {
            await $api.dataImport.importCcda(workspaceSlug.value, fileContents);
        } else if (fileExtension === 'json') {
            try {
                const bundle = FHIRUtils.createBundleFromFileContents(fileContents);
                await $api.fhir.uploadBundle(workspaceSlug.value, bundle);
            }catch (error){
                handleApiError(error, 'Invalid JSON Bundle')
                return;
            }
        }
        else {
            notification({ title: 'Error', description: `Unsupported file extension: ${fileExtension}`, displayTime: 3000, variant: 'error' });
            return;
        }

        notification({ title: 'Success', description: 'Data imported successfully', displayTime: 3000, variant: 'success' });
    } catch (error) {
        handleApiError(error, 'Unable to import')
    } finally {
        state.loading = false;
    }
}

async function onUploadDocument(value: { fileContents: ArrayBuffer, fileDate:string, fileDescription: string, fileName: string }) {
    state.loading = true;
    try {
        openUploadDocumentModal.value = false;
        const { fileContents, fileDate, fileDescription, fileName  } = value;
        const uploadDoc: UploadDocumentDto = {
            date: fileDate,
            description: fileDescription,
            patientId: currentPatient.value?.id
        };
        await $api.documents.uploadDocument(workspaceSlug.value, uploadDoc,fileContents, fileName);
        notification({ title: 'Success', description: 'Data uploaded successfully', displayTime: 3000, variant: 'success' });
    } catch (error) {
        handleApiError(error, 'Unable to upload patient document')
    } finally {
        state.loading = false;
    }
}

loadPatientsForWorkspace(workspaceSlug.value);
</script>

<template>
  <DsContainer v-if="workspace">
    <DsLoadingOverlay :loading="state.loading" />

    <!-- Add Patient Modal -->
    <AddPatientModal
      v-model="patientDto"
      :show="openAddPatientModal"
      :send-patient-data-request="false"
      :mode="addEditPatientMode"
      :is-loading="state.loading"
      @close="() => openAddPatientModal = false"
      @add-patient="onAddPatient"
      @edit-patient="onEditPatient"
    />

    <!-- Add To Group Modal -->
    <AddToGroupModal
      :show="openAddToGroupModal"
      :groups="state.allGroups"
      @add-to-group="onAddPatientToGroup($event)"
      @close="() => openAddToGroupModal = false"
      @cancel="() => openAddToGroupModal = false"
    />

    <SendInviteModal
      v-model="email"
      :show="openSendInviteModal"
      @close="() => openSendInviteModal = false"
      @cancel="() => openSendInviteModal = false"
      @send-invite="() => onSendInvite()"
    />

    <OpenInVirtualWorkspaceModal
      v-model="state.modals.openInVirtualWorkspace"
      v-model:loading="state.loading"
      :workspace-slug="workspaceSlug"
      :patient-id="patientDto.id ?? ''"
    />

    <!-- Import Data Modal -->
    <ImportDataModal
      id="patients-import-data-modal"
      :show="openImportDataModal"
      title="Import Data"
      description="Import data into this workspace. Supported types include:"
      :allowed-import-types = allowedFileImports
      @close="() => openImportDataModal = false"
      @cancel="() => openImportDataModal = false"
      @import-data="onImportData"
    />

    <!-- Upload DDOcument Modal -->
    <ImportDataModal
      :show="openUploadDocumentModal"
      show-description-date
      title="Upload Documents"
      description="Upload patient document like clinical notes/reports. Supported types include:"
      :allowed-import-types = allowedDocumentUploads
      read-mode="Binary"
      @close="() => openUploadDocumentModal = false"
      @cancel="() => openUploadDocumentModal = false"
      @import-data-with-description-date="onUploadDocument"
    />

    <div>
      <!-- Header Text -->
      <DsText size="2xl" weight="light">
        Patients
      </DsText>
      <div class="pb-5" />
      <!-- Description -->
      <DsText size="sm" weight="light">
        View details about the patients in your workspace.
      </DsText>
    </div>
    <div class="pb-5" />

    <!-- No Patients -->
    <div v-if="state.patients.length === 0 && searchPatientResultEmpty === false">
      <div class="flex flex-col items-center justify-center h-full py-20 border-2 border-silver space-y-5">
        <DsText size="2xl" weight="light">
          No Patients
        </DsText>
        <DsText size="sm" weight="light">
          There are no patients in this workspace. Click the button below to add a patient.
        </DsText>
        <div class="flex space-x-5">
          <!-- Import Data Button -->
          <DsButton variant="filled" @click="() => onImportDataButtonClick()">
            <DsIcon name="heroicons:plus" />
            Import Data
          </DsButton>

          <!-- Add Patient -->
          <DsButton :color="Colors.secondary" variant="filled" @click="onAddPatientModalClick">
            Add Patient
          </DsButton>
        </div>
      </div>
    </div>

    <div v-else class="space-y-5">
      <!-- Search Patients -->
      <div class="flex w-full">
        <div class="w-full">
          <DsTextInput
            v-model="searchPatientEntry"
            placeholder="Search Patients by Last name"
            @enter-pressed="() => loadPatientsForWorkspace(workspaceSlug, searchPatientEntry)"
          >
            <template #right>
              <DsButton :color="Colors.white" :text-color="Colors.gray" variant="outline" @click="() => loadPatientsForWorkspace(workspaceSlug, searchPatientEntry)">
                Search
                <DsIcon name='heroicons:magnifying-glass' />
              </DsButton>
            </template>
          </DsTextInput>
        </div>
      </div>
      <div class="flex space-x-5">
        <!-- Import Data Button -->
        <DsButton variant="filled" @click="() => onImportDataButtonClick()">
          <DsIcon name="heroicons:plus" />
          Import Data
        </DsButton>

        <!-- Add Patient -->
        <DsButton :color="Colors.secondary" variant="filled" @click="onAddPatientModalClick">
          Add Patient
        </DsButton>
      </div>

      <!-- Patients -->
      <PatientTable
        v-if="state.patients.length > 0"
        :workspace="workspace"
        :patients="state.patients"
        :invites="state.invites"
        :bundle="state.bundle"
        @action="(action: SelectActions, patient: Patient) => onActionSelected(action, patient)"
        @go-to-page="handleGoToPage"
      />
    </div>
  </DsContainer>
</template>
