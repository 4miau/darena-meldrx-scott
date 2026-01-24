<script setup lang="ts">
import type {
    CreateCdsHooksAppCommand
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CreateCdsHooksAppCommand';
import { Colors } from '~/types/ui/colors';
import type { CqlCompilationResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CqlCompilationResult';
import type { CdsHookForm } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CdsHookForm';
import type { CardForm } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CardForm';
import type { ElmMetadata } from '~/utils/CqlToCdsHooksHelpers';
import CdsHookExpressionDetails from '~/components/apps/new-app/CdsHookExpressionDetails.vue';

const props = defineProps<{
  modelValue: CreateCdsHooksAppCommand,
  updating?: boolean,
}>();

const emit = defineEmits<{
  'update:modelValue': [value: CreateCdsHooksAppCommand];
}>();

const state = reactive<{
  form: {
    cdsHook: CdsHookForm,
    cards: CardForm[],
    cqlEditorArtifact?: any,
  };
  updating: boolean,
  editCardIndex?: number;
  parsedData?: ElmMetadata
}>({
    form: {
        cdsHook: props.modelValue.cdsHook ?? {
            description: '',
            hook: 'patient-view',
            title: '',
            usageRequirements: ''
        },
        cards: props.modelValue.cards ?? [],
        cqlEditorArtifact: props.modelValue.cqlEditorArtifact,
    },
    updating: props.updating ?? false
});

watch(
    () => state.form,
    (newForm) => {
        emit(
            'update:modelValue',
            {
                name: props.modelValue.name,
                ...newForm
            }
        );
    },
    { deep: true }
);

function onSaveCard (card: CardForm) {
    if (state.editCardIndex === undefined) {
        return;
    }

    const isAdd = state.editCardIndex === -1;
    if (isAdd) {
        state.form.cards.push(card);
    } else {
        state.form.cards[state.editCardIndex] = card;
    }

    state.editCardIndex = undefined;
}

function onCompiled (
    {
        artifactForm,
        compilationResult
    }: { artifactForm: any, compilationResult: CqlCompilationResult[] }
) {
    state.parsedData = parseElmFiles(compilationResult.map(x => x.content));
    state.form.cqlEditorArtifact = artifactForm
}
</script>

<template>

  <div class="grid md:grid-cols-12 gap-8">
    <div class="col-span-4">
      <DsLabeledText
        label="Custom CQL"
        text="Create a CDS Hook using the CQL builder tool"
      />
    </div>

    <div class="col-span-8 space-y-5">
      <template v-if="state.updating">
        <DsButton id='update-cql-button' :color="Colors.secondary" @click="state.updating = false">
          Update CQL
        </DsButton>
      </template>
      <template v-else-if="!state.parsedData">
        <LazyCqlEditor :artifact-form="state.form.cqlEditorArtifact" @compiled="onCompiled" />
      </template>
      <template v-else>
        <DsButton id='edit-cql-button' :color="Colors.secondary" @click="state.parsedData = undefined">
          Edit CQL
        </DsButton>
      </template>
    </div>
  </div>

  <template v-if="state.parsedData && state.parsedData.primaryFile">
    <DsDivider />
    <div class="grid md:grid-cols-12 gap-8">

      <div class="col-span-4">
        <DsLabeledText
          label="Hook Details"
          text="Define the basic information about the CDS Hooks"
        />
      </div>

      <div class="col-span-8 space-y-5">

        <DsTextInput
          :model-value="state.form.cdsHook.hook"
          type="text"
          label="Hook"
          disabled
        />

        <DsTextInput
          v-model="state.form.cdsHook.title"
          type="text"
          label="Title"
          placeholder="hello-world hook"
          required
          :rules="[
                  [v => !!v && !/^\s+$/.test(v!), 'The Title is required'],
                  [v => v!.length <= 100, 'The Title must be 100 characters or less']
                ]"
        />

        <DsTextInput
          v-model="state.form.cdsHook.description"
          type="text"
          label="Description"
          placeholder="this hook prints hello world when patient is opened"
          required
          :rules="[
                  [v => !!v && !/^\s+$/.test(v!), 'The Description is required'],
                  [v => v!.length <= 200, 'The Description must be 200 characters or less']
                ]"
        />

        <DsTextInput
          v-model="state.form.cdsHook.usageRequirements"
          type="text"
          label="Usage Requirements"
          placeholder="describe requirements"
          :required="false"
          :rules="[
                  [v => v!.length <= 200, 'The field must be 200 characters or less']
                ]"
        />
      </div>
    </div>

    <DsDivider class="my-8" />

    <div class="grid md:grid-cols-12 gap-8">
      <div class="col-span-4">
        <DsLabeledText
          label="Configured Cards"
          text="Configure the Cards that will be shown for this Hook."
        />
      </div>

      <div class="col-span-8">
        <DsLabeledInput label="Cards" :required="false">
          <div
            v-for="(card, index) in state.form.cards"
            :id="card.summary.replaceAll(' ', '-').toLowerCase()"
            :key="`cds-card-${index}`"
            class="flex flex-row items-center border-b border-bliss"
          >
            <div class="flex flex-row items-center cursor-pointer w-full" @click="state.editCardIndex = index">
              <DsText size="sm" weight="light" class="grow underline">
                {{ card.summary }}
              </DsText>
            </div>
            <DsButton id='x-button' variant="subtle" :color="Colors.white" :text-color="Colors.black" @click="state.form.cards!.splice(index, 1)">
              x
            </DsButton>
          </div>

          <DsButton id="add-new-card-button" variant="transparent" :text-color="Colors.fire" @click="state.editCardIndex = -1">
            + Add New Card
          </DsButton>
        </DsLabeledInput>
      </div>
    </div>

    <DsDivider class="my-8" />

    <div class="grid md:grid-cols-12 gap-8">
      <div class="col-span-4">
        <DsLabeledText
          label="Prefetch Queries"
          text="The pre-fetch queries that the CQL library would require."
        />
      </div>

      <div class="col-span-8 space-y-5">
        <DsLabeledText label="Required Prefetch">
          <div v-if="Object.keys(state.parsedData.prefetch).length > 0">
            <ul>
              <li v-for="(query, resource) in state.parsedData.prefetch" :key="resource">
                {{ resource }}: {{ query }}
              </li>
            </ul>
          </div>
        </DsLabeledText>
      </div>
    </div>

    <DsDivider class="my-8" />

    <CdsHookExpressionDetails :metadata="state.parsedData" />

    <CdsHookAppCardForm
      v-if="state.editCardIndex != undefined"
      :expressions="state.parsedData.expressions"
      :boolean-expressions="state.parsedData.booleanExpressions"
      :card="state.editCardIndex >= 0 ? state.form.cards![state.editCardIndex] : undefined"
      @on-add="onSaveCard"
      @close="() => state.editCardIndex = undefined"
    />

  </template>

</template>
