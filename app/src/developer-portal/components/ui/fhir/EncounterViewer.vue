<script setup lang="ts">
import type { Encounter } from 'fhir/r4';

const props = defineProps<{
    resources: Encounter[];
    patientId: string;
}>();

const sectionTitle = "Encounter";

const dataExtract = props.resources.map(r => ({
    Id: r.id || '',
    Status: r.status || 'No status available',
    Type: r.type?.[0].coding?.[0].display || 'No visit type available',
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
