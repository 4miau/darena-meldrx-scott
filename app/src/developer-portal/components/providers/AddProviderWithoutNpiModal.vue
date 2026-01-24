<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import MeldRxProvider from '~/components/svg/MeldRxProvider.vue'
import DateTime from '~/utils/DateTime'
import type { Guid } from '~/types/common/Guid'
import type { DirectorySelectorView } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDirectory/DirectorySelectorView'
import type { CreateWorkspaceProviderCommand } from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceProviders/CreateWorkspaceProviderCommand";

// Set default properties...
const props = defineProps<{
  show: boolean;
  workspaceSlug: string;
  directories: DirectorySelectorView[];
}>();

const { workspace } = useWorkspace();
const formRef = ref<FormRef>();
const state = reactive<{
  form: {
    npi: string,
    providerName: string,
    dateOfActivation: string;
    directoryIds: Guid[];
  }
}>({
    form: {
        npi: '',
        providerName : '',
        dateOfActivation: DateTime.toISODateString(DateTime.nowDate()),
        directoryIds: []
    }
})

const emit = defineEmits<{
  'close': [];
  'create': [CreateWorkspaceProviderCommand];
}>();

function onAddProviderClick() {
    const isFormValid = formRef.value?.validate();
    if (!isFormValid) {
        return;
    }
    
    emit('create', {
        npi: state.form.npi,
        providerName: state.form.providerName,
        workspaceSlug: props.workspaceSlug,
        dateOfActivation: DateTime.toISODateString(state.form.dateOfActivation),
        directoryIds: props.directories.length === 1 ? props.directories.map(x => x.id) : state.form.directoryIds,
    });
}

</script>

<template>
  <DsModal :model-value="show" @close="$emit('close')">
    <div class="flex border-t-8 border-jasmine py-8 bg-space justify-center items-center">
      <MeldRxProvider />
    </div>

    <div class="space-y-5 p-4">
      <div class="text-center">
        <DsText size="2xl" weight="light">
          Add NPI Without Validation
        </DsText>
      </div>

      <div class="text-center">
        <DsText size="sm" weight="light" class="">
          Provide details for the NPIs to add to this workspace and the directory. <br>
          <strong> This provider will not be validated against the NPPES NPI registry</strong>
        </DsText>
      </div>

      <div class="space-y-5 mx-5">
        <DsLabeledText
          label="Workspace Name"
          :text="`${workspace?.name ?? 'Name not found'}`"
        />
        
        <DsForm ref="formRef">
          <DsTextInput
              v-model="state.form.npi"
              required
              :rules="[
              [v => !!v, 'NPI is required'],
              [v => /^\d{10}$/.test(v!), 'a 10 digit NPI is required']
            ]"
              label="NPI (Single)"
          />
          
          <DsTextInput
              v-model="state.form.providerName"
              required
              :rules="[
              [v => !!v, 'Provider Name is required'],
            ]"
              label="Provider Name"
          />

          <DsLabeledInput label="Date of Activation" required>
            <DsDatePicker
              v-model="state.form.dateOfActivation"
              required
              :rules="[[v => !!v, 'Date of Activation is required']]"
              past-only
            />
          </DsLabeledInput>

          <DsSelect
              v-if="directories.length > 1"
              v-model="state.form.directoryIds"
              label="Location"
              placeholder="Select a location(s)"
              :items="directories.map(x => ({value: x.id, title: x.displayName}))"
              required
              :rules="[
                  [v => v!.length > 0, 'At least one location is required']
              ]"
              multiple
          />
        </DsForm>

        <DsDivider />

        <!-- Buttons -->
        <div class="flex justify-center w-full gap-5">
          <DsButton :color="Colors.white" :text-color='Colors.gray' variant="transparent" @click="$emit('close')">
            Cancel
          </DsButton>
          <DsButton id="add-providers-modal-button" :color="Colors.secondary" @click="onAddProviderClick">
            Add Provider
          </DsButton>
        </div>
      </div>
    </div>
  </DsModal>
</template>
