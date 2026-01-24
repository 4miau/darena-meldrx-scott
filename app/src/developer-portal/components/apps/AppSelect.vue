<script setup lang="ts">
import type SoFAppBaseDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppBaseDto';

interface IDsSelectItem<T> {
    value: T;
    title: string;
};

const props = defineProps<{
    apps: SoFAppBaseDto[];
    modelValue?: SoFAppBaseDto;
}>();

defineEmits<{
  'update:modelValue': [value?: SoFAppBaseDto]
}>();

const items = computed<IDsSelectItem<SoFAppBaseDto>[]>(() => {
    if (!props.apps) { return []; }

    return props.apps.map((app: SoFAppBaseDto) => {
        const selectItem: IDsSelectItem<SoFAppBaseDto> = {
            value: app,
            title: app.clientName
        };

        return selectItem;
    });
});
</script>

<template>
  <select>
    <option v-for="(item, index) in items" :key="`app-select-${index}`" :value="item.value" :selected="item.value.clientId === props.modelValue?.clientId">
      {{ item.title }}
    </option>
  </select>
</template>
