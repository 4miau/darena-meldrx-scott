<script setup lang="ts">
import type { TextSize, TextWeight } from '~/types/ui/DsText';

defineProps<{
    id?: string;
    size: TextSize;
    weight?: TextWeight;
    text: string;
    textToCopy?: string;
    showToastOnCopy?: boolean;
    toastMessageOnCopy?: string;
}>();

</script>

<template>
  <DsCopyButton :text-to-copy="textToCopy ?? text" :show-toast-on-copy="showToastOnCopy" :toast-message-on-copy="toastMessageOnCopy">
    <DsText :id="id" :size="size" :weight="weight" class="hover:underline mt-1">
      {{ text }}
    </DsText>
  </DsCopyButton>
</template>
