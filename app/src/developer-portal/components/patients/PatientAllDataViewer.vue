<script setup lang="ts">
import type { FhirResource } from 'fhir/r4';
import AllergyIntoleranceViewer from '~/components/ui/fhir/AllergyIntoleranceViewer.vue'
import ConditionViewer from '~/components/ui/fhir/ConditionViewer.vue'
import EncounterViewer from '~/components/ui/fhir/EncounterViewer.vue'
import TaskViewer from '~/components/ui/fhir/TaskViewer.vue'
import DefaultFhirViewer from '~/components/ui/fhir/DefaultFhirViewer.vue'
import ResourceType from '~/types/fhir/ResourceType';

const { $api } = useNuxtApp();


const props = defineProps<{
	workspaceSlug: string;
	patientId: string;
}>();

//const filterShowResources = ref<FhirResource[]>();
const isLoading = ref<boolean>(false);
const groupedResources = ref<Record<ResourceType, FhirResource[]>>(
    Object.values(ResourceType).reduce((acc, type) => {
        acc[type] = [];
        return acc;
    }, {} as Record<ResourceType, FhirResource[]>)
);

const customViewers: Partial<Record<ResourceType, Component>> = {
    [ResourceType.AllergyIntolerance]: AllergyIntoleranceViewer,
    [ResourceType.Condition]: ConditionViewer,
    [ResourceType.Encounter]: EncounterViewer,
    [ResourceType.Task]: TaskViewer,
};

const componentMap: Record<ResourceType, Component> = Object.values(ResourceType).reduce((map, resourceType) => {
    map[resourceType] = customViewers[resourceType] || DefaultFhirViewer;
    return map;
}, {} as Record<ResourceType, Component>);

function getViewerComponent(type: ResourceType) {
    return componentMap[type] || DefaultFhirViewer;
}

async function loadAllPatientData(){
    isLoading.value = true;
    try {
        const bundle = await $api.fhir.getEverythingByPatientId(props.workspaceSlug, props.patientId);
        if(bundle.entry && bundle.entry.length > 0){
            bundle.entry.forEach(entry => {
                const resourceType = entry.resource!.resourceType;
                if (!groupedResources.value[resourceType]) {
                    groupedResources.value[resourceType] = [];
                }
                groupedResources.value[resourceType].push(entry.resource!);
            });
        }
    } catch (error){
        handleApiError(error, 'Error loading patient data');
    }
    isLoading.value = false;
}

loadAllPatientData();
</script>

<template>

  <div v-if="isLoading" class="flex justify-center m-5 p-5 gap-2">
    <DsText>
      Loading...
    </DsText>
    <DsLoadingSpinner :loading="isLoading" />
  </div>

  <div v-for="(resources, type) in groupedResources" :key="type" class="mb-3">
    <component
        :is="getViewerComponent(type)"
        v-if="resources.length>0"
        :resources="resources"
        :patient-id="patientId"
    />
  </div></template>
