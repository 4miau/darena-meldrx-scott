<script setup lang="ts">
import { Colors } from "~/types/ui/colors";
import type { FhirResource } from 'fhir/r4';

const props = defineProps<{
    resources: FhirResource[];
    patientId: string;
    dataExtract: Array<{ [key: string]: string }>;
    title: string;
}>();

const state = reactive<{
  isCollapsed : boolean
  showFhirModal : boolean
  isLoading : boolean
  jsonDocument : string
}>({
    isCollapsed : false,
    showFhirModal : false,
    isLoading : false,
    jsonDocument : '',
})

const dataColumnNames = [...Object.keys(props.dataExtract[0]).filter(key => key !== 'Id'), ''];

// Shows the modal with Raw Fhir Json resource
function ShowFhirResource(resourceId: string) {
    const resource = props.resources.find(x => x.id === resourceId);
    state.jsonDocument = resource ? JSON.stringify(resource, null, 2) : 'Resource not found';
    state.showFhirModal = true;
}

</script>

<template>
    <!-- Json Resource Viewer Modal -->
    <DsViewer
        v-if="state.showFhirModal"
        @close="() => state.showFhirModal = false"
    >
        <DsDocumentViewer :content="state.jsonDocument" content-type="json"/>
    </DsViewer>

    <!-- Resource Accordion -->
    <div class="border border-silver">
        <div
            class="flex items-center cursor-pointer bg-bliss border-silver p-2 space-x-1"
            @click="state.isCollapsed = !state.isCollapsed"
        >
            <DsIcon
                :name="state.isCollapsed ? 'heroicons:chevron-down' : 'heroicons:chevron-up'"
                size="md"
            />
            <DsText size="sm">
                {{ title }} ({{ dataExtract.length }})
            </DsText>
        </div>
        <div v-if="!state.isCollapsed">
            <div class="border border-silver">
                <table class="w-full text-sm">
                    <th
                        v-for="(header, i) in dataColumnNames"
                        :key="`table-header-${i}`"
                        class="bg-bliss text-start"
                    >
                        <div class="px-6 py-1">
                            <DsText size="sm" weight="normal">
                                {{ header }}
                            </DsText>
                        </div>
                    </th>
                    <tr v-for="item in dataExtract" :key="`fhir-resource-row-${item.Id}`" :row="item.Id as string" class="border-t border-silver">
                        <td v-for="(columnName) in dataColumnNames" :key="columnName" class="p-2">
                            <div>
                                <div v-if="columnName != ''" :class="columnName == 'Actions' ? 'text-end' : 'px-6 text-start'">
                                    <DsText size="sm" weight="light">
                                        {{ item[columnName as keyof typeof item] }}
                                    </DsText>
                                </div>
                                <div v-else class="pr-4 text-end">
                                    <DsButton
                                        :color="Colors.secondary"
                                        :text-color='Colors.secondary'
                                        variant="outline"
                                        size="xs"
                                        @click.stop="ShowFhirResource(item.Id!)"
                                    >
                                      View FHIR
                                    </DsButton>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</template>
