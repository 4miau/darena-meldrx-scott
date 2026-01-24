<!-- eslint-disable no-console -->
<script setup lang="ts">
import { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod';
import { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';
import type { INewLinkedApp } from '~/types/ui/apps/NewLinkedApp';
useHead({ title: 'Component Gallery | MeldRx' });

const showNewLinkedAppModal = ref(false);
const newLinkedApps = ref<INewLinkedApp[]>([]);

// When a linked app actually gets added from the modal...
function onAddNewLinkedApp (app?: INewLinkedApp) {
    if (app) {
        newLinkedApps.value.push(app);
    }
}

// When you click "Add Linked App" from the list...
function onAddLinkedApp() {
    showNewLinkedAppModal.value = true;
}

// When you click the "x" on a linked app...
function onDeleteLinkedApp(idx: number) {
    newLinkedApps.value = newLinkedApps.value.filter((_, i) => i !== idx);
}

function onEditLinkedApp(idx: number) {
    console.log('Editing linked app #', idx);
}

const fhirApiProviders = useFhirApiProviders()
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        New Linked App
      </DsText>
      <div class="pb-5" />

      <DsText size="xl">
        LinkedAppList (uses NewLinkedAppModal)
      </DsText>
      <div class="pb-4" />

      <LinkedAppList
        :linked-apps="newLinkedApps"
        @add-linked-app="onAddLinkedApp"
        @delete="onDeleteLinkedApp"
        @edit-linked-app="onEditLinkedApp"
      />

      <NewLinkedAppModal
        :auth-method="SoFAppTokenAuthMethod.Public"
        :user-type="SoFAppUserType.Patient"
        :show="showNewLinkedAppModal"
        :linked-apps="newLinkedApps"
        :fhir-api-providers="fhirApiProviders"
        :current-linked-apps="newLinkedApps"
        :organization-id="'myorg'"
        @add-linked-app="onAddNewLinkedApp"
        @close="showNewLinkedAppModal = false"
      />

      <div class="pb-4" />
      <pre>Linked Apps: {{ newLinkedApps }}</pre>
    </div>
  </DsContainer>
</template>
