<script setup lang="ts">
import type { WorkspaceProviderDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceProviderDto'
import { Colors } from '~/types/ui/colors'
import DateTime from '~/utils/DateTime'
import type { UpdateWorkspaceProviderInfoCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceProviders/UpdateWorkspaceProviderInfoCommand'
import type { Guid } from '~/types/common/Guid'
import type { DirectorySelectorView } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDirectory/DirectorySelectorView'

const props = defineProps<{
  provider?: WorkspaceProviderDto
  directories: DirectorySelectorView[];
}>()

const emit = defineEmits<{
  'close': [];
  'update': [UpdateWorkspaceProviderInfoCommand];
}>();

const formRef = ref<FormRef>()
const state = reactive<{
  form: {
    displayName: string;
    dateOfActivation: string;
    directoryIds: Guid[];
  }
}>({
    form: {
        displayName: '',
        dateOfActivation: '',
        directoryIds: []
    }
})

watch(
    () => props.provider,
    (v) => {
        if (v) {
            state.form.displayName = v.displayName
            state.form.dateOfActivation = DateTime.toISODateString(v.dateOfActivation)
            state.form.directoryIds = v.directoryIds
        }
    }
)

function save() {
    const isFormValid = formRef.value?.validate();
    if (!isFormValid) {
        return;
    }

    emit('update', {
        providerId: props.provider!.id,
        displayName: state.form.displayName,
        dateOfActivation: DateTime.toISODateString(state.form.dateOfActivation),
        directoryIds: props.directories.length === 1 ? [props.directories[0].id] : state.form.directoryIds,
    });
}

</script>

<template>
  <DsModal :model-value="!!provider" @close="$emit('close')">
    <DsModalProgressCard :total-steps="1" :current-step="1">
      <div class="flex py-8 bg-space justify-center items-center">
        <MeldRxProvider />
      </div>

      <div class="space-y-5 p-4">
        <div class="text-center">
          <DsText size="2xl" weight="light">
            Edit Provider
          </DsText>
        </div>

        <div class="text-center">
          <DsText size="sm" weight="light" class="text-center">
            Edit how the provider's name will be displayed in the directory.
          </DsText>
        </div>

        <div class="space-y-5 mx-5">
          <DsForm ref="formRef">
            <DsTextInput
              v-model="state.form.displayName"
              required
              :rules="ValidationRules.providerName"
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
            <DsButton :color='Colors.white' :text-color='Colors.onyx' variant="transparent" @click="$emit('close')">
              Cancel
            </DsButton>
            <DsButton :color="Colors.secondary" @click="save">
              Edit Provider
            </DsButton>
          </div>
        </div>
      </div>
    </DsModalProgressCard>
  </DsModal>
</template>
