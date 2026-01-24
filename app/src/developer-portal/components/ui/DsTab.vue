<script setup lang="ts">
import { Colors } from "~/types/ui/colors";

defineProps<{
  labels: string[];
  selectedTab: number;
}>();

const emit = defineEmits<{
  'click': number[]
}>();

const handleClick = (index: number) => {
    emit('click', index);
};

const selectedColor = 'border-indigo bg-silver text-onyx';
const defaultColor = 'border-silver hover:border-indigo';
</script>

<template>
  <div v-for="(label, index) in labels" :key="index" :class="selectedTab === index ? 'bg-silver' : ''" class="px-2 py-1 cursor-pointer hover:bg-silver">
    <div
        :class="selectedTab === index ? selectedColor : defaultColor"
        class="flex space-x-2 px-3 py-1 border-b-[1px]"
        @click="handleClick(index)"
    >
      <DsText size="md" weight="normal" :color="Colors.onyx">
        {{ label }}
      </DsText>
      <slot :name="label.toLowerCase()" />
    </div>
  </div>
</template>
