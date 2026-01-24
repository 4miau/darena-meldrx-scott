<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import type { SourceAttributeGroup } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/SourceAttribute';

const props = defineProps<{
  title: string;
  subTitle: string;
  description?: string;
  sourceAttributes: SourceAttributeGroup[];
  originalSourceAttributes?: SourceAttributeGroup[];
  formEntryDisabled: boolean;
}>()

const emit = defineEmits<{
    'onCancel': [];
    'onSave': [sourceAttributes:SourceAttributeGroup[]];
}>();

const formRefDsi = ref<FormRef>();
const state = reactive<{
  error: string;
  showOriginalSourceAttributes: boolean;
}>({
    error: '',
    showOriginalSourceAttributes: false
})

function onSaveSourceAttributes() {
    if (!formRefDsi.value?.validate()) {
        state.error = 'Please fix the errors above.';
        return;
    }

    if (props.sourceAttributes) {
        emit('onSave', props.sourceAttributes);
    }
}
</script>

<template>
  <!-- Header Information -->
  <div class="flex flex-col w-full py-1 justify-items-start">
    <div class="flex flex-col mb-4">
      <DsText v-if="props.title" size="2xl">
        {{ props.title }}
      </DsText>
      <DsIcon name="heroicons:x-mark" size="sm" class="absolute top-4 right-4 cursor-pointer" @click="emit('onCancel')"/>
    <DsText v-if="props.title" size="sm">
      {{ props.subTitle }}
    </DsText>
    </div>
    <DsText v-if="props.description" size="lg">
      {{ props.description }}
    </DsText>
    <DsDivider/>

  <slot name="additional"/>

  <!-- Source Attributes Section -->
    <div v-if="props.sourceAttributes && props.sourceAttributes.length > 0" class="block pb-4">
        <DsLabeledInput v-if="props.originalSourceAttributes" label="Show Original Source Attributes" class="py-2">
          <DsToggle :model-value="state.showOriginalSourceAttributes" @update:model-value="state.showOriginalSourceAttributes = !state.showOriginalSourceAttributes"/>
        </DsLabeledInput>
      <DsForm ref="formRefDsi">
        <div class="grid grid-cols-1 flex-grow">
          <div
              v-for="group in state.showOriginalSourceAttributes ? props.originalSourceAttributes : props.sourceAttributes"
              :key="group.id"
              class="flex flex-col w-full justify-items-start"
          >
            <div>
              <DsText weight="bold" size="md">
                {{ group.description }}
              </DsText>
            </div>
            <div class="space-y-4">
              <div v-for="item in group.sourceAttributeItems" :key="item.id">
                <DsLabeledInput :label="item.display" required>
                  <template #popoverarea>
                    <DsIcon name="heroicons:information-circle" size="sm"/>
                  </template>
                  <template #popovercontent>
                    <DsText size="sm">
                      {{ item.description }}
                    </DsText>
                  </template>
                  <DsTextArea
                      v-model="item.answer"
                      required
                      :disabled="formEntryDisabled || state.showOriginalSourceAttributes"
                      :rules="[
                          [v => !!v, 'This field is required'],
                          [v => v!.trim().length > 0, 'This field is required'],
                          [v => v!.length <= 200, 'This field must be 200 characters or less']
                      ]"
                  />
                </DsLabeledInput>
              </div>
            </div>
            <DsDivider/>
          </div>
        </div>
      </DsForm>
    </div>
  </div>

  <!-- Footer Section -->
  <div v-if="!formEntryDisabled && props.sourceAttributes.length > 0" class="flex justify-start gap-5">
    <DsButton id='cancel-button' :color="Colors.white" :text-color='Colors.gray' variant="transparent" @click="emit('onCancel')">
      Cancel
    </DsButton>
    <DsButton id='save-attributes-button' :color="Colors.primary" @click="onSaveSourceAttributes()">
      Save Attributes
    </DsButton>
  </div>
  <div v-if="props.sourceAttributes.length == 0" class="flex flex-col space-y-3">
    <DsText>
      This extension has no source attributes
    </DsText>
    <div>
      <DsButton id='close-button' :color="Colors.white" :text-color='Colors.gray' size="md" variant="outline" @click="emit('onCancel')">
        Close
      </DsButton>
    </div>
  </div>
  <div v-if="!!state.error" class="block pb-2">
    <div class="p-4">
      <DsText size="sm" :color="Colors.fire">
        {{ state.error }}
      </DsText>
    </div>
  </div>
</template>
