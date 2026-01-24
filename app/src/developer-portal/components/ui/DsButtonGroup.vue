<!--
    Button Group
    - v-model: binds value of button to model
    - vertical: set the direction of the brn group to vertical (horizontal by default)
    - active-color: specify which color:Colors to apply when selected
    - inactive-color: specify which color:Colors to apply when not selected

    DsButton value property is consumed by the DsButtonGroup and is bound to v-model

    Usage:
      <DsButtonGroup
        v-model="currentButton"
        vertical
        :active-color="Colors.primary"
        :inactive-color="Colors.white"
      >
        <DsButton value="one" variant="filled" title="One" />
        <DsButton value="two" variant="filled" title="Two" />
        <DsButton value="three" variant="filled" title="Three" />
      </DsButtonGroup>
 -->

<script setup lang="ts" generic="T">
import { Colors } from '~/types/ui/colors';

const props = withDefaults(
    defineProps<{
    modelValue: T | T[],
    multiple?: boolean,
    vertical?: boolean,
    activeColor?: Colors,
    activeTextColor?: Colors,
    inactiveColor?: Colors,
    inactiveTextColor?: Colors,
    rounded?: boolean,
    required? :boolean,
    rules?: ValidationRule<T | T[]>[],
    spaceBetween?: boolean
  }>(),
    {
        rounded: true,
        activeColor: Colors.secondary,
        activeTextColor: Colors.white,
        inactiveColor: Colors.white,
        inactiveTextColor: Colors.onyx,
        rules: undefined,
        spaceBetween: false
    }
)

const emit = defineEmits<{
  'update:model-value': [value?: T | T[]]
}>()

const styleClass = computed(() => {
    const styles = []

    styles.push(props.vertical ? 'flex-col' : 'flex-row')
    styles.push(props.rounded ? 'rounded' : 'rounded-none')
    if(props.spaceBetween){
        styles.push('justify-between')
    }

    return styles.join(' ')
})

const slots = useSlots();
const finalSlots = computed(() => {
    if (!slots.default) { return []; }
    const slotsDefault: any = slots.default();

    // This means we used a v-for (has one element with several children)...
    if (slotsDefault.length === 1 && !!slotsDefault[0].children) {
        return slotsDefault[0].children;
    }

    return slotsDefault;
});

function onClick(value: T) {
    dirty.value = true
    if (props.multiple) {
        const result = Array.isArray(props.modelValue)
            ? [...props.modelValue]
            : [props.modelValue]

        if (result.includes(value)) {
            emit('update:model-value', result.filter(x => x !== value))
        } else {
            result.push(value)
            emit('update:model-value', result)
        }
    } else {
        emit('update:model-value', value)
    }
}

function propValueSelected(propValue: T) : boolean {
    if (Array.isArray(props.modelValue)) {
        return (props.modelValue as T[]).includes(propValue)
    }
    return propValue === props.modelValue
}

const { dirty, validationResult } = useValidation(
    () => props.modelValue,
    () => props.rules,
    () => props.required
)

</script>

<template>
  <div v-if="$slots != undefined">
    <div class="flex cursor-pointer ds-button-grp" :class="styleClass">
      <component
        :is="slot"
        v-for="(slot, i) in finalSlots"
        :key="`key-${i}`"
        class="border-silver"
        :color="propValueSelected(slot.props.value) ? props.activeColor : props.inactiveColor"
        :text-color="propValueSelected(slot.props.value) ? props.activeTextColor : props.inactiveTextColor"
        @click="() => onClick(slot.props?.value)"
      />
    </div>
    <div v-if="!validationResult.isValid" class="relative mb-6">
      <DsText size="xs" :color="Colors.fire" class="absolute top-0">
        {{ validationResult.error }}
      </DsText>
    </div>
  </div>
</template>

<style scoped>
.ds-button-grp.flex-row > :deep(button.ds-button) {
  border-radius: 0px;
  border-top-width: 1px;
  border-right-width: 1px;
  border-bottom-width: 1px;
}
.ds-button-grp.flex-row > :deep(button.ds-button):first-child  {
  border-left-width: 1px;
}

.ds-button-grp.flex-row.rounded > :deep(button.ds-button):first-child {
  border-radius: 3px 0 0 3px;
}

.ds-button-grp.flex-row.rounded > :deep(button.ds-button):last-child {
  border-radius: 0 3px 3px 0;
}

.ds-button-grp.flex-col > :deep(button.ds-button) {
  border-radius: 0px;
  border-left-width: 1px;
  border-right-width: 1px;
  border-bottom-width: 1px;
}

.ds-button-grp.flex-col > :deep(button.ds-button):first-child {
  border-top-width: 1px;
}

.ds-button-grp.flex-col.rounded > :deep(button.ds-button):first-child {
  border-radius: 3px 3px 0 0;
}

.ds-button-grp.flex-col.rounded > :deep(button.ds-button):last-child {
  border-radius: 0 0 3px 3px;
}
</style>
