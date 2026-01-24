<!--
    Text
    Usage:
        <DsText size="lg">Hello</DsText>
-->

<script setup lang="ts">
import type { TextSize, TextWeight } from '~/types/ui/DsText';

const props = defineProps<{
    size?: TextSize;
    weight?: TextWeight;
    color?: string;
    underline?: boolean;
}>();

const dynamicClasses = computed(() => {
    const classes: string[] = [];

    // Get class based on size...
    switch (props.size) {
        case 'xs': classes.push('text-xs'); break;
        case 'sm': classes.push('text-sm'); break;
        case 'md': classes.push('text-base'); break;
        case 'lg': classes.push('text-lg'); break;
        case 'xl': classes.push('text-xl'); break;
        case '2xl': classes.push('text-2xl'); break;
        default: classes.push('text-base'); break;
    }

    // Get class based on weight...
    switch (props.weight) {
        case 'light': classes.push('font-light'); break;
        case 'normal': classes.push('font-normal'); break;
        case 'bold': classes.push('font-bold'); break;
        default: classes.push('font-normal'); break;
    }

    if (props.underline) {
        classes.push('underline')
    }

    if (props.color && props.color.trim().length > 0) {
        classes.push(`text-${props.color}`);
    } else {
        classes.push('text-onyx');
    }

    return classes.join(' ');
});
</script>

<template>
  <span class="leading-normal" :class="dynamicClasses"><slot /></span>
</template>
