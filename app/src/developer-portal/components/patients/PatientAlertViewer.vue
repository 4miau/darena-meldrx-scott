<script setup lang="ts">
import type {Card} from "~/types/cds-hooks/CDSCards";

const route = useRoute();

const props = defineProps<{
  cards: Card[]
}>()

defineEmits<{
  feedbackSubmitted: [id:string]
}>()

const state = reactive<{
  patientId: string;
  workspaceSlug: string;
}>({
    patientId: route.params.patientId as string,
    workspaceSlug: route.params.workspaceSlug as string,
})

const displayIndicators = {
    critical: 'critical',
    warning: 'warning',
    info: 'info'
};

const cardsByIndicator = computed(() => {
    const groupedCards: { [key: string]: Card[] } = {};
    Object.keys(displayIndicators).forEach(indicator => {
        groupedCards[indicator] = [];
    });

    props.cards.forEach((card) => {
        if (groupedCards[card.indicator] !== undefined) {
            groupedCards[card.indicator].push(card);
        }
    });
    return groupedCards;
});

const tableHeaders = computed(() => {
    return [
        'CRITICAL' + ' (' + cardsByIndicator.value[displayIndicators.critical]?.length + ')',
        'WARNING' + ' (' + cardsByIndicator.value[displayIndicators.warning]?.length + ')',
        'INFORMATION' + ' (' + cardsByIndicator.value[displayIndicators.info]?.length + ')'
    ]
})

</script>

<template>
  <DsTable
    :headers="tableHeaders"
    :items="[cardsByIndicator]"
    :id-selector="item => 'cardAlerts'"
    disable-hover
    disable-stripe
    custom-table-row-css="border border-silver items-center content-start py-3"
    custom-header-css="text-center border border-silver py-2"
    :full-width="false"
    borderless
    fixed
  >
    <template #default="{item:groupedCards}">
      <div v-for="group in Object.keys(groupedCards)" :key="group" class="flex flex-col space-y-2 min-h-[700px] px-2 lg:px-5">
        <DsCard
          v-for="card in groupedCards[group]"
          :key="card.uuid"
          class="w-[400px]"
          :workspace-slug="state.workspaceSlug"
          :show-card="true"
          :card="card"
          @dismiss="$emit('feedbackSubmitted', card.uuid)"
          @smart-link-selected="$emit('feedbackSubmitted', card.uuid)"
          @action-selected="$emit('feedbackSubmitted', card.uuid)"
        />
        <div v-if="groupedCards[group].length === 0">
          <DsText>
            No {{ (group === 'info' ? 'information' : group) }} alerts for this patient
          </DsText>
        </div>
      </div>
    </template>
  </DsTable>
</template>


