<script setup lang="ts">
import type { Toast } from '~/composables/useToaster'
import { Colors } from '~/types/ui/colors'

const props = defineProps<Toast>()
const emit = defineEmits<{ 'close': [] }>()

const state = reactive<{
    startTime?: number,
    interval?: number,
    displayTime: number
}>({
    displayTime: props.displayTime ?? 3000
})

const variant = computed(() => {
    if (props.variant === 'error') return Colors.fire
    if (props.variant === 'warning') return Colors.marigold
    if (props.variant === 'success') return Colors.primary
    if (props.variant === 'info') return Colors.secondary

    return Colors.secondary
})

const progress = computed(() => {
    if (!props.displayTime) return 100
    return (state.displayTime / props.displayTime) * 100
})

function pauseTimer() {
    if (props.displayTime === 0) return

    clearInterval(state.interval)
    const elapsed = Date.now() - state.startTime!
    state.displayTime = Math.max(0, state.displayTime - elapsed)
}

function startTimer() {
    if (props.displayTime === 0) return

    state.startTime = Date.now()
    state.interval = setInterval(() => {
        const elapsed = Date.now() - state.startTime!
        const remaining = state.displayTime - elapsed

        if (remaining <= 0) {
            state.displayTime = 0
            emit('close')
        } else {
            state.displayTime = remaining
            state.startTime = Date.now()
        }
    }, 50) as unknown as number
}

onMounted(startTimer)
onUnmounted(() => { clearInterval(state.interval) })
</script>

<template>
    <div id='toast-default' @mouseenter='pauseTimer()' @mouseleave='startTimer()'>
        <div class='relative flex px-4 py-4 bg-white rounded-md min-w-[350px] max-w-[350px] gap-2'>
            <DsIcon name="heroicons:check-circle" size="sm" :color='variant' class='mr-3'/>

            <div class='w-full'>
                <DsText size='sm' :color='Colors.onyx' weight='bold' class='block'>{{ props.title }}</DsText>
                <DsText size='sm' :color='Colors.onyx' class='block'>{{ props.description }}</DsText>
                <div v-for="(link, i) in links" :key='`ds-toast-${i}-${link.link}`'>
                  <DsLink :href='link.link' target="_blank">
                      <DsText size='sm' :color='Colors.secondary' class='underline block'>
                          {{ link.title }}
                      </DsText>
                  </DsLink>
                </div>
            </div>

            <button class='relative ml-3 w-5 h-5' @click='$emit("close")'>
                <DsIcon name='heroicons:x-mark' size='sm' />
            </button>

            <!-- Progress Bar -->
            <div :class='`bg-${variant}`' class='absolute bottom-0 left-0 h-1 transition-all duration-100' :style='{ width: progress + "%" }' />
        </div>
    </div>
</template>
