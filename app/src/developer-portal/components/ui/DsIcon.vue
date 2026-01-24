<script setup lang="ts">
import type { Colors } from '~/types/ui/colors'

defineProps<{
  name: string
  color?: Colors
  size?: 'xs'|'sm'|'md'|'lg'|'xl'
}>()

const sizes = {
    xs: '16px',
    sm: '20px',
    md: '24px',
    lg: '30px',
    xl: '36px'
}
</script>

<template>
  <Icon :name='name' :size='sizes[size ?? "sm"]' :class='color ? `text-${color}` : ""'/>
</template>
