<script setup lang="ts">
import type { DocumentReference } from 'fhir/r4'

const {$api} = useNuxtApp()

const props = defineProps<{
  document: DocumentReference;
  workspaceSlug: string;
  patientId?: string;
}>()

const baseError = 'Failed to load document, error: '
const state = reactive<{
  isLoading: boolean,
  error?: string,
  content?: string,
  base64Content?: string,
  urlContent?: string,
}>({
    isLoading: false,
})

const binaryDocRegex = /\/Binary\/([\w\-._]+)/;
const contentType = computed(() => DocumentReferenceExtensions.getAttachmentDocumentType(props.document))

watchEffect(async () => {
    state.isLoading = true

    state.error = undefined;
    state.content = undefined;
    state.base64Content = undefined;
    state.urlContent = undefined;

    const unsupportedTypes = [
        'video/x-msvideo',
        'image/bmp',
        'image/tiff',
        'video/mp4',
        'video/mpeg',
        'video/x-msvideo',
        'application/rtf',
        'text/rtf',
        'text/csv',
    ]


    try {
        if (props.document.content[0] && unsupportedTypes.includes(props.document.content[0].attachment.contentType!)) {
            state.error = baseError + 'Unable to preview this document type.'
        }

        else if (contentType.value === 'xml') {
            if (props.document.content[0].attachment.url) {
                state.urlContent = props.document.content[0].attachment.url
            }
            else {
                state.error = baseError + 'Expected url data when contentType is xml'
            }
        }

        else if (contentType.value === 'text') {
            if (props.document.content[0].attachment.data) {
                state.base64Content = props.document.content[0].attachment.data
            }
            else if (props.document.content[0].attachment.url) {
                await loadContentFromUrl(props.document.content[0].attachment.url)
            }
            else {
                state.error = baseError + 'Expected base64 or url data when contentType is text'
            }
        }

        else if (contentType.value === 'html') {
            if(props.document.content[0].attachment.data){
                state.base64Content = props.document.content[0].attachment.data
            }
            else if (props.document.content[0].attachment.url) {
                await loadContentFromUrl(props.document.content[0].attachment.url)
            }
            else {
                state.error = baseError + 'Expected base64 data or url when contentType is html'
            }
        }

        else if (contentType.value === 'pdf') {
            if(props.document.content[0].attachment.url){
                state.urlContent = props.document.content[0].attachment.url
            } else {
                state.error = baseError + 'Expected url when contentType is pdf'
            }
        }

        else if (contentType.value === 'image') {
            if(props.document.content[0].attachment.url){
                state.urlContent = props.document.content[0].attachment.url
            } else {
                state.error = baseError + 'Expected url when contentType is image'
            }
        }

        else if (contentType.value === 'json') {
            if(props.document.content[0].attachment.data){
                state.content = props.document.content[0].attachment.data
            } else {
                state.error = baseError + 'Expected data when contentType is json'
            }
        } else {
            state.error = baseError + 'unsupported document type'
        }
    } finally {
        state.isLoading = false
    }

    async function loadContentFromUrl(url : string){

        const binaryFileMatch = binaryDocRegex.exec(props.document.content[0].attachment.url ?? '');
        if (binaryFileMatch != null) {
            const id = binaryFileMatch[1];
            const blobContent = await $api.fhir.getBinaryDocument(props.workspaceSlug, id, props.document.content[0].attachment.contentType ?? '');
            if (!blobContent) {
                state.error = baseError + 'no content found';
                return;
            }

            if(contentType.value === 'text' || contentType.value === 'html'){
                state.content = await blobContent.text()
                return;
            }
        }
        state.error = baseError + `failed to load from: ${url}`
    }
})
</script>

<template>
  <div v-if="state.isLoading" class="h-full flex justify-center items-center">
    <DsLoadingSpinner loading />
  </div>
  <div v-else-if="state.error" class="p-4">
    <DsText>{{ state.error }}</DsText>
  </div>
  <DsDocumentViewer
      v-else
      :content="state.content"
      :content-base64="state.base64Content"
      :content-url="state.urlContent"
      :content-type="contentType === 'xml' ? 'ccda' : contentType"
      :patient-id="patientId"
  />
</template>
