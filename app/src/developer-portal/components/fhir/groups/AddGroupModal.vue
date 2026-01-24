<script setup lang="ts">
import { Colors } from '~/types/ui/colors';

interface IProps {
    show: boolean;
    mode: 'create' | 'edit';
    modelValue: { name: string };
};

// Set default properties...
withDefaults(defineProps<IProps>(), {
    show: false,
    mode: 'create',
    modelValue: () => { return { name: '' } }
});

const emit = defineEmits<{
    'update:modelValue': [value: { name: string }];
    'close': [];
    'cancel': [];
    'createGroup': [value: { name: string }];
    'editGroup': [value: { name: string }];
}>();

// "Create Group"...
function onCreateGroupClick(modelValue: { name: string }) {
    // Try to validate the form...
    if (!formRef.value) { return; }
    const isFormValid = formRef.value.validate();
    if (!isFormValid) { return; }

    emit('createGroup', modelValue);
}

// "Edit Group"...
function onEditGroupClick(modelValue: { name: string }) {
    // Try to validate the form...
    if (!formRef.value) { return; }
    const isFormValid = formRef.value.validate();
    if (!isFormValid) { return; }

    emit('editGroup', modelValue);
}

const formRef = ref<FormRef>();
</script>

<template>
  <DsModal :model-value="show" @close="$emit('close')">
    <DsModalProgressCard :total-steps="1" :current-step="1">
      <DsForm ref="formRef">
        <!-- Icon -->
        <div class="flex w-full py-8 bg-space justify-center items-center">
          <div class="flex w-full items-center justify-center">
            <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center pt-2">
              <MeldRxLock />
            </div>
          </div>
          <div class="flex-1" />
        </div>

        <div class="flex w-full p-4">
          <div class="flex flex-col w-full">
            <!-- Heading -->
            <div class="w-full flex-col justify-center items-center gap-5 flex">
              <DsText size="2xl" weight="light">
                {{ mode === 'create' ? 'Create Group' : 'Edit Group' }}
              </DsText>

              <DsText size="sm" weight="light" class="text-center">
                {{ mode === 'create' ? 'Provide details to create the group.' : 'Provide details to edit the group.' }}
              </DsText>
            </div>

            <div class="grid grid-cols-1 gap-4 pt-4">
              <!-- Name -->
              <DsTextInput
                :required="true"
                :rules="[
                  [v => !!v, 'Name is required'],
                  [v => !!v && !v.includes(' '), 'Name cannot contain spaces']
                ]"
                :model-value="modelValue.name"
                type="text"
                label="Name"
                @update:model-value="$emit('update:modelValue', { ...modelValue, name: $event ?? '' })"
              />
            </div>

            <DsDivider />

            <!-- Buttons -->
            <div class="flex justify-center w-full gap-5">
              <DsButton :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" @click="$emit('cancel')">
                Cancel
              </DsButton>
              <DsButton v-if="mode === 'create'" id="create-group-modal-button" :color="Colors.secondary" @click="() => onCreateGroupClick(modelValue)">
                Create Group
              </DsButton>
              <DsButton v-if="mode === 'edit'" :color="Colors.secondary" @click="() => onEditGroupClick(modelValue)">
                Edit Group
              </DsButton>
            </div>
          </div>
        </div>
      </DsForm>
    </DsModalProgressCard>
  </DsModal>
</template>
