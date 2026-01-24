<script setup lang="ts">
const props = defineProps<{
  src?:string
  size?: 'xs'|'sm'|'md'|'lg'|'xl'
  alt?: string
}>()

const size = computed(() => {
    if (props.size === 'xs') { return 'w-8 h-8' }
    if (props.size === 'sm') { return 'w-12 h-12' }
    if (props.size === 'md') { return 'w-14 h-14' }
    if (props.size === 'lg') { return 'w-16 h-16' }
    if (props.size === 'xl') { return 'w-20 h-20' }

    return 'w-12 h-12'
})

const altInitial = computed(() => {
    const name = props.alt ?? ''
    return name.at(0)?.toUpperCase()
})
</script>

<template>
  <div :class='size' class='inline-flex items-center justify-center rounded-full'>
    <img v-if="src" :src="src" :alt="alt" class="w-full h-full object-cover">
    <span v-else class="text-[20px] leading-none truncate">{{ altInitial }}</span>
  </div>
</template>
