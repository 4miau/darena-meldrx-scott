<!--
    Displays an inline badge
-->
<script setup lang="ts">

import { Colors } from '~/types/ui/colors'
import type { DsSize } from '~/types/ui/DsSize'

const props = withDefaults(
    defineProps<{
      color: Colors;
      textColor: Colors;
      size: DsSize;
      rounded?: boolean;
    }>(),
    {
        color: Colors.secondary,
        textColor: Colors.white,
        size: 'md',
        rounded: true,
    }
)

const badgeClasses = computed(() => {
    const classes = [
        'font-semibold',
        'leading-none',
        `text-${props.textColor}`,
        `bg-${props.color}`
    ]

    if (props.size === 'xs') {
        classes.push(`text-${props.size}`, 'px-1', 'py-0.5')
    } else if (props.size === 'sm') {
        classes.push(`text-${props.size}`, 'px-1.5', 'py-1')
    } else if (props.size === 'md') {
        classes.push(`text-${props.size}`, 'px-3', 'py-2')
    }

    if (props.rounded) {
        classes.push('rounded')
    }

    return classes.join(' ')
})
</script>

<template>
  <span :class="badgeClasses">
    <slot/>
  </span>
</template>
