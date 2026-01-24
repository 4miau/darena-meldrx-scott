<script setup lang="ts">
import type { Group } from 'fhir/r4';
import type { IDsSelectItem } from '~/types/ui/DsSelect';
import { Colors } from '~/types/ui/colors';

interface IProps {
    show: boolean;
    groups: Group[];
};

// Set default properties...
const props = withDefaults(defineProps<IProps>(), {
    show: false
});

const emit = defineEmits<{
    'addToGroup': [value?: Group];
    'close': [];
    'cancel': [];
}>();

const onAddToGroupClick = () => {
    emit('addToGroup', selectedGroup.value);
};

const selectedGroup = ref<Group>();
const formRef = ref<FormRef>();

const groupOptions = computed<IDsSelectItem<Group>[]>(() => props.groups.map((group: Group) => ({ value: group, title: group.name ?? '(No name)' })));

</script>

<template>
  <DsModal :model-value="show" @close="$emit('close')">
    <DsModalProgressCard :total-steps="1" :current-step="1">
      <DsForm ref="formRef">
        <!-- Icon -->
        <div class="flex w-full py-8 bg-space justify-center items-center">
          <div class="flex w-full items-center justify-center">
            <div class="w-14 h-14 rounded-full flex justify-center items-center pt-2" style="background-color:#E79288">
              <MeldRxClipboard class="mb-2" />
            </div>
          </div>
          <div class="flex-1" />
        </div>

        <div class="flex w-full p-4">
          <div class="flex flex-col w-full">
            <!-- Heading -->
            <div class="w-full flex-col justify-center items-center gap-5 flex">
              <DsText size="2xl" weight="light">
                Add to Group
              </DsText>

              <DsText size="sm" weight="light" class="text-center">
                Select a group to add this patient to.
              </DsText>
            </div>

            <div class="grid grid-cols-1 gap-4">
              <DsSelect
                label="Group Name"
                :model-value="selectedGroup"
                :items="groupOptions"
                @update:model-value="selectedGroup = $event"
              />
            </div>

            <!-- Buttons -->
            <DsContainer>
              <div class="flex justify-center w-full gap-5">
                <DsButton id="cancel-button" :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" @click="$emit('cancel')">
                  Cancel
                </DsButton>
                <DsButton id="add-to-group-button" :color="Colors.secondary" @click="() => onAddToGroupClick()">
                  Add to Group
                </DsButton>
              </div>
            </DsContainer>
          </div>
        </div>
      </DsForm>
    </DsModalProgressCard>
  </DsModal>
</template>
