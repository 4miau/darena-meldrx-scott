<script setup lang="ts">
type SupportedLanguage = 'javascript' | 'bash';


const props = defineProps<{
  code: string;
  codeToCopy?: string;
  language: SupportedLanguage;
  showToastOnCopy?: boolean;
  toastMessageOnCopy?: string;
}>();

function onCopyClick() {
    copyToClipboard(props.codeToCopy ?? props.code);

    if (props.showToastOnCopy) {
        notification({
            title: 'Copied',
            description: props.toastMessageOnCopy ?? 'Copied to clipboard.',
            displayTime: 3000,
            variant: 'success',
        });
    }
}

</script>

<template>
  <div>
    <div class="relative">
      <pre><DsIcon
          name="heroicons:document-duplicate"
          size="xs"
          class="absolute top-0 right-0 cursor-pointer mt-[5px] mr-[9px] w-[18px] h-[18px]"
          @click="onCopyClick"
      /><code v-code-highlight :class="language">{{ code }}</code></pre>
    </div>
  </div>
</template>
