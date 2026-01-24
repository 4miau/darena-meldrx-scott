<script setup lang="ts">
import type { FhirResource } from 'fhir/r4';

const props = defineProps<{
    resources: FhirResource[];
    patientId: string;
}>();

const sectionTitle = props.resources[0].resourceType

const dataExtract = props.resources.map(r => ({
    Id: r.id,
    Type: r.resourceType ?? 'No resource type',
    LastUpdated: convertFhirDateStringToLocale(r.meta?.lastUpdated),
})) as { [key: string]: string }[];

function convertFhirDateStringToLocale(dt: string|undefined){
    if (!dt) return 'No last updated date';
    const date = new Date(dt);
    const options: Intl.DateTimeFormatOptions = {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        timeZoneName: 'short'
    };
    return date.toLocaleString(undefined, options);
}

</script>

<template>
    <BaseFhirViewer
        :resources="resources"
        :patient-id="patientId"
        :data-extract="dataExtract"
        :title="sectionTitle"
    />
</template>