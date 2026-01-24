<script setup lang="ts">
import type { Group } from 'fhir/r4'
import AddGroupModal from '~/components/fhir/groups/AddGroupModal.vue';
import { Colors } from '~/types/ui/colors';
import ResourceType from '~/types/fhir/ResourceType'

definePageMeta({ layout: 'workspace', middleware: ['require-workspace'] });
useHead({ title: 'Groups | MeldRx' });

const { $api } = useNuxtApp()
const route = useRoute()

const confirmation = useConfirmation();
const isLoading = ref<boolean>(false);
const workspaceSlug = ref<string>(route.params.workspaceSlug as string);
const openAddGroupModal = ref(false);
const groupModalMode = ref<'create' | 'edit'>('create');
const currentGroupModalData = ref<{ name: string }>({ name: '' });
const currentGroupEditing = ref<Group | null>(null);
const tableHeaders = ['Name', 'Number of Patients', 'Actions'];

const state = reactive<{groups: Group[]}>({ groups: [] })

async function loadGroupsForWorkspace(workspaceSlug: string) {
    isLoading.value = true;

    try {
        const bundleGroups = await $api.fhir.getGroups(workspaceSlug);
        state.groups = FHIRUtils.filterBundleByType<Group>(bundleGroups, ResourceType.Group)
    } catch (error) {
        handleApiError(error, 'Unable to load groups')
    } finally {
        isLoading.value = false;
    }
}

function onCreateGroupModalClick() {
    groupModalMode.value = 'create';
    currentGroupModalData.value = { name: '' };
    openAddGroupModal.value = true;
}

function onEditGroupModalClick(group: Group) {
    groupModalMode.value = 'edit';
    currentGroupEditing.value = group;
    currentGroupModalData.value = { name: group.name ?? '' };
    openAddGroupModal.value = true;
}

async function onCreateGroup() {
    const fhirGroup = FHIRUtils.createGroupModel(currentGroupModalData.value.name);

    isLoading.value = true;
    try {
        await $api.fhir.createGroup(workspaceSlug.value, fhirGroup);
        openAddGroupModal.value = false;
        await loadGroupsForWorkspace(workspaceSlug.value);
    }
    catch (error) {
        handleApiError(error, 'Unable to create group')
    } finally {
        isLoading.value = false;
    }
}

async function onEditGroup(groupData: { name: string }) {
    if (!currentGroupEditing.value) { return; }
    if (!currentGroupEditing.value.id) { return; }
    if (currentGroupEditing.value.name === groupData.name) {
        openAddGroupModal.value = false;
        return;
    }

    currentGroupEditing.value.name = groupData.name;
    currentGroupEditing.value.identifier = [
        {
            system: 'meldrx',
            value: groupData.name
        }
    ];

    isLoading.value = true;
    try {
        await $api.fhir.updateGroup(workspaceSlug.value, currentGroupEditing.value.id, currentGroupEditing.value);
        openAddGroupModal.value = false;
        await loadGroupsForWorkspace(workspaceSlug.value);
    }
    catch (error) {
        handleApiError(error, 'Unable to update group')
    }
    finally {
        isLoading.value = false;
    }
}

// Delete the group...
async function onDeleteGroup(groupId: string) {
    const {isCancelled} = await confirmation("Are you sure you want to delete this group? This action cannot be undone.", "Delete Group")
    if(isCancelled){
        return;
    }

    isLoading.value = true;

    try {
        await $api.fhir.deleteGroup(workspaceSlug.value, groupId);
        await loadGroupsForWorkspace(workspaceSlug.value);
    } catch(error){
        handleApiError(error, 'Failed to delete group')
    }

    isLoading.value = false;
}

loadGroupsForWorkspace(workspaceSlug.value);
</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="isLoading" />

    <!-- Add/Edit Group Modal -->
    <AddGroupModal
      v-model="currentGroupModalData"
      :show="openAddGroupModal"
      :mode="groupModalMode"
      @close="() => openAddGroupModal = false"
      @cancel="() => openAddGroupModal = false"
      @create-group="() => onCreateGroup()"
      @edit-group="(groupData) => onEditGroup(groupData)"
    />

    <div>
      <!-- Header Text -->
      <DsText size="2xl" weight="light">
        Groups
      </DsText>
      <div class="pb-5" />
      <!-- Description -->
      <DsText size="sm" weight="light">
        Manage the FHIR groups for this workspace.
      </DsText>
    </div>
    <div class="pb-5" />

    <!-- No Groups -->
    <div v-if="state.groups.length === 0">
      <div class="flex flex-col items-center justify-center h-full py-20 border-2 border-silver space-y-5">
        <DsText size="2xl" weight="light">
          No Groups
        </DsText>
        <DsText size="sm" weight="light">
          There are no groups in this workspace. Click the button below to add a group.
        </DsText>
        <DsButton variant="filled" @click="() => onCreateGroupModalClick()">
          <DsIcon name="heroicons:plus" size='sm' />
          Create Group
        </DsButton>
      </div>
    </div>

    <div v-else class="space-y-5">
      <!-- Add Group Button -->
      <DsButton variant="filled" @click="() => onCreateGroupModalClick()">
        <DsIcon name="heroicons:plus" size='sm' />
        Create Group
      </DsButton>

      <!-- Groups Table -->
      <DsTable
        v-if="state.groups"
        :headers="tableHeaders"
        :items="state.groups"
        :id-selector="item => item.id!"
      >
        <template #default="{item}">
          <div>
            <DsLink underline="always" :href="`group?id=${item.id}`">
              <DsText size="lg">
                {{ item.name }}
              </DsText>
            </DsLink>
            <div class="mb-2 mt-1">
              <DsCopyButton :text-to-copy="item.id ?? ''" :show-toast-on-copy="true" class="text-xs font-light">
                Group ID: {{ item.id }}
              </DsCopyButton>
            </div>
          </div>

          <DsCopyButton :text-to-copy="FHIRUtils.getPatientIdsFromGroup(item).join(',')" :show-toast-on-copy="true" class="text-xs font-light">
            {{ FHIRUtils.getNumberOfMembersInGroup(item) }} Patients
          </DsCopyButton>

          <div class="space-x-2">
            <DsButton
              :id="`edit-${item.name?.replaceAll(' ', '-').toLowerCase()}-button`"
              :color="Colors.white"
              :text-color='Colors.gray'
              variant="outline"
              @click="onEditGroupModalClick(item)"
            >
              Edit
            </DsButton>
            <DsButton
              :id="`delete-${item.name?.replaceAll(' ', '-').toLowerCase()}-button`"
              :color="Colors.fire"
              :text-color='Colors.fire'
              variant="outline"
              @click="onDeleteGroup(item.id!)"
            >
              Delete
            </DsButton>
          </div>
        </template>
      </DsTable>
    </div>
  </DsContainer>
</template>
