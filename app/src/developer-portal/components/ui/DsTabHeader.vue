<!--
    Tab header with a yellow stripe.
    Can be "enabled" or "disabled" (different colors).

    Usage:
        <TabHeader
            title="Step 1"
            subtitle="Choose workspace type"
            :enabled="currentTab === 0"
            @click="currentTab = 0"
        />
-->

<script setup lang="ts">
import { Colors } from "~/types/ui/colors";

defineProps<{
    title: string;
    subtitle?: string;
    enabled: boolean;
}>();

defineEmits<{
  'click': []
}>();

// CSS classes for enabled and disabled...
const classContainerEnabled = 'py-1 pr-20 border-t-4 border-jasmine flex-col justify-start items-start inline-flex cursor-pointer bg-white hover:bg-bliss';
const classContainerDisabled = 'py-1 pr-20 border-t-4 border-silver flex-col justify-start items-start inline-flex';

</script>

<template>
  <div :class="enabled ? classContainerEnabled : classContainerDisabled" @click="$emit('click')">
    <DsText size="sm" weight="light" :color="enabled ? Colors.jasmine : Colors.silver">
      {{ title }}
    </DsText>
    <DsText v-if="!!subtitle" size="sm" weight="light" :color="enabled ? Colors.onyx : Colors.silver">
      {{ subtitle }}
    </DsText>
  </div>
</template>
