<script setup lang="ts">


const props = defineProps<{
    textToCopy: string;
    showToastOnCopy?: boolean;
    toastMessageOnCopy?: string;
}>();

function onClick() {
    copyToClipboard(props.textToCopy);

    if (props.showToastOnCopy) {
        notification({
            title: 'Copied',
            description: props.toastMessageOnCopy ?? 'Copied to clipboard.',
            displayTime: 3000,
            variant: 'success'
        })
    }
}

</script>

<template>
  <div class="flex items-center cursor-pointer" @click="onClick">
    <DsIcon
      name="heroicons:document-duplicate"
      size="xs"
      class="mr-1"
    />
    <slot />
  </div>
</template>
