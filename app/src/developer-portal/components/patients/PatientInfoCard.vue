<script setup lang="ts">
import type {Patient} from 'fhir/r4';

defineProps<{
  patient: Patient
}>();
</script>

<template>
  <div class="flex items-center bg-bliss border border-silver rounded-sm p-2">
    <DsAvatar
        :alt="PatientUtils.formatName(patient)[0].toUpperCase()"
        size="md"
        class="mr-3 border border-silver bg-white text-silver"
    />
    <div>
      <DsText size="xl">
        {{ PatientUtils.formatName(patient) }}
      </DsText>
      <div class="flex flex-row space-x-3">
        <DsText size="sm">
          SEX:
          <DsText size="sm" weight="light" class="capitalize"> {{ PatientUtils.formatSex(patient) }}</DsText>
        </DsText>
        <DsText size="sm">
          AGE:
          <DsText size="sm" weight="light">
            {{ PatientUtils.formatAge(patient) }}
          </DsText>
        </DsText>
        <DsText size="sm">
          DATE OF BIRTH:
          <DsText size="sm" weight="light">
            {{ PatientUtils.formatDateOfBirth(patient) }}
          </DsText>
        </DsText>
      </div>
    </div>
  </div>
</template>
