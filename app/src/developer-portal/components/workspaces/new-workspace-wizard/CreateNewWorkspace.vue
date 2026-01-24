<script setup lang="ts">
import type { INewWorkspace } from '~/types/meldrx-api/workspaces';
import type FhirApiProviderDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';

const props = defineProps<{
  modelValue: INewWorkspace;
  fhirApiProviders: FhirApiProviderDto[];
  showCanSellWorkspaceData: boolean;
  showCanChangeSlug: boolean;
  showCanAddLiteWorkspace: boolean;
}>();

const emit = defineEmits<{
  'update:modelValue': [value: INewWorkspace];
  'createWorkspace': [];
  'cancel': [];
}>();

const currentTab: Ref<number> = ref(0);
const localForm = ref<INewWorkspace>({ ...props.modelValue });
watch(localForm.value, (newValue: INewWorkspace) => {
    emit('update:modelValue', newValue);
});
</script>

<template>
  <div class="w-full flex-col justify-start items-start gap-5 inline-flex">
    <!-- Step 1 / Step 2 -->
    <div>
      <!-- Tab Headers -->
      <div class="justify-start items-start gap-1.5 inline-flex">
        <DsTabHeader title="Step 1" subtitle="Choose workspace type" :enabled="currentTab >= 0" @click="() => {}" />
        <DsTabHeader title="Step 2" subtitle="Provide workspace details" :enabled="currentTab >= 1" @click="() => {}" />
      </div>
      <div class="pb-5" />

      <!-- Tab 1 Content (Step 1) -->
      <div v-if="currentTab === 0">
        <Step01ChooseWorkspaceTypePanel
          v-model:workspace-type="localForm.workspaceType"
          @next="currentTab = 1"
          @cancel="$emit('cancel')"
        />
      </div>

      <!-- Tab 2 Content (Step 2) -->
      <div v-if="currentTab === 1">
        <div v-if="props.modelValue?.workspaceType === 'linked'">
          <Step02LinkedWorkspaceDetailsPanel
            v-model:workspace-name="localForm.name"
            v-model:validation-option="localForm.validationOption"
            v-model:patient-strategy="localForm.patientStrategy"
            v-model:email="localForm.email"
            v-model:first-name="localForm.firstName"
            v-model:last-name="localForm.lastName"
            v-model:organization-name="localForm.organizationName"
            v-model:organization-identifier="localForm.organizationIdentifier"
            v-model:fhir-url="localForm.fhirUrl"
            v-model:fhir-api-provider-meld-rx-identifier="localForm.fhirApiProviderId"
            v-model:workspace-slug="localForm.workspaceSlug"
            v-model:app-id="localForm.appId"
            :show-can-sell-workspace-data="showCanSellWorkspaceData"
            :show-can-change-slug="showCanChangeSlug"
            :workspace-id="''"
            :fhir-server-endpoint="''"
            :fhir-api-providers="props.fhirApiProviders"
            :current-linked-apps="[]"
            success-button-name="Create Workspace"
            :show-previous-step-button="true"
            @success="$emit('createWorkspace')"
            @previous="currentTab = 0"
            @cancel="$emit('cancel')"
          />
        </div>

        <div v-if="props.modelValue?.workspaceType === 'standalone'">
          <Step02StandaloneWorkspaceDetails
            v-model:workspace-name="localForm.name"
            v-model:validation-option="localForm.validationOption"
            v-model:email="localForm.email"
            v-model:first-name="localForm.firstName"
            v-model:last-name="localForm.lastName"
            v-model:organization-name="localForm.organizationName"
            v-model:organization-identifier="localForm.organizationIdentifier"
            v-model:workspace-slug="localForm.workspaceSlug"
            v-model:app-id="localForm.appId"
            v-model:is-lite-workspace="localForm.isLiteWorkspace"
            :show-can-sell-workspace-data="showCanSellWorkspaceData"
            :show-can-change-slug="showCanChangeSlug"
            :show-can-add-lite-workspace="showCanAddLiteWorkspace"
            success-button-name="Create Workspace"
            :workspace-id="''"
            :fhir-server-endpoint="''"
            :show-previous-step-button="true"
            @success="$emit('createWorkspace')"
            @previous="currentTab = 0"
            @cancel="$emit('cancel')"
          />
        </div>
      </div>
    </div>
  </div>
</template>
