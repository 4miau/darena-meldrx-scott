<script setup lang="ts">
import {getDocument, GlobalWorkerOptions} from 'pdfjs-dist';
import {Colors} from "~/types/ui/colors";

if(!GlobalWorkerOptions.workerSrc){
    GlobalWorkerOptions.workerSrc = new URL('pdfjs-dist/build/pdf.worker.mjs', import.meta.url).toString();
}

const props = defineProps<{
  url: string
}>();

const pdfCanvas = ref<HTMLCanvasElement>();

const state = reactive<{
  totalPages: number;
  currentPage: number;
  loading: boolean;
  error: string;
}>({
    totalPages: 1,
    currentPage: 1,
    loading: false,
    error: "",
})

const stateNonReactive: {
  document: any
} = {
    document: null
}

async function renderPage(pageNum: number) {
    try {
        const page = await stateNonReactive.document.getPage(pageNum);
        const scale = 1.5;
        const viewport = page.getViewport({scale});
        if (pdfCanvas.value) {
            pdfCanvas.value.width = viewport.width;
            pdfCanvas.value.height = viewport.height;

            const renderContext = {
                canvasContext: pdfCanvas.value.getContext('2d'),
                viewport: viewport,
            };

            await page.render(renderContext).promise;
        }
    } catch (err) {
        state.error = 'Error rendering page: ' + (err as Error).message;
    }
}

async function loadPdf() {
    state.loading = true;
    state.error = "";

    try {
        const loadingTask = getDocument({
            url: props.url,
            withCredentials: true,
            httpHeaders: {
                'Accept': 'application/pdf'
            }
        });
        stateNonReactive.document = await loadingTask.promise;
        state.totalPages = stateNonReactive.document.numPages
        await renderPage(state.currentPage = 1);
    } catch (err) {
        state.error = 'Error loading PDF: ' + (err as Error).message;
    } finally {
        state.loading = false;
    }
};

function onPrevPage() {
    if (state.currentPage <= 1) {
        return;
    }
    renderPage(--state.currentPage);
}

function onNextPage() {
    if (state.currentPage >= state.totalPages) {
        return;
    }
    renderPage(++state.currentPage);
}

watch(
    () => props.url,
    loadPdf,
    {immediate: true}
)
</script>

<template>
  <div>
    <canvas v-show="!state.loading" ref="pdfCanvas"/>
    <div v-if="state.loading">
      <DsLoadingSpinner :loading="state.loading"/>
      Loading PDF...
    </div>
    <div v-if="state.totalPages > 1" class="flex justify-center space-x-1">
      <DsButton id="previous-page-button" :color="Colors.secondary" @click="onPrevPage()">
        Previous Page
      </DsButton>
      <DsButton id="next-page-button" :color="Colors.secondary" @click="onNextPage()">
        Next Page
      </DsButton>
    </div>
  </div>
</template>

