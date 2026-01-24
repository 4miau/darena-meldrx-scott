<script setup lang="ts">
import {Colors} from "~/types/ui/colors";

const emit = defineEmits<{
  'close': [];
}>();

const contentRef = ref<HTMLElement>();


useClickOutsideElement(contentRef,() => { emit('close') });

</script>

<template>
  <div class="relative z-50">
    <!-- Overlay -->
    <div class="fixed inset-0 bg-bliss/75 transition-opacity ease-out duration-300"/>

    <!-- Container -->
    <div class="fixed inset-0 flex justify-center self-center sm:max-h-[85%]">
      <div ref="contentRef" class="flex flex-col bg-white rounded-lg sm:max-w-[80%]">
        <div class="flex justify-end mt-2 mr-2 ">
          <DsIcon name="heroicons:x-mark" size="sm" class="cursor-pointer" @click="emit('close')"/>
        </div>
        <!-- Content -->
        <div  class="overflow-hidden">
          <slot/>
        </div>

        <!-- Footer -->
        <div class="p-1 border-t border-bliss dark:border-silver flex justify-center">
          <DsButton variant="outline" :color="Colors.secondary" :text-color='Colors.secondary' @click="$emit('close')">
            Close
          </DsButton>
        </div>
      </div>
    </div>
  </div>
</template>