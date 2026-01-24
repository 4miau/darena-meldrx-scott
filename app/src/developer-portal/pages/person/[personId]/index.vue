<script setup lang="ts">
import type { Patient } from 'fhir/r4'
import DsLoadingOverlay from '~/components/ui/DsLoadingOverlay.vue'
import PatientDocumentViewer from '~/components/patients/PatientDocumentViewer.vue'
import PatientAllDataViewer from '~/components/patients/PatientAllDataViewer.vue'
import PatientInfoCard from '~/components/patients/PatientInfoCard.vue'
import PatientOverview from '~/components/patients/PatientOverview.vue'
import { Colors } from '~/types/ui/colors'
import { externalNavigation } from '#imports'
import type { PersonMetadata } from '~/composables/useAuth'
import type { WorkspaceDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto'

definePageMeta({ layout: 'blank' })
useHead({ title: 'Patient Chart | MeldRx' })

const router = useRouter()
const { $api } = useNuxtApp()
const { people } = useAuth()


const state = reactive<{
  person?: PersonMetadata;
  patient?: Patient;
  page: number;
  isLoading: boolean;
  currentTab: number;
  patientWorkspace?: { patientId: string, workspace: WorkspaceDto };
}>({
    page: 1,
    isLoading: false,
    currentTab: 0,
})

const tabs = [
    {
        title: 'Overview',
        component: PatientOverview,
        isProvider: null,
    },
    {
        title: 'Documents',
        component: PatientDocumentViewer,
        isProvider: 'isProvider',
    },
    {
        title: 'All Data',
        component: PatientAllDataViewer,
        isProvider: null,
    },
]

const selectedTab = computed(() => tabs.at(state.currentTab))

const workspaceOptions = computed(() =>
    (state.person && state.person.patients.length > 1)
        ? state.person.patients.map(x => ({
            value: x.workspace.id,
            title: x.workspace.name
        }))
        : []
)

async function loadPatient (patientWorkspace: { patientId: string, workspace: WorkspaceDto }) {
    state.isLoading = true
    try {
        // set to undefined before async call to refresh ui
        state.patientWorkspace = undefined

        state.patient = await $api.fhir.getResourceById<Patient>(
            patientWorkspace.workspace.fhirDatabaseDisplayName,
            'Patient',
            patientWorkspace.patientId
        )

        state.patientWorkspace = patientWorkspace
    } catch (error) {
        handleApiError(error, 'Failed to load patient information')
    } finally {
        state.isLoading = false
    }
}

async function sendSynapsePackage () {
    if (!state.person || state.isLoading || !state.patientWorkspace) {
        return
    }

    state.isLoading = true
    try {
        const scheme = 'CmsBlueButtonOidc'
        const result = await $api.synapse.create(
            {
                description: 'New Package',
                workspaceId: state.patientWorkspace.workspace.id,
                patientId: state.patientWorkspace.patientId,
                downloadReturnUrl: location.href,
                scheme
            }
        )

        return externalNavigation.synapsePackage(result.id, result.downloadInfo.downloadContextToken, scheme)
    } catch (error) {
        handleApiError(error, 'Failed to load patient information')
        state.isLoading = false
    }
}

async function selectWorkspace(id: string) {
    const newWorkspace = state.person?.patients.find(p => p.workspace.id === id)
    if (!newWorkspace) {
        return
    }

    await loadPatient(newWorkspace)
}

const personId =router.currentRoute.value.params.personId
if (!personId || typeof personId === 'object') {
    navigateTo('/')
}

state.person = people.value.find(x => x.person.id === personId)
if (!state.person) {
    throw showError({ statusCode: 404 })
}

loadPatient(state.person.patients[0])

</script>

<template>
  <DsLoadingOverlay :loading="state.isLoading"/>

  <template v-if="state.person && state.patient && state.patientWorkspace">


    <div class="flex flex-row m-5 gap-5">

      <!-- Switch selected person -->
      <div v-if="people.length > 1" class="flex flex-col items-center justify-around bg-bliss border border-silver rounded-sm p-2">
        <div>
          <DsText size="sm" weight="light">
            {{ people.length }} People Available
          </DsText>
        </div>
        <div>
          <DsButton size="xs" :color="Colors.white" :text-color='Colors.onyx' variant="outline" @click="externalNavigation.changeContext">
            <DsIcon name="heroicons:arrows-right-left" size='sm' />
            Switch Person
          </DsButton>
        </div>
      </div>

      <!-- Switch selected workspace -->
      <div v-if="workspaceOptions.length > 0" class="flex flex-col items-center justify-around bg-bliss border border-silver rounded-sm p-2">
        <div>
          <DsText size="sm" weight="light">
            VIEWING DATA FOR ORGANIZATION
          </DsText>
        </div>
        <div>
          <DsSelect
              :model-value="state.patientWorkspace.workspace.id"
              :items="workspaceOptions"
              @update:model-value="selectWorkspace"
          />
        </div>
      </div>

      <!-- Patient Card -->
      <PatientInfoCard :patient="state.patient"/>

      <div class="flex flex-col items-center justify-around bg-bliss border border-silver rounded-sm p-2">
        <div>
          <DsText size="sm" weight="light">
            SEND CMS DATA TO MY PROVIDER
          </DsText>
        </div>
        <div>
          <DsButton
              size="xs"
              :color="Colors.white"
              :text-color='Colors.gray'
              variant="outline"
              @click="sendSynapsePackage"
          >
            <DsIcon name="heroicons:arrow-right"/>
            Send Data
          </DsButton>
        </div>
      </div>

    </div>


    <!-- Navigation Tabs -->
    <div class="border rounded-t-md border-silver m-5">

      <!-- Tab Headers -->
      <div class="flex rounded-t-md border-b border-silver space-x-5 bg-bliss px-5">
        <DsTab
            :labels="tabs.map(tab => tab.title)"
            :selected-tab="state.currentTab"
            @click="state.currentTab = $event"
        />
      </div>

      <!-- Tab Contents -->
      <div v-if="selectedTab" class="p-5">
        <component
            :is="selectedTab.component"
            :workspace-slug="state.patientWorkspace.workspace.fhirDatabaseDisplayName"
            :patient-id="state.patientWorkspace.patientId"
            :[selectedTab.isProvider!]="false"
        />
      </div>
    </div>
  </template>
</template>
