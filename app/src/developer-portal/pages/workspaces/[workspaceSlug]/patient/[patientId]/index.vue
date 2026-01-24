<script setup lang="ts">
import type { Patient } from 'fhir/r4';
import DsLoadingOverlay from '~/components/ui/DsLoadingOverlay.vue';
import PatientDocumentViewer from '~/components/patients/PatientDocumentViewer.vue';
import PatientAllDataViewer from '~/components/patients/PatientAllDataViewer.vue';
import PatientManageViewer from '~/components/patients/PatientManageViewer.vue';
import PatientInfoCard from '~/components/patients/PatientInfoCard.vue';
import PatientAlertViewer from '~/components/patients/PatientAlertViewer.vue';
import type { Card } from '~/types/cds-hooks/CDSCards';
import { Colors } from '~/types/ui/colors';
import PatientAiPlayground from '~/components/patients/PatientAiPlayground.vue';

definePageMeta({
    layout: 'blank',
    middleware: ['require-workspace'],
    allowLinked: true,
});
useHead({ title: 'Patient Chart | MeldRx' });

const route = useRoute();
const { $api } = useNuxtApp();
const { isLinked } = useAuth();
const { workspace } = useWorkspace();

const patientId = ref<string>(route.params.patientId as string);
const workspaceSlug = ref<string>(route.params.workspaceSlug as string);

const state = reactive<{
    patient?: Patient;
    page: number;
    isLoading: boolean;
    currentTab: number;
    cards: Card[];
}>({
    page: 1,
    isLoading: false,
    cards: [],
    currentTab: 0,
});

const tabs = [
    {
        title: 'Alerts',
        component: PatientAlertViewer,
        enabled: true,
        reloadPatientData: null,
        cards: 'cards',
        feedbackSubmitted: 'feedbackSubmitted',
        isProvider: null,
    },
    {
        title: 'Documents',
        component: PatientDocumentViewer,
        enabled: true,
        reloadPatientData: null,
        cards: null,
        feedbackSubmitted: null,
        isProvider: 'isProvider',
    },
    {
        title: 'All Data',
        component: PatientAllDataViewer,
        enabled: true,
        reloadPatientData: null,
        cards: null,
        feedbackSubmitted: null,
        isProvider: null,
    },
    {
        title: 'Chat',
        component: PatientAiPlayground,
        enabled: true,
        reloadPatientData: null,
        cards: null,
        feedbackSubmitted: null,
        isProvider: null,
    },
    {
        title: 'Manage',
        component: PatientManageViewer,
        enabled: !isLinked(),
        reloadPatientData: 'reloadPatientData',
        cards: null,
        feedbackSubmitted: null,
        isProvider: null,
    },
];

const selectedTab = computed(() => tabs.at(state.currentTab));

async function loadPatient(): Promise<boolean> {
    try {
        state.patient = await $api.fhir.getResourceById<Patient>(
            workspaceSlug.value,
            'Patient',
            patientId.value
        );
        return true;
    } catch (error: any) {
        handleApiError(error, 'Unable to load patient');
    }

    return false;
}

async function loadCards() {
    state.isLoading = true;
    try {
        const cardsResponse = await $api.cdsServices.getCards(
            workspaceSlug.value,
            {
                eventType: 'patient-view',
                patientId: patientId.value,
            }
        );
        state.cards = Object.values(cardsResponse.success).flatMap(
            (x) => x.cards || []
        );

        for (const errKey in cardsResponse.error) {
            notification({
                title: `${errKey} CDS Hook Error`,
                description: cardsResponse.error[errKey],
                displayTime: 5000,
                variant: 'error',
            });
        }
    } catch (error) {
        handleApiError(error, 'Unable to get cards list');
    } finally {
        state.isLoading = false;
    }
}

loadPatient().then((success) => (success ? loadCards() : null));
</script>

<template>
    <DsLoadingOverlay :loading="state.isLoading" />
    <template v-if="state.patient">
        <div class="flex flex-row m-5 gap-5">
            <!-- Patient Card -->
            <PatientInfoCard :patient="state.patient" />

            <!-- EHR Apps Card -->
            <EhrAppsCard />
        </div>

        <!-- Navigation Tabs -->
        <div class="border rounded-t-md border-silver m-5">
            <!-- Tab Headers -->
            <div
                class="flex rounded-t-md border-b border-silver space-x-5 bg-bliss px-5"
            >
                <DsTab
                    :labels="
                        tabs.filter((x) => x.enabled).map((tab) => tab.title)
                    "
                    :selected-tab="state.currentTab"
                    @click="state.currentTab = $event"
                >
                    <template #alerts>
                        <div
                            v-if="state.cards.length > 0"
                            class="text-center text-white rounded-full w-[32px]"
                            :class="
                                state.cards.some(
                                    (x) => x.indicator === 'critical'
                                )
                                    ? 'bg-fire'
                                    : state.cards.some(
                                          (x) => x.indicator === 'warning'
                                      )
                                    ? 'bg-marigold'
                                    : 'bg-secondary'
                            "
                        >
                            {{ state.cards.length }}
                        </div>
                    </template>
                </DsTab>
            </div>
            <div
                v-if="workspace?.subscriptionType === 'developer'"
                class="mx-5 mt-2"
            >
                <DsText size="sm" :color="Colors.fire">
                    You are currently on a developer subscription, which is
                    intended solely for testing, development, and evaluation
                    purposes. This subscription type does not permit the use of
                    the service for storing Protected Health Information (PHI).
                    If your project requires the handling of PHI please consider
                    upgrading to one of our paid subscriptions.
                </DsText>
            </div>
            <!-- Tab Contents -->
            <div v-if="selectedTab" class="p-5">
                <component
                    :is="selectedTab.component"
                    :workspace-slug="workspaceSlug"
                    :patient-id="patientId"
                    :[selectedTab.isProvider!]="true"
                    :[selectedTab.cards!]="state.cards"
                    @[selectedTab.feedbackSubmitted]="loadCards"
                    @[selectedTab.reloadPatientData]="loadPatient"
                />
            </div>
        </div>
    </template>
</template>
