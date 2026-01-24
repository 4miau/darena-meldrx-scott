<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import MeldRxMail from '~/components/svg/MeldRxMail.vue';

defineProps <{
    show: boolean,
    modelValue: string
}>();

const emit = defineEmits<{
  'close': [];
  'cancel': [];
  'update:modelValue': [string];
  'sendInvite': [];
}>();

function onSendInviteClick() {
    // Try to validate the form...
    if (!formRef.value) { return; }
    const isFormValid = formRef.value.validate();
    if (!isFormValid) {
        errorMessage.value = 'Please fix the errors above.';
        return;
    }

    emit('sendInvite');
}

const formRef = ref<FormRef>();
const errorMessage = ref<string>('');
</script>

<template>
  <DsModal :model-value="show" @close="$emit('close')">
    <DsModalProgressCard :total-steps="1" :current-step="1">
      <DsForm ref="formRef">
        <!-- Icon -->
        <div class="flex w-full py-8 bg-space justify-center items-center">
          <div class="flex w-full items-center justify-center">
            <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center pt-2">
              <MeldRxMail />
            </div>
          </div>
          <div class="flex-1" />
        </div>

        <div class="flex w-full p-4">
          <div class="flex flex-col w-full">
            <!-- Heading -->
            <div class="w-full flex-col justify-center items-center mb-5 flex">
              <DsText size="2xl" weight="light">
                Invite Patient
              </DsText>

              <DsText size="sm" weight="light" class="text-center">
                Provide email address to invite a patient to view their health records.
              </DsText>
            </div>

            <div class="flex justify-center w-full">
              <!-- Email -->
              <DsTextInput
                :model-value="modelValue"
                :rules="ValidationRules.email"
                type="email"
                label="Email Address"
                class="w-[330px]"
                @update:model-value="$emit('update:modelValue', $event as string)"
              />
            </div>
            <DsDivider class="my-2" />
            <!-- Buttons -->
            <div class="flex justify-center w-full gap-5">
              <DsButton :text-color='Colors.secondary' variant="transparent" @click="$emit('cancel')">
                Cancel
              </DsButton>
              <DsButton :color="Colors.secondary" @click="onSendInviteClick">
                Send Invitation
              </DsButton>
            </div>
          </div>
        </div>
      </DsForm>
    </DsModalProgressCard>
  </DsModal>
</template>
