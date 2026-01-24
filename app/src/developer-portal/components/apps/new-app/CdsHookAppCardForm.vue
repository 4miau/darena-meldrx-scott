<script setup lang="ts">
import { Colors } from '~/types/ui/colors'
import type { IDsSelectItem } from '~/types/ui/DsSelect'
import type { CardForm } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CardForm';

const props = defineProps<{
  card?: CardForm
  expressions: Record<string, string[]>
  booleanExpressions: Record<string, string[]>
}>()

const emit = defineEmits<{
  'close': [];
  'onAdd': [CardForm];
}>()

const formRef = ref<FormRef>()
const state = reactive<{
  form: CardForm
}>({
    form: props.card === undefined
        ? {
            condition: '',
            detail: '',
            indicator: '',
            summary: ''
        }
        : { ...props.card }
})

const conditionItems = computed<IDsSelectItem<string>[]>(() => {
    return Object.values(props.booleanExpressions)
        .flat()
        .map(expression => ({
            value: expression,
            title: expression
        }))
})

const recommendationItems = computed<IDsSelectItem<string>[]>(() => {
    return Object.values(props.expressions)
        .flat()
        .map(expression => ({
            value: `\${${expression}}`,
            title: expression
        }))
})

function onAdd () {
    if (!formRef.value?.validate()) {
        return
    }

    emit('onAdd', state.form)
}

const indicatorItems: IDsSelectItem<string>[] = [
    {
        value: 'info',
        title: 'Info'
    },
    {
        value: 'warning',
        title: 'Warning'
    },
    {
        value: 'critical',
        title: 'Critical'
    }
]
</script>

<template>
  <DsModal :model-value="true" auto-width @close="$emit('close')">
    <DsModalProgressCard :total-steps='1' :current-step='1'>
      <div class="flex w-full py-6 bg-space justify-center items-center">
        <div class="flex w-full items-center justify-center">
          <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center">
            <MeldRxApp/>
          </div>
        </div>
      </div>
      <DsForm ref="formRef">
        <div class="w-full flex-col items-center py-4 px-8 flex">
          <DsText size="2xl" weight="light">
            Configure Card Details
          </DsText>

          <div class="w-full space-y-2">
            <DsSelect
                v-model="state.form.condition"
                label="Condition for card to display"
                :items="conditionItems"
                required
                :rules="[[v => !!v, 'Condition is required']]"
            />

            <DsSelect
                v-model="state.form.detail"
                label="Recommendation"
                :items="recommendationItems"
                required
                :rules="[[v => !!v, 'Recommendation is required']]"
            />

            <DsSelect
                v-model="state.form.indicator"
                label="Indicator"
                :items="indicatorItems"
                required
                :rules="[[v => !!v, 'Indicator is required']]"
            />

            <DsTextInput
                v-model="state.form.summary"
                label="Card Summary"
                required
                :rules="[[v => !!v, 'Summary is required']]"
            />

          </div>
          <DsDivider/>
          <div class="flex justify-center w-full gap-5">
            <DsButton id="cancel" :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" @click="$emit('close')">
              Cancel
            </DsButton>
            <DsButton id="save" :color="Colors.secondary" @click="onAdd">
              Save
            </DsButton>
          </div>
        </div>
      </DsForm>
    </DsModalProgressCard>
  </DsModal>
</template>
