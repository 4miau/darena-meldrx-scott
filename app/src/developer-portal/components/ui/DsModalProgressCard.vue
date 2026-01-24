<!--
    Modal card with a yellow progress bar on the top.

    Usage:
        <DsModalProgressCard :totalSteps="3" :currentStep="1" >
            {{ Content Goes Here }}
        </DsModalProgressCard>

    Notes:
        - totalSteps must be between 1 and 12.
-->

<script setup lang="ts">

defineProps<{
    totalSteps: number;
    currentStep: number;
}>();

// These are all hard-coded to help Tailwind JIT compile.
// If you do a dynamic class like `grid-cols-${totalSteps}`, it won't work.
const classGridCols1 = 'grid-cols-1';
const classGridCols2 = 'grid-cols-2';
const classGridCols3 = 'grid-cols-3';
const classGridCols4 = 'grid-cols-4';
const classGridCols5 = 'grid-cols-5';
const classGridCols6 = 'grid-cols-6';
const classGridCols7 = 'grid-cols-7';
const classGridCols8 = 'grid-cols-8';
const classGridCols9 = 'grid-cols-9';
const classGridCols10 = 'grid-cols-10';
const classGridCols11 = 'grid-cols-11';
const classGridCols12 = 'grid-cols-12';
const classGridCols: string[] = [
    classGridCols1,
    classGridCols2,
    classGridCols3,
    classGridCols4,
    classGridCols5,
    classGridCols6,
    classGridCols7,
    classGridCols8,
    classGridCols9,
    classGridCols10,
    classGridCols11,
    classGridCols12
];

</script>

<template>
  <div class="bg-white shadow">
    <div>
      <!-- Progress Indicator -->
      <div class="w-full">
        <div class="grid" :class="classGridCols[totalSteps - 1]">
          <div
            v-for="i in totalSteps"
            :key="i"
            class="py-1"
            :class="currentStep >= i ? 'bg-jasmine' : 'bg-silver'"
          />
        </div>
      </div>

      <!-- Content -->
      <slot />
    </div>
  </div>
</template>
