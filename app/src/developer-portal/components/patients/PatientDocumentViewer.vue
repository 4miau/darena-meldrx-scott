<script setup lang="ts">
import type {BundleEntry, DocumentReference, FhirResource} from 'fhir/r4';
import {Colors} from "~/types/ui/colors";
import type {UploadDocumentDto} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/UploadDocumentDto';
import DsRenderDocumentReference from '~/components/document-viewer/DsRenderDocumentReference.vue'

const {$api} = useNuxtApp();
const confirmation = useConfirmation()

const props = defineProps<{
  workspaceSlug: string;
  patientId: string;
  isProvider: boolean;
}>();

const state = reactive<{
  selectedDocument?: DocumentReference;
  isLoading: boolean;
  isLoadingMoreDocuments: boolean;
  openUploadDocumentModal: boolean;
  documents: DocumentReference[];
  nextPage?: string;
  totalDocuments: number;
}>({
    isLoading: false,
    isLoadingMoreDocuments: false,
    openUploadDocumentModal: false,
    documents: [],
    totalDocuments: 0,
})

// Allowed patient document uploads
const allowedDocumentUploads = [
    {fileExtension: ['.jpg', '.png', '.jpeg'], fileDescription: 'Profile Pictures'},
    {fileExtension: ['.pdf', '.json', '.xml', '.txt', '.csv', '.doc', '.docx', '.bmp', '.tif', '.rtf', '.mp4', '.mpg', '.avi'], fileDescription: 'Clinical Documents'}
];

// CSS classes for selected and unselected document...
const classSelectedDocument = 'hover:bg-bliss bg-silver cursor-pointer';
const classUnselectedDocument = 'hover:bg-bliss bg-white cursor-pointer';

// Handle scroll events on document selector
async function loadMoreDocuments(event: Event) {
    const target = event.target as HTMLElement;
    const {scrollTop, scrollHeight, clientHeight} = target;
    if (scrollTop + clientHeight < scrollHeight - 5) {
        return
    }

    if(!state.nextPage){
        return;
    }

    if (state.isLoadingMoreDocuments) {
        return;
    }

    state.isLoadingMoreDocuments = true;
    try {
        const bundle = await $api.fhir.getLink<DocumentReference>(state.nextPage);
        if (!bundle || !bundle.entry) {
            return;
        }

        const newDocuments = bundle.entry
            .map((entry: BundleEntry) => entry.resource)
            .filter((resource?: FhirResource) => !!resource && resource.resourceType === 'DocumentReference') as DocumentReference[];

        state.documents.push(...newDocuments)

        state.nextPage = bundle.link?.find(link => link.relation === 'next')?.url;

    } catch (error) {
        handleApiError(error, 'Unable to get document list');
    } finally {
        state.isLoadingMoreDocuments = false;
    }
}

// Load additional patient data, with incremental pagination calls
async function loadDocuments() {
    state.isLoading = true;
    try {
        const bundle = await $api.fhir.getResourcesByPatientId<DocumentReference>(props.workspaceSlug, 'DocumentReference', props.patientId);
        if (!bundle || !bundle.entry) {
            return;
        }

        state.documents = bundle.entry
            .map((entry: BundleEntry) => entry.resource)
            .filter((resource?: FhirResource) => !!resource && resource.resourceType === 'DocumentReference') as DocumentReference[];

        state.totalDocuments = bundle.total ?? 0
        state.nextPage = bundle.link?.find(link => link.relation === 'next')?.url;
    } catch (error) {
        handleApiError(error, 'Unable to get document list');
    } finally {
        state.isLoading = false;
    }
}

// Upload Patient Documents
async function onUploadDocument(value: {
  fileContents: ArrayBuffer,
  fileDate: string,
  fileDescription: string,
  fileName: string
}) {
    state.isLoading = true;
    try {
        state.openUploadDocumentModal = false;
        const {fileContents, fileDate, fileDescription, fileName} = value;
        const uploadDoc: UploadDocumentDto = {
            date: fileDate,
            description: fileDescription,
            patientId: props.patientId
        };
        await $api.documents.uploadDocument(props.workspaceSlug, uploadDoc, fileContents, fileName);
        successNotification('Data uploaded successfully')
    } catch (error) {
        handleApiError(error, 'Unable to upload patient document')
    } finally {
        state.isLoading = false;
        await loadDocuments()
    }
}

// Delete Patient Documents
async function onDeleteDocument(documentId: string) {
    if (!documentId) {
        return
    }
    const {isCancelled} = await confirmation("Are you sure you want to delete this document? This action cannot be undone.", "Delete Document")
    if(isCancelled){
        return;
    }

    state.isLoading = true;
    try {
        await $api.fhir.deleteDocument(props.workspaceSlug, documentId);
        successNotification('Data deleted successfully')
    } catch (error) {
        handleApiError(error, 'Unable to delete document')
    } finally {
        state.isLoading = false;
        state.selectedDocument = undefined;
        await loadDocuments();
    }
}

loadDocuments();
</script>

<template>

  <!-- Upload Document Modal -->
  <ImportDataModal
      :show="state.openUploadDocumentModal"
      :show-description-date="true"
      title="Upload Documents"
      description="Upload patient document like clinical notes/reports. Supported types include:"
      :allowed-import-types=allowedDocumentUploads
      read-mode="Binary"
      @close="() => state.openUploadDocumentModal = false"
      @cancel="() => state.openUploadDocumentModal = false"
      @import-data-with-description-date="onUploadDocument"
  />

  <div>
    <div v-if="!state.isLoading"  class="w-full border border-silver flex flex-row">

      <!-- Document Selector -->
      <div v-if="state.documents.length > 0" class="border border-silver max-w-[400px] w-full">

        <!-- Header -->
        <div class="flex bg-bliss border-b border-silver w-full py-2 px-3 justify-between">
          <DsText>
            DOCUMENTS ({{ state.totalDocuments }})
          </DsText>
          <DsButton
              v-if="isProvider"
              id='upload-document-button'
              variant="outline"
              :color="Colors.primary"
              :text-color='Colors.primary'
              padding='px-2'
              size="xs"
              @click="state.openUploadDocumentModal = true;"
          >
            Upload Document
          </DsButton>
        </div>

        <div class="h-[700px] overflow-auto" @scroll="loadMoreDocuments">
          <!-- Document Scroller -->
          <div
              v-for="document in state.documents"
              :key="document.id"
              :class="state.selectedDocument?.id === document.id ? classSelectedDocument : classUnselectedDocument">
            <div
                class="flex flex-row justify-between items-center border-b border-silver"
                @click="state.selectedDocument = document">
              <div class="flex flex-col px-2 py-1">
                <DsText size="md" weight="light">
                  {{ DocumentReferenceExtensions.getDisplayName(document) }}
                </DsText>
                <DsText size="xs" weight="light">
                  TYPE: {{ DocumentReferenceExtensions.getAttachmentDocumentType(document) }}
                </DsText>
                <DsText size="xs" weight="light">
                  CREATED: {{ DocumentReferenceExtensions.getAttachmentCreationDate(document) }}
                </DsText>
              </div>
              <div>
                <DsIcon
                    :class="state.selectedDocument?.id === document.id ? 'text-indigo' : 'text-silver'"
                    name="heroicons:chevron-right" size="lg"/>
              </div>
            </div>
          </div>
          <div v-if="!state.nextPage" class="flex justify-center py-3">
            <DsText size="md" weight="light">
              End of list
            </DsText>
          </div>
          <div v-else class="h-[15px]"/>
        </div>
      </div>

      <!-- Document Viewer and Actions -->
      <div class="border border-silver w-full">

        <!-- Document Actions -->
        <div
            v-if="state.selectedDocument && isProvider"
            class="flex bg-bliss border-b border-silver py-2 px-3 space-x-3 items-center"
        >

          <DsText>
            ACTIONS
          </DsText>

          <DsButton id='delete-document-button' variant="outline" padding='px-2' :color="Colors.fire" :text-color='Colors.fire' size="xs" @click="onDeleteDocument(state.selectedDocument.id!)">
            Delete
          </DsButton>

        </div>

        <!-- Document Render -->
        <div class="w-full overflow-x-hidden overflow-y-visible h-[700px]">
          <div v-if="state.documents.length === 0">
            <div class="bg-bliss border-b border-silver w-full h-[40px]"/>
            <div class="flex flex-col justify-center items-center h-[660px] space-y-2">
              <DsIcon name="heroicons:document" size="xl"/>
              <DsText size="2xl" weight="light" :color="Colors.black">
                There are no documents for this patient.
              </DsText>
              <div>
                <DsButton v-if="isProvider" id='upload-document-button' variant="filled" :color="Colors.primary" @click="state.openUploadDocumentModal = true;">
                  <DsIcon name='heroicons:plus' size='xs' />
                  Upload Document
                </DsButton>
              </div>
            </div>
          </div>
          <DsRenderDocumentReference
              v-else-if="state.selectedDocument"
              :document="state.selectedDocument"
              :workspace-slug="workspaceSlug"
              :patient-id="isProvider ? undefined : patientId"
              />
          <div v-else class="flex flex-col justify-center items-center space-y-2 h-[700px]">
            <DsIcon name="heroicons:document" size="xl"/>
            <DsText size="2xl" weight="light" :color="Colors.black">
              Select a document to display its details here.
            </DsText>
          </div>
        </div>
      </div>
    </div>
  </div>

  <div v-if="state.isLoading" class="flex justify-center m-5 p-5 gap-2">
    <DsText>
      Loading...
    </DsText>
    <DsLoadingSpinner :loading="state.isLoading" />
  </div>
</template>
