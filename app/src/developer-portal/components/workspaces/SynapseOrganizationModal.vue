<script setup lang="ts">
import {Colors} from '~/types/ui/colors';
import {
    type CreateWorkspaceSynapseOrganization,
    SynapseOrganizationType, type UpdateWorkspaceSynapseOrganization,
    type WorkspaceSynapseOrganization
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceSynapseOrganization";
import type {IDsSelectItem} from "~/types/ui/DsSelect";
import type {WorkspaceExternalApp} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceExternalApp";

const props = defineProps<{
  show: boolean,
  synapseOrganization?: WorkspaceSynapseOrganization;
  externalApps: WorkspaceExternalApp[];
}>();

function createDefaultSynapseOrganization(): CreateWorkspaceSynapseOrganization | UpdateWorkspaceSynapseOrganization {

    if (props.synapseOrganization) {
        state.showAddress = true
        return {...props.synapseOrganization}
    }

    return {
        organizationName: '',
        externalAppId: '',
        type: SynapseOrganizationType.Provider,
        fhirEndpoint: ''
    }
}

const state = reactive<{
  form: CreateWorkspaceSynapseOrganization | UpdateWorkspaceSynapseOrganization;
  showAddress: boolean
}>({
    form: createDefaultSynapseOrganization(),
    showAddress: false,
});

const externalAppOptions = computed<IDsSelectItem<string>[]>(() => props.externalApps.map((externalApp: WorkspaceExternalApp) => ({ value: externalApp.id!, title: externalApp.clientName })));
const synapseOrganizationTypes = [
    { value: SynapseOrganizationType.Provider, title: 'Provider' },
    { value: SynapseOrganizationType.Payer, title: 'Payer' },
];
// Watch for changes in `props.externalApp` and update `state.form`
watch(() => props.show, (value) => {
    if(value){
        state.showAddress = false
        state.form = createDefaultSynapseOrganization()
    }
});

const emit = defineEmits<{
  'editSynapseOrganization': [value: UpdateWorkspaceSynapseOrganization];
  'addSynapseOrganization': [value: CreateWorkspaceSynapseOrganization];
  'close': [];
}>();

function onSubmit() {
    if (!formRef.value) return;
    const isValid = formRef.value.validate();
    if (!isValid) return;

    if (props.synapseOrganization) {
        emit('editSynapseOrganization', state.form as UpdateWorkspaceSynapseOrganization); // Editing existing app
    } else {
        emit('addSynapseOrganization', state.form as CreateWorkspaceSynapseOrganization); // Creating new app
    }
}

const formRef = ref<FormRef>();

</script>

<template>
  <DsModal :model-value="props.show" @close="$emit('close')">
    <DsModalProgressCard :total-steps="1" :current-step="1">
    <div class="flex w-full py-8 bg-space justify-center items-center">
      <div class="flex w-full items-center justify-center">
        <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center">
          <MeldRxProvider/>
        </div>
      </div>
      <div class="flex-1"/>
    </div>
    <div class="flex flex-col w-full py-5 px-10">
      
      <div class="w-full flex-col justify-center items-center gap-5 flex">
        <DsText size="2xl" weight="light">
          Synapse Organization Details
        </DsText>

        <DsText size="sm" weight="light">
          Provide the details of the organization you want ot connect with.
        </DsText>
      </div>
      <!-- Synapse Organization Details -->
      <DsForm ref="formRef">
        <div class="flex flex-col space-y-2">
            
          <!-- Organization Name -->
          <DsTextInput
              v-model="state.form.organizationName"
              required
              :rules="[[v => !!v, 'Organization Name is required']]"
              type="text"
              label="Organization Name"
          />
  
          <!-- Organization Type -->
          <DsSelect
              v-model="state.form.type"
              :items="synapseOrganizationTypes"
              required
              :rules="[[v => !!v, 'Organization Type is required']]"
              label="Organization Type"
          />
          
          <DsLabeledInput label="Include Address Information" required>
            <DsToggle v-model="state.showAddress" />
          </DsLabeledInput>
  
          <div v-if="state.showAddress">
            <!-- Address Line 1 -->
            <DsTextInput
                v-model="state.form.addressLine1"
                type="text"
                label="Address Line 1"
            />
    
            <!-- Address Line 2 -->
            <DsTextInput
                v-model="state.form.addressLine2"
                type="text"
                label="Address Line 2"
            />
    
            <!-- City -->
            <DsTextInput
                v-model="state.form.city"
                type="text"
                label="City"
            />
    
            <!-- State -->
            <DsTextInput
                v-model="state.form.state"
                type="text"
                label="State"
            />
    
            <!-- Postalcode -->
            <DsTextInput
                v-model="state.form.postalcode"
                class="pb-3"
                type="text"
                label="Postalcode"
            />
          </div>
          
          <DsDivider />
  
          <!-- External App Selector -->
          <DsSelect
              v-model="state.form.externalAppId"
              class="pt-3"
              :items="externalAppOptions"
              required
              :rules="[[v => !!v, 'External App is required']]"
              label="External App"
          />
  
          <!-- FHIR Endpoint -->
          <DsTextInput
              v-model="state.form.fhirEndpoint"
              required
              :rules="[[v => !!v, 'FHIR Endpoint is required']]"
              type="text"
              label="FHIR Endpoint"
          />
          <DsText size="xs" weight="light">
            Note: the Authorize and Token endpoints will be extracted from the metadata of this endpoint
          </DsText>

        </div>
      </DsForm>

      <DsDivider/>

      <!-- Buttons -->
      <div class="flex justify-center w-full gap-5">
        <DsButton variant="outline" :color="Colors.secondary" :text-color='Colors.secondary' @click="$emit('close')">
          Close
        </DsButton>
        <DsButton :color="Colors.secondary" @click="onSubmit">
          {{ props.synapseOrganization ? 'Update Connected Organization' : 'Add Connected Organization' }}
        </DsButton>
      </div>
    </div>
    </DsModalProgressCard>
  </DsModal>
</template>
