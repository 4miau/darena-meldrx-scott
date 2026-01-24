<script setup lang="ts">
import type { Condition } from 'fhir/r4';

const props = defineProps<{
    resources: Condition[];
    patientId: string;
}>();

const sectionTitle = "Condition";

const dataExtract = props.resources.map(r => ({
    Id: r.id || '',
    Type: r.code?.coding?.[0]?.display ?? 'No Condition Code',
    ClinicalStatus: r.clinicalStatus?.coding?.[0].display ?? 'No status available',
    Category: r.category?.[0].coding?.[0].display ?? 'No category available',
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