<!-- eslint-disable no-console -->
<script setup lang="ts">
import { ref } from 'vue';
import { getDefaultPatientDto, type PatientDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PatientDto';
useHead({ title: 'Component Gallery | MeldRx' });

const openModalBase = ref(false);
const openModalBaseWithProgress = ref(false);
const totalSteps = ref<string>('3');
const currentStep = ref<string>('1');

const openAddPatientModal = ref(false);

const patientDto = ref<PatientDto>(getDefaultPatientDto());
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        Modals
      </DsText>
      <div class="pb-5" />

      <!-- DsModal -->
      <DsText size="xl">
        DsModal
      </DsText>
      <DsModal :model-value="openModalBase" @close="() => openModalBase = false">
        <p>DsModal Content 1</p>
        <p>DsModal Content 2</p>
        <p>DsModal Content 3</p>
      </DsModal>
      <DsButton variant="filled" @click="openModalBase = true">
        Open Modal
      </DsButton>
      <DsDivider />

      <!-- DsModal (with progress indicator) -->
      <DsText size="xl">
        DsModal (with progress indicator)
      </DsText>
      <DsModal :model-value="openModalBaseWithProgress" @close="() => openModalBaseWithProgress = false">
        <DsModalProgressCard :total-steps="parseInt(totalSteps)" :current-step="parseInt(currentStep)">
          <pre>

            Current Step: {{ currentStep }}
            Total Steps: {{ totalSteps }}
           </pre>
        </DsModalProgressCard>
      </DsModal>
      <DsButton variant="filled" @click="openModalBaseWithProgress = true">
        Open Modal
      </DsButton>
      <DsTextInput v-model:model-value="totalSteps" label="Total Steps" />
      <DsTextInput v-model:model-value="currentStep" label="Current Step" />
      <DsDivider />

      <!-- AddPatientModal -->
      <DsText size="xl">
        AddPatientModal
      </DsText>
      <AddPatientModal
        v-model="patientDto"
        :show="openAddPatientModal"
        :send-patient-data-request="false"
        mode="add"
        :is-loading="false"
        :active="openAddPatientModal"
        @close="() => openAddPatientModal = false"
      />
      <DsButton @click="openAddPatientModal = true">
        Open Add Patient Modal
      </DsButton>
      <DsDivider />
    </div>
  </DsContainer>
</template>
