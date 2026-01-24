<script setup lang="ts">

defineProps<{
    showModal: boolean;
    organizationId: string;
}>();

defineEmits<{
    'close': [];
}>();

</script>

<template>
  <DsModal :model-value="showModal" @close="$emit('close')">
    <DsModalProgressCard :total-steps='1' :current-step='1'>
      <!-- Invite Member Icon-->
      <div class="flex w-full py-6 bg-space justify-center items-center">
        <div class="flex w-full items-center justify-center">
          <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center pt-2">
            <InviteMember />
          </div>
        </div>
        <div class="flex-1" />
      </div>

      <!-- Invite Member Details -->
      <div class="flex w-full py-4 px-8">
        <InviteMemberForm
          :organization-id="organizationId"
          @close-modal="$emit('close')"
        />
      </div>
    </DsModalProgressCard>
  </DsModal>
</template>
