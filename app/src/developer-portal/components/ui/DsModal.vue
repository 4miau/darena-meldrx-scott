<script setup lang="ts">
defineProps<{
    modelValue?: boolean
}>()

const emit = defineEmits<{
    'close': [],
    'update:modelValue': [boolean],
}>()

const contentRef = ref<HTMLElement>()

function close() {
    emit('update:modelValue', false)
    emit('close')
}
</script>

<template>
    <div v-if='modelValue' class='z-50 fixed inset-0 bg-bliss/75 flex items-center justify-center transition-opacity ease-out duration-300' role='dialog' aria-modal='true'>
        <!-- Modal -->
        <div ref='contentRef' class='relative w-full bg-white sm:max-w-lg shadow'>
            <!-- Close Button -->
            <button type='button' class='absolute top-4 right-2 text-dsgray' @click="close">
                <svg class='w-3 h-3' aria-hidden='true' xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 14 14'>
                    <path stroke='currentColor' stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='m1 1 6 6m0 0 6 6M7 7l6-6M7 7l-6 6'/>
                </svg>
                <span class='sr-only'>Close modal</span>
            </button>

            <!-- Content -->
            <div>
                <slot/>
            </div>
        </div>
    </div>
</template>