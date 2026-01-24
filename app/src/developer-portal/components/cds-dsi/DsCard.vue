<script setup lang="ts">
import type {Card, CardAction, CardLink, CardOverrideReasonWithComment} from '~/types/cds-hooks/CDSCards';
import {Colors} from '~/types/ui/colors';
import type {Feedback} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CdsHooks/Feedback';

const {$api} = useNuxtApp();
const route = useRoute()
const isLoading = ref<boolean>(false);
const props = defineProps<{
  showCard: boolean,
  card: Card,
  workspaceSlug: string,
  displayOnly?: boolean,
}>();

const emit = defineEmits<{
  'dismiss': [overrideReason: string, cardId: string, userComment?: string],
  'smartLinkSelected': [CardLink, string?],
  'actionSelected': [CardAction, string?],
}>()
const patientId = ref<string>(route.params.patientId as string)
const showDetails = ref<boolean>(false)
const showDismissModal = ref<boolean>(false)
const overrideReasonDisplay = ref<string | undefined>('Other (Provide reason)')
const overrideReasonCode = ref<string | undefined>('Other (Provide reason)')
const overrideReasonSystem = ref<string | undefined>('Other (Provide reason)')
const selectedOverrideReason = ref<string>('')
const userComment = ref<string>('')

const dismissReasons = (props.card.overrideReasons && props.card.overrideReasons.length > 0)
    ? [
        {
            title: 'SELECT A REASON',
            options: [
                ...props.card.overrideReasons.map(reason => ({
                    value: reason.code,
                    title: reason.display!,
                    ...(reason.system ? {system: reason.system} : {})
                })),
                {
                    value: 'other',
                    title: 'Other (Provide reason)'
                }
            ]
        }
    ]
    : [
        {
            title: 'SELECT A REASON',
            options: [
                {
                    value: 'other',
                    title: 'Other (Provide reason)'
                }
            ]
        }
    ];


function dismissSelected(selectedReason: string) {
    if (props.displayOnly) {
        return
    }
    if (props.card.overrideReasons) {
        const dismissReason = dismissReasons.find(x => x.options)?.options.find(x => x.value === selectedReason);
        if (dismissReason) {
            overrideReasonDisplay.value = dismissReason.title;
            overrideReasonCode.value = dismissReason.value;
            overrideReasonSystem.value = dismissReason.system ?? '';
        }
    }
    selectedOverrideReason.value = selectedReason
    userComment.value = ''
    showDismissModal.value = true
}

async function onClickSuggestion(action: CardAction, id: string) {
    isLoading.value = true;
    try {
        const suggestionFeedback: Feedback[] = [{
            card: props.card.uuid!,
            outcome: "accepted",
            outcomeTimestamp: new Date().toISOString(),
            acceptedSuggestions: [{'id': id}]
        }];
        const payload = {
            feedback: suggestionFeedback
        };

        await $api.cdsServices.postFeedback(props.workspaceSlug, "meldrxcdshooks-democards", payload);

    } catch (error) {
        handleApiError(error, 'Unable to process suggestion');
    } finally {
        isLoading.value = false;
    }
    emit('actionSelected', action, id);
}

async function submitOverride() {
    isLoading.value = true;
    try {
        const overrideReason: CardOverrideReasonWithComment = {
            reason: {code: selectedOverrideReason.value, system: ""},
            userComment: userComment.value
        };
        const overrideFeedback: Feedback[] = [{
            card: props.card.uuid!,
            outcome: "overridden",
            outcomeTimestamp: new Date().toISOString(),
            overrideReason: overrideReason
        }];
        const payload = {
            feedback: overrideFeedback
        };
        await $api.cdsServices.postFeedback(props.workspaceSlug, "meldrxcdshooks-democards", payload);

    } catch (error) {
        handleApiError(error, 'Unable to submit override');
    } finally {
        isLoading.value = false;
    }
    emit('dismiss', userComment.value, selectedOverrideReason.value, props.card.uuid)
    showDismissModal.value = false
}

const dynamicIcon = computed(() => {
    if (props.card.indicator === 'info') {
        return 'heroicons:information-circle';
    }
    if (props.card.indicator === 'warning') {
        return 'heroicons:exclamation-triangle';
    }
    if (props.card.indicator === 'critical') {
        return 'heroicons:exclamation-circle';
    }
    return 'heroicons:information-circle';
});

const dynamicColor = computed(() => {
    if (props.card.indicator === 'info') {
        return Colors.secondary;
    }
    if (props.card.indicator === 'warning') {
        return Colors.marigold;
    }
    if (props.card.indicator === 'critical') {
        return Colors.fire;
    }
    return Colors.onyx;
});

const dynamicButtonColor = computed(() => {
    if (props.card.indicator === 'info') {
        return Colors.secondary;
    }
    if (props.card.indicator === 'warning') {
        return Colors.marigold;
    }
    if (props.card.indicator === 'critical') {
        return Colors.fire;
    }
    return Colors.onyx;
});

function getSuggestionButtonColor(type: string) {
    switch (type) {
        case 'create':
            return Colors.primary;
        case 'update':
            return Colors.secondary;
        case 'delete':
            return Colors.fire;
        default:
            return Colors.onyx;
    }
}

async function launchApp(link: CardLink) {
    const response = await $api.ehr.createContext(props.workspaceSlug, {
        patientId: patientId.value,
        workspaceSlug: props.workspaceSlug
    })
    const issuerUrl = `${window.location.origin}/api/fhir/${props.workspaceSlug}`;
    const launchUrl = `${link.url}?iss=${issuerUrl}&launch=${response.launchContext}`
    window.open(launchUrl, '_blank');
}

</script>

<template>
  <div
      v-if="showCard"
      class="flex flex-col space-y-2 bg-bliss border border-t-silver border-r-silver border-b-silver rounded-sm p-4 relative"
      :class="`border-l-${dynamicColor} border-l-4 border-l-solid`"
  >
    <!-- Card Indicator -->
    <div class="flex place-content-between">
      <div class="flex items-center space-x-1">
        <DsIcon :name="dynamicIcon" size="md" :color='dynamicColor'/>
        <DsText>
          {{ card.indicator.toUpperCase() }}
        </DsText>
      </div>
      <DsDropdown class="object-cover" label="Feedback" :options="dismissReasons" @select="(v) => dismissSelected(v)"/>
    </div>

    <!-- Card Source -->
    <DsText v-if="card.source" class="flex items-center gap-1" size="sm">
      <div v-if="card.source.icon">
        <img class="max-h-[24px] max-w-[24px]" :src="card.source.icon" alt="">
      </div>
      <div>
        <DsText size="xs" weight="light">
          SOURCE
        </DsText>
      </div>
      <div>
        <DsLink v-if="card.source.url" new-tab :href="card.source.url">
          <DsText :color="Colors.secondary" size="xs" weight="light">
            {{ card.source.label }}
          </DsText>
        </DsLink>
        <DsText v-if="!card.source.url" size="xs" weight="light">
          {{ card.source.label }}
        </DsText>
      </div>
    </DsText>

    <!-- Card Summary/Detail -->
    <div class="flex flex-col space-y-2 pb-2">
      <DsText size="md">
        {{ card.summary }}
      </DsText>
      <div v-if="card.detail">
        <div class="flex items-center cursor-pointer" @click="showDetails = !showDetails">
          <DsText underline size="sm">
            {{ showDetails ? 'Hide Details' : 'View more details' }}
          </DsText>
          <DsIcon :class="{'rotate-90':showDetails}" class="duration-200" name="heroicons:chevron-right" size="xs"/>
        </div>
        <div v-if="showDetails">
          <DsText size="sm">
            {{ card.detail }}
          </DsText>
        </div>
      </div>
    </div>

    <template v-if="(card.links?.length ?? 0 > 0) || (card.suggestions?.length ?? 0 > 0)">

      <DsDivider size="xs" :color="Colors.silver"/>

      <div class="flex flex-col gap-4">
        <!-- Card Links -->
        <div v-if="card.links?.some(x=>x.type=== 'absolute')" class="flex flex-col gap-1">
          <DsText size="xs" weight="light">
            LINKS
          </DsText>
          <div v-for="link in card.links" :key="link.url" class="flex flex-col gap-4">
            <DsLink
v-if="link.type === 'absolute'" :new-tab="true" :color="Colors.secondary"
                    class="flex items-center space-x-1 pt-1" :href="link.url">
              <DsIcon name="heroicons:link" size="xs"/>
              <DsText :colors="Colors.secondary" size="sm">
                {{ link.label }}
              </DsText>
            </DsLink>
          </div>
        </div>

        <!-- Suggested Apps -->
        <div v-if="card.links?.some(x=>x.type=== 'smart')" class="flex flex-col gap-1">
          <DsText size="xs" weight="light">
            SUGGESTED APPS
          </DsText>
          <div v-for="link in card.links" :key="link.url" class="flex flex-col gap-4">
            <div v-if="link.type === 'smart'">
              <DsButton
                :color="dynamicButtonColor" :text-color='dynamicButtonColor' :title="link.label" size="sm" variant="outline"
                :disabled="displayOnly" @click="launchApp(link)"
              >
                {{ link.label }}
              </DsButton>
            </div>
          </div>
        </div>

        <!-- Card Suggestions -->
        <div v-if="card.suggestions" class="flex flex-col gap-1">
          <DsText size="xs" weight="light">
            SUGGESTED ACTIONS
          </DsText>
          <div class="flex flex-col gap-4">
            <div v-for="suggestion in card.suggestions" :key="suggestion.uuid">
              <div class="flex flex-col gap-1">
                <div v-if="suggestion.isRecommended">
                  <DsText size="sm" color="text-black">
                    RECOMMENDED
                  </DsText>
                </div>
                <div v-for="action in suggestion.actions" :key="action.description" class="flex flex-col gap-4">
                  <div class="flex flex-col gap-1">
                    <DsText size="sm">
                      {{ action.description }}
                    </DsText>
                    <div>
                      <DsButton
                          :class="action.type === 'create' ? 'font-light' : 'capitalize'"
                          :color="getSuggestionButtonColor(action.type)"
                          :text-color="getSuggestionButtonColor(action.type)"
                          size="sm"
                          variant="outline"
                          :disabled="displayOnly"
                          @click="onClickSuggestion(action, suggestion.uuid!);"
                      >
                        {{ suggestion.label }}
                      </DsButton>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>

    <DsModal :model-value="showDismissModal" @close="showDismissModal = false">
      <DsForm>
        <div class="flex w-full py-8 bg-space justify-center items-center border-t-[10px] border-jasmine">
          <div class="flex w-full items-center justify-center">
            <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center">
              <MeldRxMail/>
            </div>
          </div>
          <div class="flex-1"/>
        </div>
        <div class="flex w-full p-4">
          <div class="flex flex-col w-full space-y-5 mx-10">
            <!-- Heading -->
            <div class="w-full flex-col justify-center items-center gap-5 flex">
              <DsText size="2xl" weight="light">
                Override Reason
              </DsText>

              <DsText size="sm" weight="light" class="text-center">
                Provide feedback for why this card is not useful or relevant.
              </DsText>
            </div>

            <div>
              <!-- First Name -->
              <DsTextInput
                  v-model="userComment"
                  type="text"
                  :label="overrideReasonDisplay"
              />
            </div>
            <DsDivider/>
            <div class="flex justify-center">
              <DsButton :color="Colors.white" :text-color='Colors.onyx' class="mr-4" @click="showDismissModal = false">
                Cancel
              </DsButton>
              <DsButton id="submit-dismissal-button" title="Submit" :color="Colors.secondary" @click="submitOverride()">
                Submit
              </DsButton>
            </div>
          </div>
        </div>
      </DsForm>
    </DsModal>
  </div>
</template>
