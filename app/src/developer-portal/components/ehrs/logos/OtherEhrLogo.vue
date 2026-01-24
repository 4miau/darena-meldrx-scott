<script setup lang="ts">
import type { EhrLogoSize } from '~/types/ehrs';
import type { TextSize } from '~/types/ui/DsText';

const props = defineProps<{
    size?: EhrLogoSize
  }>()

const size = computed<TextSize>(() => {
    if (props.size === 'sm') {
        return 'sm'
    }
    if (props.size === 'md') {
        return 'md'
    }

    return 'md'
})

</script>

<template>
  <DsText :size="size">
    <span v-if="size == 'sm'">Other</span>
    <span v-else>Other / Not Listed</span>
  </DsText>
</template>
