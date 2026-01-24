<script setup lang="ts">
import {Colors} from "~/types/ui/colors";

const prop = defineProps<{
  modelValue: string;
  required?: boolean;
  rules?: ValidationRule<string[]>[];
  scopes: ReadonlyArray<string>;
}>();

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>();


const localScopes = computed({
    get() {
        if (!prop.modelValue || prop.modelValue === '') {
            return [];
        }
        return prop.modelValue.split(' ').filter(Boolean);
    },
    set(value: string[]) {
        emit('update:modelValue', value.join(' '))
    }
});

watch(() => prop.scopes, () => {
    localScopes.value = localScopes.value.filter(scope => prop.scopes.includes(scope))
})

const convertedScopes = computed(() => prop.scopes.map(scope => ({value: scope, title: scope})))
const allOptionsSelected = computed(() => {
    return prop.scopes.every(scope => prop.modelValue.includes(scope))
})

function selectAll() {
    if (allOptionsSelected.value) {
        emit('update:modelValue', '');
    } else {
        const buffer = [...prop.modelValue.split(' ')]

        for (let i = 0; i < prop.scopes.length; i++) {
            const option = prop.scopes[i];
            if (!prop.modelValue.includes(option)) {
                buffer.push(option)
            }
        }

        emit('update:modelValue', buffer.join(' '));
    }
};

function onCopy() {
    copyToClipboard(prop.modelValue);
    notification({
        title: 'Copied',
        description: 'Scopes copied to the clipboard',
        displayTime: 3000,
        variant: 'success'
    });
}
</script>

<template>
  <div>
    <DsSelect
      v-model="localScopes"
      :items="convertedScopes"
      placeholder="Select scopes"
      searchable
      search-placeholder="Search a scope..."
      label="Scopes"
      :required="required"
      :rules="rules"
      multiple
    />
    <div class="flex py-2 space-x-2">
      <DsButton variant='outline' :color='Colors.secondary' :text-color='Colors.secondary' tile @click='selectAll()'>
        {{ allOptionsSelected ? 'Deselect All' : 'Select All' }}
      </DsButton>
      <DsButton variant='outline' :text-color='Colors.primary' tile @click='onCopy'>
        <DsIcon name="heroicons:document-duplicate" size="xs" />
        Copy
      </DsButton>
    </div>
  </div>
</template>
