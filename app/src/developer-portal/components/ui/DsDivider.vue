<!--
    Horizontal line to divide content
-->

<script lang="ts" setup>
import type { DsSize } from '~/types/ui/DsSize';
import { Colors } from '~/types/ui/colors';

const props = withDefaults(
    defineProps<{
      size?: DsSize,
      color?: Colors
    }>(),
    {
        size: 'sm',
        color: Colors.bliss
    }
);

const styles = computed(() => {
    const classes = [];

    if (props.size === 'xs') {
        classes.push('border-t-[1px]');
    } else if (props.size === 'sm') {
        classes.push('border-t-[2px]');
    } else if (props.size === 'md') {
        classes.push('border-t-[4px]');
    }

    classes.push(`border-${props.color}`);

    return classes;
});

</script>

<template>
  <div class="self-stretch my-5" :class="styles" />
</template>
