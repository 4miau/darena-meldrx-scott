<script setup lang="ts">
import { Colors } from '~/types/ui/colors';

const props = defineProps<{
    modelValue: string[];
    rules?: ValidationRule<string[]>[];
    limit?: number;
}>();

const emit = defineEmits<{
    'update:modelValue': [value: string[]];
}>();

const { dirty, validationResult } = useValidation<string[]>(
    () => props.modelValue,
    () => props.rules,
    () => true
);

function onAddRedirectUrl() {
    dirty.value = true;
    emit('update:modelValue', [...props.modelValue, '']);
}

function onRemoveRedirectUrl(index: number) {
    dirty.value = true;
    emit('update:modelValue', props.modelValue.filter((_x, i) => i !== index))
}

function onEditRedirectUrl(value: string, index: number) {
    dirty.value = true;
    emit('update:modelValue', props.modelValue.map((url, i) => i === index ? value : url))
}

const limitReached = computed(() => props.limit !== undefined && props.limit <= props.modelValue.length)

</script>

<template>
  <DsLabeledInput label="Redirect URLs" required>
    <RedirectUrlListItem
      v-for="(redirectUrl, index) in modelValue"
      :id="'redirect-url-' + index"
      :key="index"
      :redirect-url="redirectUrl"
      @update:redirect-url="(value) => onEditRedirectUrl(value, index)"
      @delete="() => onRemoveRedirectUrl(index)"
    />
    <DsButton
      v-if="!limitReached"
      id="add-a-redirect-url-button"
      variant="transparent"
      :color="Colors.fire"
      :text-color='Colors.fire'
      @click="onAddRedirectUrl"
    >
      + Add a Redirect URL
    </DsButton>
    <div v-if="!validationResult.isValid" class="relative mb-6">
      <DsText id="redirect-url-error" size="xs" :color="Colors.fire" class="absolute top-0">
        {{ validationResult.error }}
      </DsText>
    </div>
  </DsLabeledInput>
</template>
