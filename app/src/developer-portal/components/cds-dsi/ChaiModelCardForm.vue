<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import type {
    ChaiModelCardGroup,
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/ChaiModelCardForm";

const props = defineProps<{
  title: string;
  subTitle: string;
  modelCardForm: ChaiModelCardGroup[];
  formEntryDisabled?: boolean;
}>()

const emit = defineEmits<{
  'onCancel': [];
  'onSave': [modelCardForm:ChaiModelCardGroup[]];
}>();

const formRefDsi = ref<FormRef>();
const state = reactive<{
  error: string;
}>({
    error: ''
})

function onSaveModelCard() {
    if (!formRefDsi.value?.validate()) {
        state.error = 'Please fix the errors above.';
        return;
    }

    if (props.modelCardForm) {
        emit('onSave', props.modelCardForm);
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
    <DsText size="lg">
      View and modify the CHAI Model Card for this application.
    </DsText>
    <DsDivider/>

    <slot name="additional"/>

    <!-- Model Card Section -->
    <div v-if="props.modelCardForm && props.modelCardForm.length > 0" class="block pb-4">
      <DsForm ref="formRefDsi">
        <div class="grid grid-cols-1 flex-grow overflow-auto">
          <div
              v-for="group in props.modelCardForm"
              :key="group.id"
              class="flex flex-col w-full justify-items-start"
          >
            <DsText size="md" weight="bold">
              {{ group.display}}
            </DsText>
            <div class="space-y-4">
              <div v-for="item in group.chaiModelCardItems" :key="item.id">
                <DsLabeledInput :label="item.display" required>
                  <DsTextArea
                      v-model="item.answer"
                      required
                      :disabled="formEntryDisabled"
                      :rules="[
                          [v => !!v, 'This field is required'],
                          [v => v!.trim().length > 0, 'This field is required'],
                          [v => v!.length <= 2000, 'This field must be 2000 characters or less']
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
  <div v-if="!formEntryDisabled && props.modelCardForm.length > 0" class="flex justify-start gap-5">
    <DsButton id='cancel-button' :color="Colors.white" :text-color='Colors.gray' variant="transparent" @click="emit('onCancel')">
      Cancel
    </DsButton>
    <DsButton id='save-model-card' :color="Colors.primary" @click="onSaveModelCard()">
      Save Model Card
    </DsButton>
  </div>
  <div v-if="!!state.error" class="block pb-2">
    <div class="p-4">
      <DsText size="sm" :color="Colors.fire">
        {{ state.error }}
      </DsText>
    </div>
  </div>
</template>
