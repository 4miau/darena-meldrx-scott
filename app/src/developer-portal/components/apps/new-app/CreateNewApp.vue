<!--
    Full form to use for creating a new app.
    Contains all the steps for creating a new app.
-->

<script setup lang="ts">
import type FhirApiProviderDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';
import type { INewApp } from '~/types/ui/apps/NewApp';
import type { INewLinkedApp } from '~/types/ui/apps/NewLinkedApp';
import type { CreateCdsHooksAppCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CreateCdsHooksAppCommand'

const confirmation = useConfirmation();

const props = defineProps<{
    modelValue: INewApp;
    fhirApiProviders: FhirApiProviderDto[];
    organizationId: string;
}>();

const emit = defineEmits<{
  'update:modelValue': [value: INewApp];
  'createApp': [];
  'createCdsHookApp': [value: CreateCdsHooksAppCommand];
  'cancel': [];
}>();

const localForm = ref<INewApp>({ ...props.modelValue });
watch(localForm.value, (newValue: INewApp) => {
    emit('update:modelValue', newValue);
});

// Delete Linked App...
async function deleteLinkedApp(index: number) {
    // Check if user really wants to delete it first...
    const { isCancelled } = await confirmation(
        'Are you sure you want to delete this linked app? This action cannot be undone.',
        'Delete Linked App'
    );
    if (isCancelled) { return; }

    localForm.value.linkedApps = localForm.value.linkedApps.filter((_, i) => i !== index)
}

// Add new linked app...
function addLinkedApp(appDetails?: INewLinkedApp) {
    if (appDetails) {
        localForm.value.linkedApps.push(appDetails)
    }
}

const currentTab: Ref<number> = ref(0);

</script>

<template>
  <div>
    <!-- Step 1 instructions -->
    <DsText v-if="currentTab === 0" size="sm" weight="light">
      Choose the type and user base of your application.
    </DsText>

    <!-- Step 2 instructions -->
    <DsText v-if="currentTab === 1" size="sm" weight="light">
      Register a new application to begin crafting your healthcare solution.
    </DsText>

    <!-- Step 3 instructions -->
    <DsText v-if="currentTab === 2" size="sm" weight="light">
      Link your application to external EHR systems.
    </DsText>
    <div class="py-2" />

    <!-- Tab Headers -->
    <div class="justify-start items-start gap-1.5 inline-flex">
      <DsTabHeader title="Step 1" subtitle="Choose app type" :enabled="currentTab >= 0" @click="() => {}" />
      <DsTabHeader title="Step 2" subtitle="Provide app details" :enabled="currentTab >= 1" @click="() => {}" />
      <DsTabHeader v-if="localForm.userType != 'CdsHooks'" title="Step 3" subtitle="Connect linked apps" :enabled="currentTab >= 2" @click="() => {}" />
    </div>
    <div class="py-4" />

    <!-- Tab 1 -->
    <div v-if="currentTab === 0">
      <Step01AppType
          v-model:app-type="localForm.userType"
          @next="() => currentTab = 1"
          @cancel="() => $emit('cancel')"
      />
    </div>

    <!-- Tab 2 -->
    <div v-if="currentTab === 1">
      <Step02AppDetails
          v-model:app-name="localForm.appName"
          v-model:user-type="localForm.userType"
          v-model:authentication-client-type="localForm.authenticationClientType"
          v-model:scopes="localForm.scopes"
          v-model:redirect-urls="localForm.redirectUrls"
          v-model:ehr-launch-url="localForm.ehrLaunchUrl"
          @previous="() => currentTab = 0"
          @next="() => currentTab = 2"
          @cancel="() => $emit('cancel')"
          @create-cds-hook-app="$emit('createCdsHookApp', $event)"
      />
    </div>

    <!-- Tab 3 -->
    <div v-if="currentTab === 2">
      <Step03LinkedApps
        :auth-method="modelValue.authenticationClientType"
        :user-type="modelValue.userType"
        :linked-apps="localForm.linkedApps"
        :fhir-api-providers="fhirApiProviders"
        :organization-id="organizationId"
        @update:linked-apps="localForm.linkedApps = $event"
        @previous="() => currentTab = 1"
        @create-app="() => $emit('createApp')"
        @cancel="() => $emit('cancel')"
        @delete-linked-app="deleteLinkedApp"
        @add-linked-app="addLinkedApp"
      />
    </div>
  </div>
</template>
