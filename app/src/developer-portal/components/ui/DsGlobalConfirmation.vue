<script setup lang="ts">
const { state, hide } = useConfirmationState()

function cancel() {
    state.value.cancelCallback()
    hide()
}

function confirm() {
    state.value.confirmCallback()
    hide()
}

</script>

<template>
  <DsConfirmation
    :active="state.active"
    :message="state.message"
    :confirmation-message="state.confirmationMessage"
    @close="cancel"
    @confirm="confirm"
  />
</template>
