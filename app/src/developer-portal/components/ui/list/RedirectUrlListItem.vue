<script setup lang="ts">
import { Colors } from '~/types/ui/colors';

defineProps<{
  redirectUrl: string;
}>();

defineEmits<{
  'update:redirectUrl': [value: string];
  'delete': [];
}>();

</script>

<template>
  <div class="flex flex-row items-start border-b border-bliss py-2">
    <DsTextInput
      :model-value="redirectUrl"
      placeholder="https://example.com"
      class="grow"
      required
      :rules="ValidationRules.url"
      @update:model-value="$emit('update:redirectUrl', $event ?? '')"
    />
    <DsButton variant="subtle" :color="Colors.white" :text-color='Colors.gray' @click="$emit('delete');">
      x
    </DsButton>
  </div>
</template>
