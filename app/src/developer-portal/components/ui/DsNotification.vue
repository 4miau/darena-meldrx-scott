<script setup lang="ts">
useToaster((toast: Toast) => {
    queue.value.push(toast)
})

function remove(toast: Toast) { queue.value.splice(queue.value.indexOf(toast), 1) }

const queue = ref<Toast[]>([])
</script>

<template>
    <transition-group name="toast-stack" tag="div" class="fixed top-4 right-4 z-50 flex flex-col space-y-2">
        <DsToast v-for='toast in queue' :key='toast.id' v-bind='toast' class='transition-all duration-200' @close='remove(toast)' />
    </transition-group>
</template>
