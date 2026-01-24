<script setup lang="ts">
useHead({ title: 'Component Gallery | MeldRx' });

const fhirUrl = ref<string>('');

</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        FHIR URL Validator
      </DsText>
      <div class="pb-5" />

      <DsText size="md" class="underline">
        Valid FHIR URLs:
        <ul>
          <li>
            <DsText size="sm">
              https://fhir.epic.com/interconnect-fhir-oauth/api/FHIR/R4
            </DsText>
          </li>
          <li>
            <DsText size="sm">
              https://launch.smarthealthit.org/v/r4/sim/WzIsIiIsIiIsIkFVVE8iLDAsMCwwLCIiLCIiLCIiLCIiLCIiLCIiLCIiLDAsMV0/fhir
            </DsText>
          </li>
        </ul>
      </DsText>
      <DsDivider />

      <FhirUrlValidator v-model:fhir-url="fhirUrl" />

      <div class="pb-4" />
      <pre>
FHIR URL: {{ fhirUrl }}
      </pre>
      <DsDivider />
    </div>
  </DsContainer>
</template>
