<script setup lang="ts">
import { ref } from 'vue';
import {Colors} from '~/types/ui/colors'
useHead({ title: 'Component Gallery | MeldRx' });
const showModal = ref<boolean>(false);
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        Workspace Created
      </DsText>

      <WorkspaceCreatedModal
        :show="showModal"
        fhir-server-endpoint="http://google.com"
        @close="showModal = false"
        @go-to-workspace="showModal = false"
      />
      <DsButton :text-color='Colors.gray' @click="showModal = true">
        Show Modal
      </DsButton>
      <DsDivider />
    </div>
  </DsContainer>
</template>
