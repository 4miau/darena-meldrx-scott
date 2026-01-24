<script setup lang="ts">
import { Colors } from '~/types/ui/colors'
import { LinkedApiActionTarget } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/LinkedApiActionTarget'
import type { WorkspaceDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto'
import ResourceType from '~/types/fhir/ResourceType'
import type LinkedFhirApiActionConfigDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/LinkedFhirApiActionConfigDto'
import { LinkedApiAction } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/LinkedApiAction'
import type { Guid } from '~/types/common/Guid'
import type {IDsSelectItem} from "~/types/ui/DsSelect";

definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    allowLinked: true,
});
useHead({ title: 'Data Rules | MeldRx' });
const { $api } = useNuxtApp()

type actionMode = 'read' | 'write'

const route = useRoute()
const isLoading = ref<boolean>(false)
const workspaceSlug = ref<string>(route.params.workspaceSlug as string)

// Setup page state...
interface IState { workspace: WorkspaceDto | null }
const state = useState<IState>('workspaces', () => ({
    workspace: null
}))

interface IStateDataRules {dataActionRules: LinkedFhirApiActionConfigDto[] | null }
const linkedFhirApiId = computed(() => state.value.workspace?.linkedFhirApiDto?.id)
const workspaceId = computed(() => state.value.workspace?.id)
const dataRules = ref<LinkedFhirApiActionConfigDto[]>()
const pseudoDataRules = ref<LinkedFhirApiActionConfigDto[]>()

const dataRulesState = useState<IStateDataRules>('dataActionRules', () => ({ dataActionRules: null }))
const defaultReadActionTarget: LinkedApiActionTarget = LinkedApiActionTarget.Both;
const defaultWriteActionTarget: LinkedApiActionTarget = LinkedApiActionTarget.MeldRx;

// Control for Bulk Accordian display/hide
const dropDownBulkShow = ref(false)
const dropDownBulkIcon = computed(() => {
    return dropDownBulkShow.value ? 'heroicons:chevron-up' : 'heroicons:chevron-down'
})

// Load workspaces from server...
async function loadWorkspace(id: string): Promise<WorkspaceDto | null> {
    isLoading.value = true
    const workspace = await $api.workspaces.get(id)
    isLoading.value = false
    state.value.workspace = workspace
    return workspace
}

// Load Workspace information, after page is Mounted. And extract data rules accordinlgy
onMounted(async () => {
    await loadWorkspace(workspaceSlug.value)
    if (!workspaceId.value) {
        return notification({ title: 'Error', description: 'Unable to load workspace data rules', displayTime: 3000, variant: 'error' });
    }
    if (linkedFhirApiId.value !== undefined) {
        await loadDataRules(workspaceId.value)
    }
})
watch(linkedFhirApiId, async (newLinkedFhirApiId) => {
    if (!workspaceId.value) {
        return notification({ title: 'Error', description: 'Unable to load workspace data rules', displayTime: 3000, variant: 'error' });
    }
    if (newLinkedFhirApiId) {
        await loadDataRules(workspaceId.value)
    }
})

// Loads Data Rules from the DB, and fills the gap in rules for missing Resources by reverting to the defaults
async function loadDataRules(workspaceId: string): Promise<LinkedFhirApiActionConfigDto[] | null> {
    isLoading.value = true
    dataRules.value = await $api.linkedFhirApiAction.list(workspaceId)
    isLoading.value = false
    dataRulesState.value.dataActionRules = dataRules.value
    // Create Master copy of all needed Rules, configured to Default Action.
    pseudoDataRules.value = Object.keys(ResourceType)
        .map(resourceType => ({
            resourceType,
            linkedApiAction: LinkedApiAction.Create,
            actionTarget: defaultWriteActionTarget,
            linkedFhirApiId: linkedFhirApiId.value as Guid
        }))
        .concat(
            Object.keys(ResourceType).map(resourceType => ({
                resourceType,
                linkedApiAction: LinkedApiAction.Read,
                actionTarget: defaultReadActionTarget,
                linkedFhirApiId: linkedFhirApiId.value as Guid
            }))
        )
    // Fill the Gaps in DB Data with pseudo Items
    for (const item of pseudoDataRules.value) {
        if (!dataRules.value?.some(x => x.resourceType === item.resourceType && x.linkedApiAction === item.linkedApiAction))
        {
            dataRules.value?.push(item)
        }
    }
    return dataRules.value
}

const workspaceConfig = [
    { value: LinkedApiActionTarget.External, title: LinkedApiActionTarget.External },
    { value: LinkedApiActionTarget.MeldRx, title: LinkedApiActionTarget.MeldRx },
    { value: LinkedApiActionTarget.Both, title: LinkedApiActionTarget.Both }
]
const fhirResources = computed(() => {
    const resourceTypes = dataRules.value?.map(x => x.resourceType) || []
    const uniqueResourceTypes = Array.from(new Set(resourceTypes))
    const convertedResourceTypes: IDsSelectItem<string>[] = []
    for (const resourceType  of uniqueResourceTypes) {
        convertedResourceTypes.push({value: resourceType, title: resourceType})
    }
    return convertedResourceTypes
})

// DataRulesHash flattens the Rules for UI button display
interface KeyValue { read: LinkedApiActionTarget, write: LinkedApiActionTarget, linkedFhirApiId: Guid, readId: Guid, writeId: Guid }
type DataRulesHash = Record<string, KeyValue>
const dataRulesHash = ref<DataRulesHash>({})
watch(dataRules, (newDataRules) => {
    dataRulesHash.value = {}
    for (const resItem of newDataRules ?? []) {
        if (!dataRulesHash.value[resItem.resourceType]) {
            dataRulesHash.value[resItem.resourceType] = {
                read: LinkedApiActionTarget.Both,
                write: LinkedApiActionTarget.MeldRx,
                linkedFhirApiId: '',
                readId: '',
                writeId: ''
            }
        }
        if (resItem.linkedApiAction === LinkedApiAction.Read) {
            dataRulesHash.value[resItem.resourceType].read = resItem.actionTarget
            dataRulesHash.value[resItem.resourceType].readId = resItem.id ?? ''
            dataRulesHash.value[resItem.resourceType].linkedFhirApiId = resItem.linkedFhirApiId
        }
        if (resItem.linkedApiAction === LinkedApiAction.Create) {
            dataRulesHash.value[resItem.resourceType].write = resItem.actionTarget
            dataRulesHash.value[resItem.resourceType].writeId = resItem.id ?? ''
            dataRulesHash.value[resItem.resourceType].linkedFhirApiId = resItem.linkedFhirApiId
        }
    }
})

const formRef = ref<FormRef>()
const bulkForm = ref<{
  actionTypes: actionMode[],
  fhirResources: string[],
  action: LinkedApiActionTarget
}>({
    actionTypes: [],
    fhirResources: [],
    action: LinkedApiActionTarget.Both
})

async function updateBulkRules() {
    if (!formRef.value?.validate()) { return }

    const changeDataRules: LinkedFhirApiActionConfigDto[] = []
    for (const resourceType of bulkForm.value.fhirResources) {
        if (bulkForm.value.actionTypes.includes('read')) {
            const payload: LinkedFhirApiActionConfigDto = {
                actionTarget: bulkForm.value.action,
                linkedApiAction: LinkedApiAction.Read,
                linkedFhirApiId: linkedFhirApiId.value ?? '',
                resourceType
            }
            if (dataRulesHash.value[resourceType].readId !== '') {
                payload.id = dataRulesHash.value[resourceType].readId
            }
            dataRulesHash.value[resourceType].read = bulkForm.value.action
            changeDataRules.push(payload)
        }
        if (bulkForm.value.actionTypes.includes('write')) {
            const payload: LinkedFhirApiActionConfigDto = {
                actionTarget: bulkForm.value.action,
                linkedApiAction: LinkedApiAction.Create,
                linkedFhirApiId: linkedFhirApiId.value ?? '',
                resourceType
            }
            if (dataRulesHash.value[resourceType].writeId !== '') {
                payload.id = dataRulesHash.value[resourceType].writeId
            }
            dataRulesHash.value[resourceType].write = bulkForm.value.action
            changeDataRules.push(payload)
        }
    }
    try {
        isLoading.value = true
        if (!workspaceId.value) {
            return notification({ title: 'Error', description: 'Unable to update workspace data rules', displayTime: 3000, variant: 'error' });
        }
        await $api.linkedFhirApiAction.bulkUpsert(workspaceId.value, changeDataRules)
        isLoading.value = false
        await loadDataRules(workspaceId.value)
    } catch (error) {
        notification({
            title: 'Error',
            description: `Error during Bulk Rules change: ${error}`,
            displayTime: 3000,
            variant: 'error'
        })
    }
}

// If user selects "Write", make sure they don't select "Both". So switch it to "MeldRx"...
watch(bulkForm.value, (newBulkForm) => {
    if (newBulkForm.actionTypes.includes('write') && newBulkForm.action === LinkedApiActionTarget.Both) {
        bulkForm.value.action = LinkedApiActionTarget.MeldRx
    }
})

async function handleSelectChange(actionTarget: LinkedApiActionTarget, resourceType: string, mode: actionMode) {
    isLoading.value = true
    try {
        if (mode === 'read') {
            dataRulesHash.value[resourceType].read = actionTarget
            const payload: LinkedFhirApiActionConfigDto = {
                actionTarget,
                linkedApiAction: LinkedApiAction.Read,
                linkedFhirApiId: linkedFhirApiId.value ?? '',
                resourceType
            }
            if (dataRulesHash.value[resourceType].readId !== '') {
                payload.id = dataRulesHash.value[resourceType].readId
            }
            if (!workspaceId.value) {
                return notification({ title: 'Error', description: 'Unable to update workspace data rules', displayTime: 3000, variant: 'error' });
            }
            const resp = await $api.linkedFhirApiAction.upsert(workspaceId.value, payload)

            dataRulesHash.value[resourceType].readId = resp.id as Guid
        }
        if (mode === 'write') {
            dataRulesHash.value[resourceType].write = actionTarget
            const payload: LinkedFhirApiActionConfigDto = {
                actionTarget,
                linkedApiAction: LinkedApiAction.Create,
                linkedFhirApiId: linkedFhirApiId.value ?? '',
                resourceType
            }
            if (dataRulesHash.value[resourceType].writeId !== '') {
                payload.id = dataRulesHash.value[resourceType].writeId
            }
            if (!workspaceId.value) {
                return notification({ title: 'Error', description: 'Unable to update workspace data rules', displayTime: 3000, variant: 'error' });
            }
            const resp = await $api.linkedFhirApiAction.upsert(workspaceId.value, payload)

            dataRulesHash.value[resourceType].writeId = resp.id as Guid
        }

        notification({
            title: 'Success',
            description: `Updated rules for ${resourceType}, ${mode} to ${actionTarget}`,
            displayTime: 1000,
            variant: 'success'
        })
    } catch (error) {
        handleApiError(error, `Unable to change rules for ${resourceType}`);
    }

    isLoading.value = false
}
</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="isLoading" />
    <!-- Top page header and Back Button -->
    <div>
      <DsText size="2xl" weight="light" class="block">
        Data Rules
      </DsText>
      <div class="pb-5" />
      <DsText size="xs" weight="light">
        Below, you may define a custom rules for routing healthcare data. Choose whether to perform read or write actions for certain resource in your MeldRx data storage, remotely via your linked app , or in both places.
      </DsText>
      <div class="pb-5" />
    </div>

    <!-- Bulk Update Accordian -->
    <div class="bg-bliss border-t border-b border-space p-1 py-2 cursor-pointer flex align-center items-center" @click="dropDownBulkShow=!dropDownBulkShow">
      <DsIcon class="mr-1" :name="dropDownBulkIcon" size="xs" />
      <DsText size="md" weight="light" class="self-center">
        Bulk Updates
      </DsText>
    </div>

    <!-- Bulk Update Menu Options -->

    <DsForm ref="formRef">
      <div class="py-2" />
      <div v-if="dropDownBulkShow" class="grid grid-cols-12 gap-14 pl-6 pr-6">
        <!-- Action Selection -->
        <div class="col-span-5">
          <DsText size="sm" weight="normal">
            Action Selection
          </DsText>
          <div class="pb-2" />
          <DsText size="xs" weight="light">
            Select the action(s) to trigger this rule
          </DsText>
        </div>
        <!-- Trigger Action -->
        <div class="col-span-7">
          <DsLabeledInput label="Trigger Action" required>
            <DsButtonGroup
              v-model="bulkForm.actionTypes"
              multiple
              required
              :rules="[
                [(v) => (v?.length ?? 0) > 0, 'Trigger action must be selected']
              ]"
            >
              <DsButton value="read" :text-color='Colors.gray'>
                Read
              </DsButton>
              <DsButton value="write" :text-color='Colors.gray'>
                Write
              </DsButton>
            </DsButtonGroup>
          </DsLabeledInput>
        </div>

        <!-- Resource Type Selection -->
        <div class="col-span-5">
          <DsText size="sm" weight="normal">
            Resource Type Selection
          </DsText>
          <div class="pb-2" />
          <DsText size="xs" weight="light">
            Choose the FHIR resource types to be included in this rule
          </DsText>
        </div>
        <!-- Resource Types -->
        <div class="col-span-7">
          <DsLabeledInput label="Resource Types" required>
            <DsSelect
              v-model="bulkForm.fhirResources"
              :items="fhirResources"
              placeholder="Select FHIR resources"
              searchable
              search-placeholder="Search FHIR resources..."
              multiple
              required
              :rules="[
                [(v) => (v?.length ?? 0) > 0, 'At least one resource type must be selected']
              ]"
            />
          </DsLabeledInput>
        </div>

        <!-- Action Target -->
        <div class="col-span-5">
          <DsText size="sm" weight="normal">
            Action Target
          </DsText>
          <div class="pb-2" />
          <DsText size="xs" weight="light">
            Choose where this rule should target.
          </DsText>
        </div>

        <!-- Targets -->
        <div class="col-span-7">
          <DsLabeledInput label="Target" required>
            <DsButtonGroup
              v-model="bulkForm.action"
              :vertical="false"
              :active-color="Colors.secondary"
              required
              :rules="[
                [(v) => (v?.length ?? 0) > 0, 'Trigger action must be selected']
              ]"
            >
              <DsButton :value="LinkedApiActionTarget.External" variant="filled" :text-color='Colors.gray'>
                External
              </DsButton>
              <DsButton :value="LinkedApiActionTarget.MeldRx" variant="filled" :text-color='Colors.gray'>
                MeldRx
              </DsButton>
              <DsButton :value="LinkedApiActionTarget.Both" variant="filled" :text-color='Colors.gray' :disabled="bulkForm.actionTypes.includes('write')">
                Both
              </DsButton>
            </DsButtonGroup>
          </DsLabeledInput>
        </div>

        <div class="col-span-12">
          <DsButton class="col-span-3" variant="filled" :color="Colors.primary" @click="updateBulkRules">
            Update Rules
          </DsButton>
        </div>
      </div>
    </DsForm>
    <DsDivider />

    <!-- Data Rules -->
    <div class="max-w-3xl border border-silver border-solid m-15 p-15">
      <div class="border border-silver border-solid">
        <div class="grid grid-cols-4 gap-4">
          <div class="col-span-2 p-4">
            <DsText size="md" weight="light">
              Resource Type
            </DsText>
          </div>
          <div class="col-span-1 p-1">
            <DsText size="md" weight="light">
              Read
            </DsText><br>
            <DsText size="xs" weight="light">
              Read, Search
            </DsText>
          </div>
          <div class="col-span-1 p-1">
            <DsText size="md" weight="light">
              Write
            </DsText><br>
            <DsText size="xs" weight="light">
              Create, Update, Delete
            </DsText>
          </div>
        </div>
        <div v-for="(resourceType, index) in fhirResources" :key="index">
          <div class="grid grid-cols-4 gap-4" :class="index % 2 === 0 ? 'bg-white' : 'bg-bliss'">
            <div class="col-span-2 p-4 flex items-center">
              <DsText size="sm" weight="light">
                {{ resourceType.title }}
              </DsText>
            </div>
            <div class="col-span-1 p-4">
              <DsSelect
                :model-value="dataRulesHash[resourceType.value].read"
                :items="workspaceConfig"
                @update:model-value="e => handleSelectChange(e, resourceType.value, 'read')"
              />
            </div>
            <div class="col-span-1 p-4">
              <DsSelect
                :model-value="dataRulesHash[resourceType.value].write"
                :items="workspaceConfig.filter(x => x.value !== LinkedApiActionTarget.Both)"
                @update:model-value="e => handleSelectChange(e, resourceType.value, 'write')"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  </DsContainer>
</template>
