<!--
  Displays a set of fat buttons representing each item in a list of options.
  Must adhere to the IItem<T> interace.

  Usage:
    const userTypes: { value: UserType, title: string }[] = [
      { value: 'Patient', title: 'Patient' },
      { value: 'Provider', title: 'Provider' },
      { value: 'System', title: 'System' }
    ];

    <DsSingleSelectButtonList
      v-model="currentUserType"
      :options="userTypes"
      minWidth = "8rem"
      maxWidth = "12rem"
    />
-->

<script setup lang="ts" generic="T">
import { Colors } from '~/types/ui/colors'
import type { IDsSingleSelectButtonListItem } from '~/types/ui/DsSingleSelectButtonList'

const props = defineProps<{
    modelValue?: T
    label?: string
    required?: boolean
    disabled?: boolean
    options?: IDsSingleSelectButtonListItem<T>[]
    minWidth?: string
    maxWidth?: string
    tile?: boolean;
}>()

defineEmits<{
    'update:modelValue': [value?: T]
}>()

const widthStyles = reactive({
    minWidth: props.minWidth ?? '8rem',
    maxWidth: props.maxWidth ?? '12rem'
})
</script>

<template>
    <DsLabeledInput :label="label" :required="required">
          <DsButtonGroup
              :model-value="modelValue"
              :vertical="false"
              :rounded="!tile"
              :active-color="Colors.secondary"
              :inactive-color="Colors.white"
              @update:model-value="$emit('update:modelValue', $event as T)"
          >
              <DsButton
                  v-for="(option, index) in options"
                  :key="`DsSingleSelectButtonList_${index}`"
                  :text-color="Colors.gray"
                  variant="filled"
                  :disabled="disabled"
                  :value="option.value"
                  :sub-content="`${option.subTitle ? option.subTitle : ''}`"
                  :icon-top="option.iconTop"
                  :style='widthStyles'
              >
                  {{ option.title }}
              </DsButton>
          </DsButtonGroup>
    </DsLabeledInput>
</template>
