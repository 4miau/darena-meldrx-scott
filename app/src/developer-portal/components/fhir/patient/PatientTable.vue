<script setup lang="ts">
import type { Bundle, Patient } from 'fhir/r4';
import type { InviteDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Invites';
import { SelectActions } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/SelectAction';
import DsFhirResourceTablePager from '~/components/ui/table/DsFhirResourceTablePager.vue';
import PatientUtils from '~/utils/PatientUtils';
import type { WorkspaceDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto';
import { FhirServerType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/FhirServerType';

const route = useRoute();

const props = defineProps<{
    workspace: WorkspaceDto;
    patients: Patient[];
    invites: InviteDto[];
    bundle: Bundle;
}>();

const emit = defineEmits<{
    'action': [SelectActions, patient: Patient];
    'goToPage': [page: number]
}>();

const tableHeaders = ['', 'Patient Name', 'Sex', 'Age', 'Date of Birth', 'Invite Status', 'Actions'];
const selectedAction = ref<string>('');

const getInviteStatus = (patient: Patient) => {
    const invite = props.invites.find(x => x.accessiblePatientId === patient.id) as InviteDto

    return PatientUtils.determinePatientInviteStatus(invite)
}

const getActionsList = (patient: Patient) => {
    const inviteStatus = getInviteStatus(patient)

    return [
        {
            title: 'CONNECT WITH PATIENT',
            options: [
                { value: SelectActions.sendInvite, title: 'Send Invitation' },
                { value: SelectActions.copyInvite, title: 'Copy Invite URL', hide: inviteStatus === 'Requested' },
                { value: SelectActions.revokeInvite, title: 'Revoke Invitation', hide: inviteStatus !== 'Invited' },
            ],
            hide: inviteStatus === 'Accepted' || props.workspace.type === FhirServerType.Virtual,
        },
        {
            title: 'REQUEST DATA FROM PATIENT',
            options: [
                { value: SelectActions.sendRequest, title: 'Send Request' },
                { value: SelectActions.copyInvite, title: 'Copy Invite URL', hide: inviteStatus !== 'Requested' },
                { value: SelectActions.revokeInvite, title: 'Revoke Invitation', hide: inviteStatus !== 'Requested' },
            ],
            hide: inviteStatus === 'Accepted' || props.workspace.type === FhirServerType.Virtual,
        },
        {
            title: 'OTHER ACTIONS',
            options: [
                { value: SelectActions.addToGroup, title: 'Add To Group' },
                { value: SelectActions.uploadDocument, title: 'Upload Document', hide: props.workspace.type === FhirServerType.Virtual },
                { value: SelectActions.edit, title: 'Edit Patient' },
                { value: SelectActions.openInVirtualWorkspace, title: 'Open in Virtual Workspace', hide: props.workspace.isLiteWorkspace || props.workspace.type === FhirServerType.Virtual },
                { value: SelectActions.delete, title: 'Delete Patient' }
            ]
        }
    ]
}
function onActionSelected(value: SelectActions, patient: Patient) {
    selectedAction.value = ''
    emit('action', value, patient);
}
</script>

<template>
  <DsTable
    v-if="patients"
    :headers="tableHeaders"
    :items="patients"
    :id-selector="item => item.id!"
  >
    <template #default="{item}">
      <DsAvatar
        :alt="PatientUtils.formatName(item)[0].toUpperCase()"
        size="sm"
        class="bg-bliss text-silver border border-silver"
      />

      <DsLink underline="always" new-tab :href="`/workspaces/${route.params.workspaceSlug}/patient/${item.id}`">
        <DsText
          size="md"
          weight="light"
          class="cursor-pointer"
          @click="onActionSelected(SelectActions.openInApp, item)"
        >
          {{ PatientUtils.formatName(item) }}
        </DsText>
      </DsLink>

      <DsText size="md" weight="light" class="capitalize">
        {{ PatientUtils.formatSex(item) }}
      </DsText>

      <DsText size="md" weight="light">
        {{ PatientUtils.formatAge(item) }}
      </DsText>

      <DsText size="md" weight="light">
        {{ PatientUtils.formatDateOfBirth(item) }}
      </DsText>

      <DsText size="md" weight="light">
        {{ getInviteStatus(item) }}
      </DsText>

      <DsDropdown
        :id="PatientUtils.formatName(item).replaceAll(' ', '-').toLowerCase() + '-actions'"
        :options="getActionsList(item)"
        label="Actions"
        @select="(v) => { onActionSelected(v, item) }"
      />
    </template>
    <template #footer>
      <DsFhirResourceTablePager class="pl-6" :bundle="bundle" @go-to-page="emit('goToPage', $event)" />
    </template>
  </DsTable>
</template>
