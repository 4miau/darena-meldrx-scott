<!--
    DsColorPicker
    Usage:
        <DsColorPicker v-model="formModel.color" />
-->
<script setup lang="ts">

defineProps<{
    modelValue: string|undefined;
}>();

const emit = defineEmits<{
  'update:modelValue': [string?]
}>();

</script>

<template>
    <div class="flex flex-col items-start gap-5">
        <input
            :value="modelValue"
            type="color"
            style="width: 50px; height: 50px; border: 1px; cursor: pointer;"
            @input="($e) => { emit('update:modelValue', ($e.target as HTMLInputElement).value); }"
        >
        <DsText size="xs" >Current Color: {{ modelValue }}</DsText>
    </div>
</template>
