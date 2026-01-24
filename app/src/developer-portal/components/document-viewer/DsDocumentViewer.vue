<script setup lang="ts">

import type {DocumentType} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/DocumentType";

const props = defineProps<{
  content?: any;
  contentUrl?: string;
  contentBase64?: string;
  contentType: DocumentType;
  patientId?: string;
}>();
const config = useRuntimeConfig()
if (!config.public.containerEnv) {
    if (props.contentType === "text" && !props.contentBase64 && !props.content) {
        throw Error("Expected contentBase64 or content when contentType is text")
    }
    if (props.contentType === "html" && !props.contentBase64) {
        throw Error("Expected contentBase64 when contentType is html")
    }
    if (props.contentType === "pdf" && !props.contentUrl) {
        throw Error("Expected contentUrl when contentType is pdf")
    }
    if (props.contentType === "image" && !props.contentUrl) {
        throw Error("Expected contentUrl when contentType is image")
    }
    if (props.contentType === "json" && !props.content) {
        throw Error("Expected content when contentType is json")
    }
}

const adjustedContentUrl = computed(() => {
    return props.patientId ? `${props.contentUrl}?patientId=${props.patientId}` : (props.contentUrl || '');
});

function adapter(text: string): string {
    return atob(text);
}


</script>

<template>
  <div class="p-4 w-full h-full">
    <div >
      <div v-if="contentType === 'image'">
        <img :src="adjustedContentUrl" alt="image">
      </div>
      <div v-if="contentType === 'text'">
        <DsText v-if="contentBase64">
          {{ adapter(contentBase64) }}
        </DsText>
        <DsText v-else >
          {{ content }}
        </DsText>
      </div>
      <div v-if="contentType === 'html' && contentBase64">
        <!-- eslint-disable-next-line vue/no-v-html -->
        <div v-html="adapter(contentBase64)"/>
      </div>
      <div v-if="contentType === 'pdf' && contentUrl">
        <div>
          <PdfViewer :url="adjustedContentUrl"/>
        </div>
      </div>
      <div v-if="contentType === 'ccda' && contentUrl">
        <div>
          <CcdaViewer :ccda="contentUrl" :patient-id="patientId"/>
        </div>
      </div>
    </div>
    <div v-if="contentType === 'json' && content" class="h-full overflow-auto">
      <pre><code v-code-highlight class="json">{{ content }}</code></pre>
    </div>
  </div>
</template>
