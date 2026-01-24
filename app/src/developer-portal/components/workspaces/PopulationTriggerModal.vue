<script setup lang="ts">
import {Colors} from '~/types/ui/colors';
import {
    type CreatePopulationTrigger,
    type PopulationTrigger,
    PopulationType,
    type UpdatePopulationTrigger
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PopulationTrigger";
import type {Group} from "fhir/r4";
import type {ApiFilter} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter";
import ValidationRules from "~/utils/ValidationRules";
import type {IDsSelectItem} from "~/types/ui/DsSelect";
import ResourceType from "~/types/fhir/ResourceType";

const { $api } = useNuxtApp();
const formRef = ref<FormRef>();

const props = defineProps<{
  show: boolean,
  workspaceSlug: string;
  populationTrigger?: PopulationTrigger;
}>();

const emit = defineEmits<{
  'editPopulationTrigger': [value: UpdatePopulationTrigger];
  'createPopulationTrigger': [appDetails: CreatePopulationTrigger];
  'close': [];
}>();

const state = reactive<{
  form: CreatePopulationTrigger | UpdatePopulationTrigger;
  groups?: Group[];
  apiFilter: ApiFilter;
  loading: boolean;
}>({
    loading: false,
    form: createDefaultPopulationTrigger(),
    apiFilter: {
        page: 1,
        filter: '',
    }
});

const extensionsMap = ref<IDsSelectItem<string>[]>([]);
const groupsMap = ref<IDsSelectItem<string>[]>([]);

function createDefaultPopulationTrigger(): CreatePopulationTrigger | UpdatePopulationTrigger{
  
    if(props.populationTrigger) {
        return {...props.populationTrigger}
    }
    
    return {
        populationType: PopulationType.Workspace,
        groupId: '',
        includedIndicators: [],
        cdsServiceId: '',
    }
}

function onSubmit() {
    if (!formRef.value) return;
    const isValid = formRef.value.validate();
    if (!isValid) return;

    if (props.populationTrigger) {
        emit('editPopulationTrigger', state.form as UpdatePopulationTrigger);
    } else {
        emit('createPopulationTrigger', state.form as CreatePopulationTrigger);
    }
}

async function loadExtensions(apiFilter: ApiFilter) {
    state.loading = true
    try {
        const pagedExtensions = await $api.ehr.list(props.workspaceSlug, apiFilter);
        extensionsMap.value = pagedExtensions.resources.map(x => ({
            value: x.appId,
            title: x.appName
        }));
        state.apiFilter = apiFilter;
        state.apiFilter.page = pagedExtensions?.currentPage ?? 1;
    } catch (error) {
        handleApiError(error, 'Unable to load extensions')
    } finally {
        state.loading = false
    }
}

async function loadGroups() {
    state.loading = true;
    
    try{
        const bundleGroups = await $api.fhir.getGroups(props.workspaceSlug);
        const groups = FHIRUtils.filterBundleByType<Group>(bundleGroups, ResourceType.Group);
        if (groups.length === 0) {
            state.loading = false;
            return;
        }
        groupsMap.value = groups.map((group: Group) => ({ value: group.id!, title: group.name ?? '(No name)' }));

    } catch (error) {
        handleApiError(error, 'Unable to load extensions')
    } finally {
        state.loading = false
    }
}

watch(() => props.show, (value) => {
    if(value){
        state.form = createDefaultPopulationTrigger()
    }
});
loadExtensions(state.apiFilter)
loadGroups()
</script>

<template>
  <DsModal :model-value="props.show" @close="$emit('close')">
    <DsModalProgressCard :total-steps="1" :current-step="1">
    <div class="flex w-full py-8 bg-space justify-center items-center">
      <div class="flex w-full items-center justify-center">
        <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center">
          <MeldRxApp/>
        </div>
      </div>
      <div class="flex-1"/>
    </div>
    <div class="flex flex-col w-full py-5 px-10">
      
      <div class="w-full flex-col justify-center items-center gap-5 flex">
        <DsText size="2xl" weight="light">
          Population Trigger Details
        </DsText>

        <DsText size="sm" weight="light">
          Configure the extension to use and the population to run on.
        </DsText>
      </div>
      <!-- Population Trigger Details -->
      <DsForm ref="formRef">
        <div  class="flex flex-col space-y-2">
          <DsSelect
              v-model="state.form.cdsServiceId"
              :items="extensionsMap"
              label="Extensions"
              required
              :rules="[ValidationRules.describeNotEmpty('Please select an extension')]"
              searchable
              placeholder="Select an extension to use"
          />

          <DsLabeledInput label="Population Type" required>
            <DsButtonGroup
                v-model="state.form.populationType"
                :active-color="Colors.primary"
                :inactive-color='Colors.white'
                :active-text-color='Colors.primary'
                :rounded="false"
            >
              <DsButton :value="PopulationType.Workspace" size="sm" variant="outline">
                Workspace
              </DsButton>
              <DsButton :value="PopulationType.Group" size="sm" variant="outline">
                Group
              </DsButton>
            </DsButtonGroup>
          </DsLabeledInput>

          <DsSelect
              v-if="state.form.populationType === PopulationType.Group"
              v-model="state.form.groupId"
              :items="groupsMap"
              label="Groups"
              required
              :rules="[ValidationRules.describeNotEmpty('Please select a group')]"
              searchable
              placeholder="Select a group"
          />

          <DsLabeledInput label="Indicators to include" required>
            <DsButtonGroup
                v-model="state.form.includedIndicators"
                :active-color="Colors.primary"
                :inactive-color='Colors.white'
                :active-text-color='Colors.primary'
                :rounded="false"
                multiple
            >
              <DsButton value="critical" size="sm" variant="outline">
                Critical
              </DsButton>
              <DsButton value="warning" size="sm" variant="outline">
                Warning
              </DsButton>
              <DsButton value="info" size="sm" variant="outline">
                Informational
              </DsButton>
            </DsButtonGroup>
          </DsLabeledInput>
        </div>
      </DsForm>

      <DsDivider/>

      <!-- Buttons -->
      <div class="flex justify-center w-full gap-5">
        <DsButton variant="outline" :color="Colors.secondary" :text-color='Colors.secondary' @click="$emit('close')">
          Close
        </DsButton>
        <DsButton :color="Colors.secondary" @click="onSubmit">
          {{ props.populationTrigger ? 'Update Trigger' : 'Create Population Trigger' }}
        </DsButton>
      </div>
    </div>
    </DsModalProgressCard>
  </DsModal>
</template>
