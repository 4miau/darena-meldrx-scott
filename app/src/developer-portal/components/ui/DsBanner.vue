<script setup lang="ts">
import { Colors } from '~/types/ui/colors'

defineProps<{
  icon: string
  iconSize?: 'xs'|'sm'|'md'|'lg'|'xl'
  color?: Colors
}>()

</script>

<template>
    <div class='flex items-center'>
      <DsIcon :name="icon" :size="iconSize ?? 'lg'" :color='color ?? Colors.fire' />
      <DsText size="sm" weight="normal" :color="color ?? Colors.fire" class="max-w-[500px] pl-2 text-left">
          <slot />
      </DsText>
    </div>
</template>
