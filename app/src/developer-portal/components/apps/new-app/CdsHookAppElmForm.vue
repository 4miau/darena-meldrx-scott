<script setup lang="ts">
import { Colors } from '~/types/ui/colors'
import { type ElmMetadata, parseElmFiles } from '~/utils/CqlToCdsHooksHelpers';
import type { CreateCdsHooksAppCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CreateCdsHooksAppCommand'
import type { CdsHookForm } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CdsHookForm';
import type { CardForm } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CardForm';
import CdsHookExpressionDetails from '~/components/apps/new-app/CdsHookExpressionDetails.vue';

const props = defineProps<{
  modelValue: CreateCdsHooksAppCommand
}>()

const emit = defineEmits<{
  'update:modelValue': [value: CreateCdsHooksAppCommand];
}>()

const state = reactive<{
  form: {
    cdsHook: CdsHookForm,
    cards: CardForm[],
    elmFiles?: FileList,
  };
  editCardIndex?: number;
  parsedData: ElmMetadata
}>({
    form: {
        cdsHook: props.modelValue.cdsHook ?? {
            description: '',
            hook: 'patient-view',
            title: '',
            usageRequirements: ''
        },
        cards: props.modelValue.cards ?? [],
        elmFiles: props.modelValue.elmFiles ?? undefined
    },
    parsedData: {
        prefetch: {},
        expressions: {},
        booleanExpressions:{},
        primaryFile: null,
    }
})

watch(
    () => state.form.elmFiles,
    async (files) => {
        if (files === undefined || files.length === 0) {
            state.parsedData = {
                prefetch: {},
                expressions: {},
                booleanExpressions:{},
                primaryFile: null,
            }

            state.form.cards = []

            return
        }

        const loadFiles = []
        for (let i = 0; i < files.length; i++) {
            loadFiles.push(files.item(i)!.text())
        }

        const fileContents = await Promise.all(loadFiles)
        state.parsedData = parseElmFiles(fileContents)
        state.form.cards = []
    }
)

watch(
    () => state.form,
    (newForm) => {
        emit(
            'update:modelValue',
            {
                name: props.modelValue.name,
                ...newForm
            }
        )
    },
    { deep: true }
)

function onSaveCard (card: CardForm) {
    if (state.editCardIndex === undefined) {
        return
    }

    const isAdd = state.editCardIndex === -1
    if (isAdd) {
        state.form.cards.push(card)
    } else {
        state.form.cards[state.editCardIndex] = card
    }

    state.editCardIndex = undefined
}
</script>

<template>
  <div class="grid md:grid-cols-12 gap-8">
    <div class="col-span-4">
      <DsLabeledText
          label="CQL Based CDS Hook"
          text="Upload a CQL File"
      />
    </div>
    <div class="col-span-8">
      <DsFileSelector v-model="state.form.elmFiles" placeholder-text="Upload .elm file(s)" multiple/>
    </div>
  </div>

  <template v-if="state.parsedData.primaryFile">
    <DsDivider/>
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

    <DsDivider class="my-8"/>

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
            <DsButton variant="subtle" :color="Colors.white" @click="state.form.cards!.splice(index, 1)">
              x
            </DsButton>
          </div>

          <DsButton id="add-new-card-button" variant="transparent" :text-color="Colors.fire" :color="Colors.white" @click="state.editCardIndex = -1">
            + Add New Card
          </DsButton>
        </DsLabeledInput>
      </div>
    </div>

    <DsDivider class="my-8"/>

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

    <DsDivider class="my-8"/>

    <CdsHookExpressionDetails :metadata="state.parsedData" />

  </template>

  <CdsHookAppCardForm
      v-if="state.editCardIndex != undefined"
      :expressions="state.parsedData.expressions"
      :boolean-expressions="state.parsedData.booleanExpressions"
      :card="state.editCardIndex >= 0 ? state.form.cards![state.editCardIndex] : undefined"
      @on-add="onSaveCard"
      @close="() => state.editCardIndex = undefined"
  />
</template>

