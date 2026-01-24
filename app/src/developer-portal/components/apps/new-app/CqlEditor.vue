<!--

    bridge between Vue and React cql-editor ripped out from: https://github.com/darena-solutions/AHRQ-CDS-Connect-Authoring-Tool

    the 2 apps communicate via window.cqlEditor, initialization of this object happens on the react app side since we are waiting for it to load.

    cqlEditor definition:

    {
      // pass the artifact to the react side
      // this should be used to initialize the editor during "app management"
      init(artifact) : void

      // when the react side updates the artifact model
      // the Vue side should pass a callback to this slow in order to receive updates
      onUpdate(artifact) : void
    }

    NOTE: search code for CqlEditorBridge to find the linking
-->

<script setup>
import { Colors } from "~/types/ui/colors";

const { $api } = useNuxtApp();
const config = useRuntimeConfig();


const props = defineProps({
    artifactForm: {
        type: Object,
        default: undefined
    }
});

const emit = defineEmits(['compiled']);

const state = reactive({
    isLoading: true,
    isCompiling: false,
    script: undefined,
    scriptError: undefined,
    artifactForm: undefined
});

onMounted(() => {
    state.script = document.createElement('script');
    state.script.src = config.public.cqlEditorPath;
    state.script.onload = () => {
        const waitForEditor = setInterval(
            () => {
                if (window.cqlEditor === undefined || window.cqlEditor === null) {
                    return;
                }

                state.artifactForm = props.artifactForm === undefined ? artifactSample : props.artifactForm;
                window.cqlEditor.init(state.artifactForm);
                window.cqlEditor.onUpdate = (artifact) => state.artifactForm = artifact;
                clearInterval(waitForEditor);
                state.isLoading = false;
            },
            500
        );
    };

    state.script.onerror = () => {
        state.isLoading = false;
        state.scriptError = "Failed to load cql editor"
    }

    document.head.appendChild(state.script);
});

onUnmounted(() => {
    window.cqlEditor = undefined;
    document.head.removeChild(state.script);
});

async function compile () {
    if (state.isCompiling) {
        return;
    }
    state.isCompiling = true;
    try {
        const compilationResult = await $api.apps.compileCql(state.artifactForm);
        emit('compiled', {
            artifactForm: state.artifactForm,
            compilationResult
        });
    } catch (error) {
        handleApiError(error, 'Unable to compile cql');
    }
    state.isCompiling = false;
}

const artifactSample = {
    'effectivePeriod': {
        'start': null,
        'end': null
    },
    '_id': 'fakeid',
    'name': 'name',
    'version': '1',
    'description': '',
    'url': '',
    'status': null,
    'experimental': null,
    'publisher': '',
    'context': [],
    'purpose': '',
    'usage': '',
    'copyright': '',
    'approvalDate': null,
    'lastReviewDate': null,
    'topic': [],
    'author': [],
    'reviewer': [],
    'endorser': [],
    'relatedArtifact': [],
    'strengthOfRecommendation': {
        'strengthOfRecommendation': null,
        'code': '',
        'system': '',
        'other': ''
    },
    'qualityOfEvidence': {
        'qualityOfEvidence': null,
        'code': '',
        'system': '',
        'other': ''
    },
    'fhirVersion': '',
    'expTreeInclude': {
        'id': 'And',
        'name': 'And',
        'conjunction': true,
        'returnType': 'boolean',
        'fields': [
            {
                'id': 'element_name',
                'type': 'string',
                'name': 'Group Name',
                'value': 'MeetsInclusionCriteria'
            },
            {
                'id': 'comment',
                'type': 'textarea',
                'name': 'Comment'
            }
        ],
        'uniqueId': 'And-3fde953e-f20f-4909-ac51-0ab731ad1c2d',
        'childInstances': [],
        'path': ''
    },
    'expTreeExclude': {
        'id': 'Or',
        'name': 'Or',
        'conjunction': true,
        'returnType': 'boolean',
        'fields': [
            {
                'id': 'element_name',
                'type': 'string',
                'name': 'Group Name',
                'value': 'MeetsExclusionCriteria'
            },
            {
                'id': 'comment',
                'type': 'textarea',
                'name': 'Comment'
            }
        ],
        'uniqueId': 'Or-0d880d02-6d56-433a-acf3-d042a3aef278',
        'childInstances': [],
        'path': ''
    },
    'recommendations': [],
    'subpopulations': [
        {
            'special': true,
            'subpopulationName': 'Doesn\'t Meet Inclusion Criteria',
            'special_subpopulationName': 'not "MeetsInclusionCriteria"',
            'uniqueId': 'default-subpopulation-1'
        },
        {
            'special': true,
            'subpopulationName': 'Meets Exclusion Criteria',
            'special_subpopulationName': '"MeetsExclusionCriteria"',
            'uniqueId': 'default-subpopulation-2'
        }
    ],
    'baseElements': [],
    'parameters': [],
    'errorStatement': {
        'id': 'root',
        'ifThenClauses': [
            {
                'ifCondition': {
                    'label': null,
                    'value': null
                },
                'statements': [],
                'thenClause': ''
            }
        ],
        'elseClause': ''
    },
    'user': 'demo',
    'createdAt': '2024-10-10T12:48:08.903Z',
    'updatedAt': '2024-10-11T11:07:08.690Z',
    '__v': 0
};

</script>

<template>
  <div v-show="!state.isLoading && state.scriptError">
    <DsText :color="Colors.fire">
      {{state.scriptError}}
    </DsText>

    <DevOnly>
      <div>
        Ensure <strong>app/src/cql-editor</strong> is running during development
      </div>
    </DevOnly>
  </div>
  <div v-show="!state.isLoading && !state.scriptError">
    <div id="cql-editor" />
    <div class="flex items-center gap-4">
      <DsButton id='save-cql-button' :color="Colors.secondary" :disabled="state.isCompiling" @click="compile()">
        Save CQL
      </DsButton>
      <DsLoadingSpinner :loading="state.isCompiling" />
    </div>
  </div>
  <DsLoadingSpinner :loading="state.isLoading" />
</template>

<style scoped>

</style>
