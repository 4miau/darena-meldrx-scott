<script setup>
// WARNING: DO NOT USE TYPESCRIPT HERE

const props = defineProps({
    'ccda': {
        type: String,
        required: true
    },
    'patientId': {
        type: String,
        default: undefined,
        required: false
    }
})

const state = reactive({
    interval: null
})

function loadXml() {
    const ccdaUrl = props.patientId ? props.ccda + `?patientId=${props.patientId}` : props.ccda;
    new Transformation().setXml(ccdaUrl).setXslt('https://cdn.meldrx.com/assets/ccda-viewer/cda.xsl').transform('viewcda')
}

watch(
    () => props.ccda,
    () => {
        if(window._ccda_core && window._ccda_xslt){
            loadXml()
        } else {
            onMounted(() => {
                if (!state.interval) {
                    clearInterval(state.interval)
                }
                state.interval = setInterval(
                    () => {
                        if (window._ccda_core && window._ccda_xslt) {
                            loadXml()
                            clearInterval(state.interval)
                        }
                    },
                    150
                )
            })
        }
    },
    {
        immediate: true
    }
)
</script>

<template>
  <div class="pure-g">
    <div class="pure-u-1">
      <div id="viewcda" class="cdaview"/>
    </div>
  </div>
</template>
