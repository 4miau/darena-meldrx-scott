<script setup lang="ts">
import type { Bundle} from 'fhir/r4';
import { Colors } from '~/types/ui/colors'

definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    refreshWorkspace: true,
    allowLinked: true,
});
useHead({ title: 'Upload Sample Data | MeldRx' })

const { $api } = useNuxtApp()

const route = useRoute()
const { workspace, loadingWorkspace } = useWorkspace()
const runtimeConfig = useRuntimeConfig();

const folderSampleData = 'sample_data';
// Get workspace ID from route...
const workspaceSlug = ref<string>(route.params.workspaceSlug as string)
const isLoading = ref<boolean>(false)
const availableSampleData = ref<Record<string, string[]>>({});
const selectedSampleData = ref<string>('');
const uploadProgressStatus = ref<string>('');

function formatString(input: string): string {
    return input.split('_').join(' ');
}

function constructFileName(folder:string,sampleData:string, fileName:string): string {
    return `${runtimeConfig.public.storageUrl}/${folder}/${sampleData}/${fileName}`;
}

function initializeUploadProgress(fileList: string[]) {
    uploadProgressStatus.value = fileList.map(file => `${file}: Pending`).join('\n');
}

function updateUploadProgress(fileName: string, status: 'Success' | 'Failed' | 'Pending') {
    const progressLines = uploadProgressStatus.value.split('\n').filter(line => line.trim() !== '');
    const updatedLines = progressLines.map(line =>
        line.startsWith(fileName) ? `${fileName}: ${status}` : line
    );
    if (!updatedLines.some(line => line.startsWith(fileName))) {
        updatedLines.push(`${fileName}: ${status}`);
    }
    uploadProgressStatus.value = updatedLines.join('\n');
}

// Fetch available sample data from public blob storage
async function fetchSampleData() {
    try {
        availableSampleData.value = await $api.workspaces.getAvailableSampleData();
    } catch {
        notification({ title: 'Error', description: `Error fetching or parsing the data`, displayTime: 3000, variant: 'error' });
    }
}

// Upload selected sample data to the workspace
async function uploadSampleData() {
    if (!selectedSampleData.value) {
        notification({ title: 'Error', description: `No sample data selected for import.`, displayTime: 3000, variant: 'error' });
        return;
    }

    const fileList = availableSampleData.value[selectedSampleData.value] || [];
    if (fileList.length === 0) {
        notification({ title: 'Error', description: `No files available for the selected sample data.`, displayTime: 3000, variant: 'error' });
        return;
    }

    try{
        initializeUploadProgress(fileList);
        isLoading.value = true;
        const groupedFiles = fileList.reduce((acc, fileName) => {
            const fileExtension = fileName.split('.').pop()?.toLowerCase();
            if (!fileExtension) return acc;
            if (!acc[fileExtension]) {
                acc[fileExtension] = [];
            }
            acc[fileExtension].push(fileName);
            return acc;
        }, {} as Record<string, string[]>);

        for (const [groupKey, fileList] of Object.entries(groupedFiles)) {
            if(groupKey === 'xml') {
                for (const fileName of fileList) {
                    const fileUrl = constructFileName(folderSampleData, selectedSampleData.value, fileName);
                    try {
                        const fileContents = await $fetch<string>(fileUrl, { method: 'GET' });

                        updateUploadProgress(fileName, 'Pending');
                        await $api.dataImport.importCcda(workspaceSlug.value, fileContents);
                        updateUploadProgress(fileName, 'Success');
                    } catch (error) {
                        handleApiError(error, `Unable to import ${fileUrl}`);
                        updateUploadProgress(fileName, 'Failed');
                        continue;
                    }
                }
            } else if(groupKey === 'json') {
                const numberedFiles = fileList.filter(fileUrl => /^\d+_/.test(fileUrl));
                if (numberedFiles.length > 0) {
                    const uniqueNumbers = Array.from(
                        new Set(numberedFiles.map(file => parseInt(file.split('_')[0], 10)))
                    );
                    uniqueNumbers.sort((a, b) => a - b);

                    for (const number of uniqueNumbers) {
                        const filteredFiles = fileList.filter(file => file.startsWith(`${number}_`));

                        await Promise.all(
                            filteredFiles.map(async (fileName) => {
                                try {
                                    const fileUrl = constructFileName(folderSampleData, selectedSampleData.value, fileName);
                                    const fileContents = await $fetch<Bundle>(fileUrl, { method: 'GET' });

                                    updateUploadProgress(fileName, 'Pending');
                                    await $api.fhir.uploadBundle(workspaceSlug.value, fileContents);
                                    updateUploadProgress(fileName, 'Success');
                                } catch (error) {
                                    updateUploadProgress(fileName, 'Failed');
                                    handleApiError(error, `Unable to import ${fileName}`);
                                }
                            })
                        );
                    }
                } else {
                    await Promise.all(
                        fileList.map(async (fileName) => {
                            const fileUrl = constructFileName(folderSampleData, selectedSampleData.value, fileName);
                            try {
                                const fileContents = await $fetch<Bundle>(fileUrl, { method: 'GET' });
                                updateUploadProgress(fileName, 'Pending');
                                await $api.fhir.uploadBundle(workspaceSlug.value, fileContents);
                                updateUploadProgress(fileName, 'Success');
                            } catch (error) {
                                updateUploadProgress(fileName, 'Failed');
                                handleApiError(error, `Unable to import ${fileUrl}`);
                            }
                        })
                    );
                }
            }
        }
        notification({ title: 'Success', description: `${formatString(selectedSampleData.value)} imported successfully`, displayTime: 3000, variant: 'success' });
    } catch (error) {
        handleApiError(error, `Error while uploading sample data`);
    } finally {
        isLoading.value = false;
    }
}

fetchSampleData();

</script>
<template>
  <DsContainer>
    <DsLoadingOverlay :loading="isLoading || loadingWorkspace" />
    <div v-if="workspace" class="mb-5">
      <DsText size="2xl" weight="light" class="block">
        Sample Data
      </DsText>
      <DsText size="xs" weight="light">
        {{ workspace.name }}
      </DsText>
      <div class="mt-5">
        <DsText size="md" weight="light">
          Populate your workspace with dummy data for testing purposes.
        </DsText>
      </div>
    </div>

    <div v-if="workspace" class="w-[1100px]">
      <!-- Workspace Actions -->
      <div class="grid grid-cols-12 gap-14">
        <!-- Workspace Type Description -->
        <div class="col-span-4">
          <DsLabeledText
            label="Select a sample data set"
            text="Select a sample data type (CCDAs or FHIR JSON bundles) and the data size you want to import to your workspace."
          />
        </div>

        <!-- Workspace Type Buttons -->
        <div class="col-span-8">
          <div class="space-y-2">
            <DsSelect
                :required="true"
                :model-value="selectedSampleData"
                :items="Object.keys(availableSampleData).map(key => ({ label: key, value: key, title: formatString(key) }))"
                label="Select a sample data set to import"
                :searchable="false"
                :rules="[[(v) => !!v, 'Data selection is required']]"
                @update:model-value="selectedSampleData = $event"
            />
            <DsButton :color="Colors.secondary" :disabled="!selectedSampleData" @click="uploadSampleData">
              Import Data
            </DsButton>
            <!-- Upload Status -->
            <pre>{{ uploadProgressStatus }}</pre>
          </div>
        </div>
      </div>
    </div>
  </DsContainer>
</template>
