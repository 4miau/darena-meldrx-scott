<script setup lang="ts">
import {
    InviteType,
    type CreateInviteDto,
    type InviteCreateResponseDto,
    type InviteDto,
    type SendInviteDto
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Invites';
import type {PatientDto} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PatientDto';
import type {Patient, Group, Bundle} from 'fhir/r4';
import ResourceType from "~/types/fhir/ResourceType";
import {InviteStatus} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/InviteStatus";
import {Colors} from "~/types/ui/colors";


const {$api} = useNuxtApp()

const confirmation = useConfirmation();

const props = defineProps<{
  workspaceSlug: string;
  patientId: string;
}>();

const emit = defineEmits<{
  'reloadPatientData': [];
}>();

const state = reactive<{
  isLoading: boolean;
  inviteDto?: InviteDto;
  inviteStatus: string;
  inviteCreateResponse?: InviteCreateResponseDto;
  inviteEmail: string;
  openSendInviteModal: boolean;
  isSynapseInvite: boolean;
  newGroupName: { name: string };
  allGroupResources: Group[];
  openAddToGroupModal: boolean;
  openAddGroupModal: boolean;
  groupModalMode: 'create' | 'edit';
  groupMembershipTableItems: Group[];
  patientResource?: Patient;
  patientDto?: PatientDto;
  openAddPatientModal: boolean;
}>({
    isLoading: false,
    inviteStatus: 'Not Invited',
    inviteEmail: '',
    openSendInviteModal: false,
    isSynapseInvite: false,
    newGroupName: {name: ''},
    allGroupResources: [],
    openAddToGroupModal: false,
    openAddGroupModal: false,
    groupModalMode: 'create',
    groupMembershipTableItems: [],
    openAddPatientModal: false,
})

const groupMembershipTableHeader = ['Group Name', 'Action'];


async function onCreateGroup() {
    const fhirGroup = FHIRUtils.createGroupModel(state.newGroupName.name);

    state.isLoading = true;
    try {
        await $api.fhir.createGroup(props.workspaceSlug, fhirGroup);
        state.openAddGroupModal = false;
    } catch (error) {
        handleApiError(error, 'Unable to create group')
    } finally {
        state.isLoading = false;
    }
}

async function onAddPatientToGroupMenuClick() {
    state.isLoading = true;
    try {
        const bundleGroups = await $api.fhir.getGroups(props.workspaceSlug);
        const groups = FHIRUtils.filterBundleByType<Group>(bundleGroups, ResourceType.Group);
        if (groups.length === 0) {
            notification({
                title: 'No Groups',
                description: 'Create a group to start adding patients.',
                displayTime: 3000,
            });
            state.isLoading = false;
            state.openAddGroupModal = true;
            return;
        }
        const filteredGroups = groups.filter((group: Group) => !FHIRUtils.isPatientInGroup(group, props.patientId));
        if (filteredGroups.length === 0) {
            notification({
                title: 'No Available Groups',
                description: 'This patient is already a member of all available groups.',
                displayTime: 3000,
            });
            state.isLoading = false;
            return;
        }
        state.allGroupResources = filteredGroups;
        state.isLoading = false;
        state.openAddToGroupModal = true;
    } catch (error) {
        handleApiError(error, 'Unable to load groups')
    }
}

async function onAddPatientToGroup(group?: Group) {
    try {
        state.openAddToGroupModal = false;
        if (!group) {
            return;
        }
        state.isLoading = true;
        const newGroup = FHIRUtils.addPatientToGroup(group, props.patientId);
        await $api.fhir.updateGroup(props.workspaceSlug, newGroup.id ?? '', newGroup);
        await loadGroupMemberShipStatus();
        successNotification(`Patient has been added to group ${group.name}.`)
    } catch (error) {
        handleApiError(error, 'Unable to add patient to group')
    } finally {
        state.isLoading = false;
    }
}

async function onRemovePatientFromGroup(group: Group | null) {
    const { isCancelled } = await confirmation(
        'Are you sure you want to remove this patient from this group?',
        'Remove Patient'
    );
    if (isCancelled) { return; }
    try {
        state.openAddToGroupModal = false;
        if (!group) {
            return;
        }
        state.isLoading = true;
        const newGroup = FHIRUtils.removePatientFromGroup(group, props.patientId);
        await $api.fhir.updateGroup(props.workspaceSlug, newGroup.id ?? '', newGroup);
        await loadGroupMemberShipStatus();
        successNotification(`Patient has been removed from group ${group.name}.`)
    } catch (error) {
        handleApiError(error, 'Unable to remove patient from group')
    } finally {
        state.isLoading = false;
    }
}


async function onEditPatient(patientDto: PatientDto) {
    state.isLoading = true;

    try {
        const fhirPatient = PatientUtils.patientDtoToPatient(patientDto, state.patientResource!);
        await $api.fhir.updatePatient(props.workspaceSlug, fhirPatient);
        state.openAddPatientModal = false;
        await loadPatientData();
        successNotification('The patient has been edited.')
    } catch (error) {
        handleApiError(error, 'Unable to edit patient')
    } finally {
        emit('reloadPatientData');
        state.isLoading = false;
    }
}

async function onDeletePatient() {
    const {isCancelled} = await confirmation("Are you sure you want to delete this patient? This action cannot be undone.", "Delete Patient")
    if(isCancelled){
        return;
    }

    state.isLoading = true;

    try {
        await $api.fhir.deletePatient(props.workspaceSlug, props.patientId);
        notification({
            title: 'Success',
            description: 'The patient has been deleted.',
            displayTime: 3000,
            variant: 'success'
        });
    } catch (error) {
        handleApiError(error, 'Unable to delete patient');
    } finally {
        state.isLoading = false;
        window.close();
    }
}

async function deleteInvite() {
    try {
        if (state.inviteDto?.id != null) {
            state.isLoading = true;
            await $api.invites.delete(props.workspaceSlug, state.inviteDto?.id);
            await loadInviteStatus()
            notification({
                title: 'Success',
                description: 'The patient invitation has been revoked.',
                displayTime: 3000,
                variant: 'success'
            });
        }
    } catch (error) {
        handleApiError(error, 'Unable to revoke invitation');
    } finally {
        state.isLoading = false;
    }
}

async function loadPatientData() {
    try {
        state.isLoading = true;
        state.patientResource = await $api.fhir.getResourceById<Patient>(props.workspaceSlug, "Patient", props.patientId);
        state.patientDto = PatientUtils.patientToPatientDto(state.patientResource);
        state.inviteEmail = state.patientDto.emailAddresses ? state.patientDto.emailAddresses : '';
    } catch (error) {
        handleApiError(error, 'Unable to fetch patient information');
    } finally {
        state.isLoading = false;
    }
}

async function loadInviteStatus() {
    try {
        state.isLoading = true;
        state.inviteDto = (await $api.invites.search(props.workspaceSlug, [props.patientId]))[0];
        state.inviteStatus = PatientUtils.determinePatientInviteStatus(state.inviteDto)
    } catch (error) {
        handleApiError(error, 'Unable to fetch patient invite information');
    } finally {
        state.isLoading = false;
    }
}

async function loadGroupMemberShipStatus() {
    try {
        state.isLoading = true;
        const bundleGroups = await $api.fhir.search(props.workspaceSlug, ResourceType.Group, {member: `Patient/${props.patientId}`}) as Bundle<Group>;
        state.groupMembershipTableItems = FHIRUtils.filterBundleByType<Group>(bundleGroups, ResourceType.Group);
    } catch (error) {
        handleApiError(error, 'Unable to fetch patient groups');
    } finally {
        state.isLoading = false;
    }
}

// Send Invite...
const onSendInviteModal = function (isSynapse: boolean) {
    state.openSendInviteModal = true;

    state.isSynapseInvite = isSynapse
}

function onCopyInviteUrl() {

    if (state.inviteStatus === '') {
        state.openSendInviteModal = true;
    } else {
        copyToClipboard(window.location.origin + '/invite/code/' + state.inviteDto?.invitationCode).toString();
        notification({
            title: 'Copied',
            description: 'Copied URL to clipboard.',
            displayTime: 3000,
            variant: 'success'
        });
    }
}

async function onSendInvite() {

    try {
        state.isLoading = true;
        const sendInvite: SendInviteDto = {
            email: state.inviteEmail
        }
        const inviteDto: CreateInviteDto =
        {
            inviteType: InviteType.Organization,
            accessiblePatientId: props.patientId,
            userEmail: state.inviteEmail,
            isSynapseRole: state.isSynapseInvite
        };
        state.inviteCreateResponse = await $api.invites.create(props.workspaceSlug, inviteDto)
        await $api.invites.send(props.workspaceSlug, state.inviteCreateResponse.id, sendInvite)
        await loadInviteStatus();
        copyToClipboard(window.location.origin + '/invite/code/' + state.inviteCreateResponse.code).toString();
        notification({
            title: 'Copied',
            description: 'Created Invite and Copied URL to clipboard.',
            displayTime: 3000,
            variant: 'success'
        });
    } catch (error) {
        handleApiError(error, 'Unable to create invitation');
    } finally {
        state.openSendInviteModal = false;
        state.isLoading = false;
    }
}

loadPatientData();
loadInviteStatus();
loadGroupMemberShipStatus();
</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="state.isLoading"/>

    <!-- Add Patient Modal -->
    <AddPatientModal
        v-model="state.patientDto"
        :show="state.openAddPatientModal"
        :send-patient-data-request="false"
        mode="edit"
        :is-loading="state.isLoading"
        @close="() => state.openAddPatientModal = false"
        @edit-patient="onEditPatient"
    />

    <!-- Add Group Modal -->
    <AddGroupModal
        v-model="state.newGroupName"
        :show="state.openAddGroupModal"
        :mode="state.groupModalMode"
        @close="() => state.openAddGroupModal = false"
        @cancel="() => state.openAddGroupModal = false"
        @create-group="() => onCreateGroup()"
    />

    <!-- Add To Group Modal -->
    <AddToGroupModal
        :show="state.openAddToGroupModal"
        :groups="state.allGroupResources"
        @add-to-group="onAddPatientToGroup($event)"
        @close="() => state.openAddToGroupModal = false"
        @cancel="() => state.openAddToGroupModal = false"
    />

    <!-- Send Invite Modal -->
    <SendInviteModal
        v-model="state.inviteEmail"
        :show="state.openSendInviteModal"
        @close="() => state.openSendInviteModal = false"
        @cancel="() => state.openSendInviteModal = false"
        @send-invite="() => onSendInvite()"
    />

    <div class="w-[1100px]">
      <!-- Patient Actions -->
      <div class="grid grid-cols-12 gap-14">
        <div class="col-span-4">
          <DsLabeledText label="Actions" text="Perform actions related to this patient."/>
        </div>
        <div class="col-span-8 space-y-1">
          <DsText size="sm">
            Actions
          </DsText>
          <div class="flex gap-2">
            <DsButton id="edit-patient-button" :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" size="sm" @click="state.openAddPatientModal=true">
              Edit Patient
            </DsButton>
            <DsButton id="delete-patient-button" :color="Colors.fire" :text-color='Colors.fire' variant="outline" size="sm" @click="onDeletePatient">
              Delete Patient
            </DsButton>
          </div>
        </div>
      </div>
      <DsDivider/>

      <!-- Invitation Actions -->
      <div class="grid grid-cols-12 gap-14">
        <div class="col-span-4">
          <DsLabeledText
              label="Invitation Status"
              text="Information and actions related to patient invitations. Learn more about the different invitation types."
          />
        </div>
        <div class="flex flex-col col-span-8 space-y-1">
          <!-- Invite Status -->
          <DsText size="sm">
            Status
          </DsText>
          <DsText size="sm" weight="light" class="flex items-center gap-2">
            <DsIcon
                v-if="state.inviteStatus == InviteStatus[1]"
                name="heroicons:check-circle" size="xs"
                class="text-dark-cyan"
            />
            {{ state.inviteStatus || 'Not Invited' }}
          </DsText>

          <div class="py-2"/>

          <DsText v-if="state.inviteStatus != InviteStatus[1]" size="sm">
            Actions
          </DsText>
          <div v-if="state.inviteStatus == InviteStatus[0] || state.inviteStatus == InviteStatus[2]" class="flex gap-2">
            <DsButton id="copy-invite-url-button" :color="Colors.primary" :text-color='Colors.primary' variant="outline" size="sm" @click="onCopyInviteUrl" >
              Copy Invite URL
            </DsButton>
            <DsButton id="revoke-invite-button" :color="Colors.fire" :text-color='Colors.fire' variant="outline" size="sm" @click="deleteInvite">
              Revoke Invite
            </DsButton>
          </div>
          <div v-if="state.inviteStatus == ''" class="flex gap-2">
            <DsButton id="share-data-with-patient-button" :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" size="sm" @click="onSendInviteModal(false)" >
              Share Data With Patient
            </DsButton>
            <DsButton id="request-data-from-patient-button" :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" size="sm" @click="onSendInviteModal(true)" >
              Request Data From Patient
            </DsButton>
          </div>
        </div>
      </div>
      <DsDivider/>

      <!-- Group Memberships Actions -->
      <div class="grid grid-cols-12 gap-14">
        <div class="col-span-4">
          <DsLabeledText
              label="Group Memberships"
              text="Add or remove this patient to or from various FHIR groups. Learn more about how to use FHIR groups."/>
        </div>
        <div class="flex flex-col col-span-8 space-y-2">
          <DsText size="sm">
            Actions
          </DsText>
          <div>
            <DsButton
                :color="Colors.primary"
                :text-color='Colors.primary'
                variant="outline"
                size="sm"
                @click="onAddPatientToGroupMenuClick"
            >
              Add to Group
            </DsButton>
          </div>

          <div class="py-2"/>

          <DsText size="sm">
            Group Memberships
          </DsText>
          <div v-if="state.groupMembershipTableItems.length>0" class="flex gap-2">
            <DsTable
                :headers="groupMembershipTableHeader"
                :items="state.groupMembershipTableItems"
                :id-selector="item => item.id!"
            >
              <template #default="{item}">
                <DsText size="sm"> {{ item.name }}</DsText>
                <DsButton
                    :color="Colors.fire"
                    :text-color='Colors.fire'
                    variant="outline"
                    size="sm"
                    @click="onRemovePatientFromGroup(item)"
                >
                  Remove
                </DsButton>
              </template>
            </DsTable>
          </div>
          <div v-else>
            <DsText weight="light" size="sm">
              None
            </DsText>
          </div>
        </div>
      </div>
    </div>
  </DsContainer>
</template>
