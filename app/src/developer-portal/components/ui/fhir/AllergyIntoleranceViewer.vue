<script setup lang="ts">
import type { AllergyIntolerance } from 'fhir/r4';

const props = defineProps<{
    resources: AllergyIntolerance[];
    patientId: string;
}>();

const sectionTitle = "Allergy";

const dataExtract = props.resources.map(r => ({
    Id: r.id || '',
    Type: r.code?.coding?.[0]?.display ?? 'No Allergy Code',
    Manifestation: r.reaction?.[0].manifestation?.[0].coding?.[0].display ?? 'No display',
    Severity: r.reaction?.[0].severity ?? 'No severity available'
})) as { [key: string]: string }[];

</script>

<template>
    <BaseFhirViewer
        :resources="resources"
        :patient-id="patientId"
        :data-extract="dataExtract"
        :title="sectionTitle"
    />
</template>
