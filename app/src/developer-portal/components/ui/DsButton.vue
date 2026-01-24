<script setup lang="ts">
import type {DsSize} from '~/types/ui/DsSize'
import DsText from './DsText.vue'
import type {Colors} from '~/types/ui/colors'


const props = defineProps<{
    id?: string
    textColor?: Colors
    textSize?: DsSize
    textWeight?: 'light' | 'normal' | 'bold'
    padding?: string
    color?: Colors
    subContent?: string
    iconTop?: string
    variant?: 'filled' | 'outline' | 'subtle' | 'transparent'
    size?: DsSize
    disabled?: boolean
    tile?: boolean
}>()

const dynamicClass = computed(() => {
    const color = props.color ?? 'primary'
    const size = props.size ?? 'md'
    const variant = props.variant ?? 'filled'

    const classes: string[] = []

    classes.push('ds-button')
    classes.push(`ds-button--${variant}-${color}`)
    classes.push(`ds-button--${size}`)
    classes.push(props.tile ? 'rounded-none' : 'rounded-sm')

    return classes.join(' ')
})

defineEmits<{ 'click': [payload: MouseEvent] }>()

const slots = useSlots()
const slotChildren = computed(() => {
    if (!slots.default) return []
    const slotsDefault = slots.default()
    if (!slotsDefault) return []

    return slotsDefault
        .filter(slot => slot.children !== 'v-if')
        .map(slot => {
            if (typeof slot.children === 'string') {
                return {
                    component: DsText,
                    body: slot.children,
                    attr: {
                        color: props.textColor || 'white',
                        size: props.textSize || 'sm',
                        weight: props.textWeight || 'normal',
                    },
                }
            }

            return {
                component: slot,
                body: '',
                attr: slot.props,
            }
        })
})

const id = computed<string>(() => {
    if (props.id) return props.id
    const title = slotChildren.value.find(slot => slot.component === DsText)?.body

    return title ?
        `${title?.trim().replaceAll(' ', '-').toLowerCase()}-button`
        : `${props.color || 'primary'} ${props.variant || 'filled'}`.replaceAll(' ', '-') + '-button'
})
</script>

<template>
	<button :id="id" type="button" :disabled="disabled" :class="dynamicClass" block @click="$emit('click', $event)">
        <DsIcon v-if='props.iconTop' :name='props.iconTop' size='lg'/>
        <div class="flex justify-center items-center align-center">
            <div v-for="(slot, i) in slotChildren" :key="i" :class="props.padding ?? 'first:pl-3.5 px-1.5 last:pr-3.5'" class='flex flex-col'>
                <component :is="slot.component" v-bind="slot.attr">
                    {{ slot.body }}
                </component>
                <div v-if='props.subContent' class='block text-xs font-light'>
                    {{ props.subContent }}
                </div>
            </div>
        </div>
	</button>
</template>
