<script setup lang="ts">
import type FhirApiProviderDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';
import type { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod';
import type { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';
import type { INewLinkedApp } from '~/types/ui/apps/NewLinkedApp';
import { Colors } from '~/types/ui/colors';

const props = defineProps<{
  authMethod: SoFAppTokenAuthMethod;
  userType: SoFAppUserType;
  fhirApiProviders: FhirApiProviderDto[];
  linkedApps: INewLinkedApp[];
  organizationId: string;
}>();

const emit = defineEmits<{
  'update:linkedApps': [value: INewLinkedApp[]];
  'createApp': [];
  'next': [];
  'previous': [];
  'cancel': [];
  'deleteLinkedApp': [linkedAppIndex: number];
  'addLinkedApp': [appDetails?: INewLinkedApp];
}>();

const showNewModal = ref<boolean>(false);

const selectedLinkedAppIndex = ref<number | null>(null)
const selectedLinkedApp = computed(() =>
    selectedLinkedAppIndex.value === null
        ? null
        : props.linkedApps[selectedLinkedAppIndex.value]
)

function onEditSave(linkedApp: INewLinkedApp) {
    emit('update:linkedApps', props.linkedApps.map((x, i) => i === selectedLinkedAppIndex.value ? linkedApp : x))
    closeEditModal()
}

function closeEditModal() {
    selectedLinkedAppIndex.value = null
}

</script>

<template>
  <div>
    <!-- App Details -->
    <div class="grid md:grid-cols-2 gap-2">
      <div>
        <DsLabeledText
          label="Connect Linked Apps"
          text="Select the external systems your application will interact with."
        />
      </div>

      <div>
        <LinkedAppList
          :linked-apps="linkedApps"
          @add-linked-app="showNewModal = true"
          @delete="$emit('deleteLinkedApp', $event)"
          @edit-linked-app="selectedLinkedAppIndex = $event"
        />
      </div>
    </div>

    <!-- Cancel/Previous/Next Buttons -->
    <DsDivider />
    <div class="justify-start items-start gap-5 inline-flex">
      <DsButton :color="Colors.white" :text-color='Colors.gray' variant="subtle" @click="$emit('cancel');">
        Cancel
      </DsButton>
      <DsButton :color="Colors.primary" :text-color='Colors.primary' variant="outline" @click="$emit('previous');">
        Previous Step
      </DsButton>
      <DsButton :color="Colors.primary" @click="$emit('createApp')">
        Register App
      </DsButton>
    </div>

    <!-- New Linked App Modal-->
    <NewLinkedAppModal
      :show="showNewModal"
      :auth-method="authMethod"
      :user-type="userType"
      :linked-apps="linkedApps"
      :fhir-api-providers="fhirApiProviders"
      :current-linked-apps="linkedApps"
      :organization-id="organizationId"
      @add-linked-app="$emit('addLinkedApp', $event)"
      @close="showNewModal = false"
    />

    <!-- Edit Linked App Modal-->
    <EditLinkedApp
      v-if="selectedLinkedApp"
      :model-value="selectedLinkedApp"
      :show="!!selectedLinkedApp"
      :fhir-api-providers="fhirApiProviders"
      :current-linked-apps="linkedApps"
      :organization-id="organizationId"
      @close="closeEditModal"
      @save="onEditSave"
    />
  </div>
</template>
