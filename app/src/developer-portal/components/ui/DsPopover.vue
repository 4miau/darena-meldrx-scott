<script setup lang="ts">

const state = reactive({
    showPopover: false,
    popoverStyles: {
        left: '0px',
        bottom: '100%',
    }
});

const triggerRef = ref<HTMLElement | null>(null);
const popoverRef = ref<HTMLElement | null>(null);

function updatePopoverPosition() {
    if (!triggerRef.value || !popoverRef.value) return;

    const triggerRect = triggerRef.value.getBoundingClientRect();
    const popoverRect = popoverRef.value.getBoundingClientRect();
    const windowWidth = window.innerWidth;

    let left = triggerRect.left + triggerRect.width / 2 - popoverRect.width / 2;
    left = Math.max(8, Math.min(left, windowWidth - popoverRect.width - 8));

    state.popoverStyles.left = `${left - triggerRect.left}px`;
}

function showPopover() {
    state.showPopover = true;
    nextTick(() => {
        updatePopoverPosition();
    });
}
</script>

<template>
  <div
    ref="triggerRef"
    class="relative inline-block"
    @mouseleave="state.showPopover = false"
    @mouseover="showPopover"
  >
    <slot/>
    <div
      v-if="state.showPopover"
      ref="popoverRef"
      class="absolute mb-2 p-2 bg-bliss rounded shadow-lg z-50 w-[240px]"
      :style="state.popoverStyles"
    >
      <slot name="content"/>
    </div>
  </div>
</template>
