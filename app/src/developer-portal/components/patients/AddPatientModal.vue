<script setup lang="ts">
import { getDefaultPatientDto, type PatientDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PatientDto';
import type { IDsSelectItem } from '~/types/ui/DsSelect';
import { Colors } from '~/types/ui/colors';
import { Gender } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/Gender';

interface IProps {
  show: boolean;
  isLoading: boolean;
  mode: 'add' | 'edit';
  modelValue: PatientDto | undefined;
  sendPatientDataRequest: boolean;
};

// Set default properties...
const props = withDefaults(defineProps<IProps>(), {
    show: false,
    isLoading: false,
    mode: 'add',
    modelValue: getDefaultPatientDto,
    sendPatientDataRequest: false
});

const emit = defineEmits<{
  'update:modelValue': [value: PatientDto];
  'update:sendPatientDataRequest': [value?: boolean];
  'close': [];
  'addPatient': [value: PatientDto];
  'editPatient': [value: PatientDto];
}>();

const genderOptions = ref<IDsSelectItem<Gender>[]>([
    { value: Gender.Male, title: 'Male' },
    { value: Gender.Female, title: 'Female' },
    { value: Gender.Unknown, title: 'Unknown' },
    { value: Gender.Other, title: 'Other' }
]);

// "Add/Edit Patient"...
function onAddPatientClick() {
    // Try to validate the form...
    if (!formRef.value) { return; }
    const isFormValid = formRef.value.validate();
    if (!isFormValid) { return; }

    if (props.mode === 'add') {
        emit('addPatient', formModel.value);
    }
    else if (props.mode === 'edit') {
        emit('editPatient', formModel.value);
    }
    emit('close')
}

const formRef = ref<FormRef>();
const formModel = ref<PatientDto>(props.modelValue);

watch(() => props.show, (v) => {
    if (v) {
        formModel.value = props.modelValue
    }
})

const headerText = computed(() => {
    return props.mode === 'edit' ? 'Edit Patient' : 'Add New Patient';
});
</script>

<template>
  <DsModal :model-value="show" @close="$emit('close')">
    <DsModalProgressCard :total-steps="1" :current-step="1">
      <DsLoadingOverlay :loading="props.isLoading" />

      <DsForm ref="formRef">
        <!-- Icon -->
        <div class="flex w-full py-8 bg-space justify-center items-center">
          <div class="flex w-full items-center justify-center">
            <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center">
              <MeldRxPatient />
            </div>
          </div>
          <div class="flex-1" />
        </div>

        <div class="flex w-full p-4">
          <div class="flex flex-col w-full">
            <!-- Heading -->
            <div class="w-full flex-col justify-center items-center gap-5 flex">
              <DsText size="2xl" weight="light">
                {{ headerText }}
              </DsText>

              <DsText size="sm" weight="light" class="text-center">
                Provide details to add or invite the patient.
              </DsText>
            </div>

            <div class="grid grid-cols-1 gap-4">
              <!-- First Name -->
              <DsTextInput
                v-model="formModel.firstName"
                :required="true"
                :rules="[[v => !!v, 'First Name is required']]"
                type="text"
                label="First Name"
              />

              <!-- Last Name -->
              <DsTextInput
                v-model="formModel.lastName"
                :required="true"
                :rules="[[v => !!v, 'Last Name is required']]"
                type="text"
                label="Last Name"
              />

              <!-- Gender -->
              <DsSelect v-model="formModel.gender" label="Sex" :items="genderOptions" />

              <!-- Date of Birth -->
              <DsLabeledInput label="Date of birth" :required="true">
                <div class="justify-start items-start inline-flex w-full">
                  <DsDatePicker
                    v-model="formModel.dateOfBirth"
                    :past-only="true"
                    :required="true"
                    :rules="[
                      [v => !!v, 'Date of birth is required']
                      ,[v => !isNaN(Date.parse(v as string)), 'Date of birth is not valid']
                      ,[v => { const today = new Date(); const inputDate = new Date(v as string); return inputDate < today;}, 'Date of birth must be in the past']
                    ]"
                  />
                </div>
              </DsLabeledInput>

              <!-- Email Address -->
              <DsTextInput
                v-model="formModel.emailAddresses"
                :rules="ValidationRules.email"
                type="email"
                label="Email Address"
              />
            </div>

            <!-- Buttons -->
            <DsContainer>
              <div class="flex justify-center w-full gap-5">
                <DsButton id="cancel-button" :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" @click="$emit('close')">
                  Cancel
                </DsButton>
                <DsButton :color="Colors.secondary" @click="onAddPatientClick">
                  {{ props.mode === 'add' ? 'Add Patient' : 'Save Changes' }}
                </DsButton>
              </div>
            </DsContainer>
          </div>
        </div>
      </DsForm>
    </DsModalProgressCard>
  </DsModal>
</template>
