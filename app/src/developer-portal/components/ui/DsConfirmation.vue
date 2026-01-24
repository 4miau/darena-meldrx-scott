<script setup lang="ts">
import { Colors } from '~/types/ui/colors'

defineProps<{
  active?: boolean;
  message?: string;
  confirmationMessage: string;
}>()

const emit = defineEmits<{
  'close': [];
  'confirm': [];
}>()

const state = reactive({
    active: false
})

function cancel () {
    state.active = false
    emit('close')
}

function confirm () {
    state.active = false
    emit('confirm')
}

</script>

<template>
  <slot :show="() => state.active = true"/>
  <DsModal :model-value="active || state.active" @close="cancel">
    <div class="grid grid-cols-8 p-5">
      <div class="col-span-1 flex items-center justify-center bg-[rgba(186,36,36,0.25)] rounded-full w-[50px] h-[50px]">
        <DsIcon name="heroicons:exclamation-triangle" size="xl" :color='Colors.fire'/>
      </div>
      <div class="col-span-7">
        <DsText size="xl">
          Are you sure?
        </DsText>
      </div>
      <div class="col-start-2 col-span-7 space-y-5">
        <div>
          <DsText size="sm">
            <template v-if="message">
              {{ message }}
            </template>
            <template v-else>
              <slot name="message"/>
            </template>
          </DsText>
        </div>
        <div>
          <DsButton :flex='false' title="Cancel" :color="Colors.white" :text-color='Colors.gray' class="mr-4" @click="cancel">
            Cancel
          </DsButton>
          <DsButton
              :id="confirmationMessage.replaceAll(' ','-').toLowerCase()+'-confirm-button'"
              :flex='false'
              :color='Colors.fire'
              :text-color="Colors.white"
              @click="confirm"
          >
            {{ confirmationMessage }}
          </DsButton>
        </div>
      </div>
    </div>
  </DsModal>
</template>
