<script setup lang="ts">
import type { IEhrMetadata, ISandboxServer } from '~/types/EhrMetadata';
import type FhirApiProviderDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/FhirApiProviderDto';
import FhirApiProviderUtils from '~/utils/FhirApiProviderUtils';
useHead({ title: 'FHIR API Providers | MeldRx' });
definePageMeta({ anonymous: true });
const { $api } = useNuxtApp();


const tableHeaders = ['Login Type', 'Username', 'Password'];

const state = reactive<{
  isLoading: boolean;
  isLoadingSandboxValidation: boolean;
  selectedSandboxFhirUrl: string;
  selectedSandboxStatus?: 'healthy' | 'unhealthy';
  fhirApiProvider?: FhirApiProviderDto;
  query: {
    id: string;
  }
}>({
    isLoading: false,
    isLoadingSandboxValidation: false,
    selectedSandboxFhirUrl: '',
    query: {
        id: '',
    }
});

useQuerySync(() => state.query)

const fhirApiProviderMetadata = computed<IEhrMetadata | undefined>(() => {
    if (!state.fhirApiProvider) {
        return undefined;
    }

    return EHRUtils.getMetadataForFhirProvider(state.fhirApiProvider.meldRxIdentifier);
});

const sandboxOptions = computed(() => {
    return fhirApiProviderMetadata.value?.sandboxServers.map(x => ({ value: x.fhirUrl, title: x.name })) ?? [];
});

const selectedSandboxOption = computed<ISandboxServer | undefined>(() => {
    return fhirApiProviderMetadata.value?.sandboxServers.find(x => x.fhirUrl === state.selectedSandboxFhirUrl);
});


// Load FHIR Provider...
async function loadFhirApiProvider(fhirApiProviderId: string) {
    state.query.id = fhirApiProviderId
    state.selectedSandboxFhirUrl = '';
    state.selectedSandboxStatus = undefined;
    state.isLoading = true;

    try {
        // Load data from DB...
        state.fhirApiProvider = await $api.fhirProviders.get(fhirApiProviderId);
    } catch (error) {
        handleApiError(error, 'Unable to load FHIR API provider');
    }

    state.isLoading = false;
}

// Occurs when the sandbox is updated...
async function onUpdateSelectedSandbox(fhirUrl: string) {
    state.selectedSandboxFhirUrl = fhirUrl;

    // Check health status of sandbox...
    state.isLoadingSandboxValidation = true;
    state.selectedSandboxStatus = undefined;
    try
    {
        const validationStatus = await $api.fhirProviders.validate(fhirUrl);
        state.selectedSandboxStatus = (validationStatus.isSuccess) ? 'healthy' : 'unhealthy';
    }
    catch
    {
        state.selectedSandboxStatus = 'unhealthy';
    }
    finally
    {
        state.isLoadingSandboxValidation = false;
    }
}

if(state.query.id){
    loadFhirApiProvider(state.query.id);
}
</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="state.isLoading" />

    <DsText size="2xl" weight="light">
      FHIR API Providers
    </DsText>
    <div class="pb-5" />

    <DsText size="sm" weight="light" class="block">
      Select a FHIR API Provider to view more information about it.
    </DsText>
    <div class="pb-5" />

    <FhirProviderSelect
      :fhir-api-provider-meld-rx-identifier="state.fhirApiProvider?.meldRxIdentifier ?? ''"
      :rules="[[(v) => !!v, 'FHIR API Provider is required']]"
      :include-ehr-quick-buttons="false"
      :include-link-to-fhir-provider="false"
      :current-linked-apps="[]"
      @update:fhir-provider-id="loadFhirApiProvider"
    />

    <DsDivider />

    <div v-if="state.fhirApiProvider">
      <DsText size="2xl" weight="light">
        {{ FhirApiProviderUtils.getDisplayString(state.fhirApiProvider) }}
      </DsText>
      <div class="pb-4" />

      <!-- Basic Info -->
      <div class="grid grid-cols-2 gap-4">
        <!-- Product Name -->
        <div>
          <DsText size="sm" class="block">
            Product Name
          </DsText>
          <DsText size="xs" weight="light">
            {{ state.fhirApiProvider.productName }}
          </DsText>
        </div>

        <!-- Organization Name -->
        <div>
          <DsText size="sm" class="block">
            Organization Name
          </DsText>
          <DsText size="xs" weight="light">
            {{ state.fhirApiProvider.organizationName }}
          </DsText>
        </div>

        <!-- Version -->
        <div>
          <DsText size="sm" class="block">
            Version
          </DsText>
          <DsText size="xs" weight="light">
            {{ state.fhirApiProvider.version }}
          </DsText>
        </div>

        <!-- CHPL ID -->
        <div>
          <DsText size="sm" class="block">
            CHPL ID
          </DsText>
          <DsText size="xs" weight="light">
            {{ state.fhirApiProvider.chplId }}
          </DsText>
        </div>

        <!-- API Documentation URL -->
        <div>
          <DsText size="sm" class="block">
            API Documentation URL
          </DsText>
          <DsText size="xs" weight="light">
            <DsLink :href="state.fhirApiProvider.apiDocumentationUrl" target="_blank">
              {{ state.fhirApiProvider.apiDocumentationUrl }}
            </DsLink>
          </DsText>
        </div>
      </div>
      <div class="pb-5" />

      <!-- Documentation -->
      <div v-if="fhirApiProviderMetadata">
        <DsDivider />

        <!-- Sandbox Info -->
        <div v-if="fhirApiProviderMetadata?.sandboxServers && fhirApiProviderMetadata?.sandboxServers.length > 0">
          <DsText size="2xl" weight="light">
            Sandbox Info
          </DsText>

          <DsText size="sm" weight="light" class="block">
            This FHIR API Provider has sandbox environments you can use for testing.<br>
            Select one of the following sandbox environments to find more information about it.
          </DsText>
          <div class="pb-5" />

          <!-- Sandbox Selector -->
          <DsSelect
            :model-value="state.selectedSandboxFhirUrl"
            label=""
            placeholder="Search a sandbox"
            :items="sandboxOptions"
            @update:model-value="onUpdateSelectedSandbox($event)"
          />
          <div class="pb-5" />

          <!-- Sandbox Details -->
          <div v-if="selectedSandboxOption">
            <div class="flex flex-row items-center">
              <!-- Sandbox Health Status (red light / green light) -->
              <div v-if="!!state.selectedSandboxStatus" class="pr-2">
                <div
                  v-if="state.selectedSandboxStatus === 'healthy'"
                  class="block bg-mint rounded-full m-0 h-[20px] w-[20px]"
                />

                <div
                  v-if="state.selectedSandboxStatus === 'unhealthy'"
                  class="block bg-fire rounded-full m-0 h-[20px] w-[20px]"
                />
              </div>
              <DsLoadingSpinner :loading="state.isLoadingSandboxValidation" />

              <!-- Sandbox Name -->
              <DsText size="lg" class="block pt-1">
                {{ selectedSandboxOption.name }}
              </DsText>
            </div>

            <!-- Sandbox Status (Description) -->
            <DsText v-if="state.selectedSandboxStatus === 'healthy'" size="sm" weight="light" class="block pt-1">
              Sandbox is currently healthy.
            </DsText>
            <DsText v-if="state.selectedSandboxStatus === 'unhealthy'" size="sm" weight="light" class="block pt-1">
              Sandbox is currently offline.
            </DsText>
            <div class="pb-5" />

            <!-- Sandbox URLs -->
            <div v-if="state.selectedSandboxStatus === 'healthy'">
              <!-- FHIR URL -->
              <div class="flex flex-row gap-2">
                <DsText size="xs" weight="light">
                  FHIR URL:
                </DsText>
                <DsTextWithCopyButton
                  size="xs"
                  :text="`${selectedSandboxOption.fhirUrl}`"
                  :text-to-copy="selectedSandboxOption.fhirUrl"
                  :show-toast-on-copy="true"
                  toast-message-on-copy="Copied FHIR URL to clipboard."
                />
              </div>

              <!-- Auth URL -->
              <div class="flex flex-row gap-2">
                <DsText size="xs" weight="light">
                  Auth URL:
                </DsText>
                <DsTextWithCopyButton
                  size="xs"
                  :text="`${selectedSandboxOption.authUrl}`"
                  :text-to-copy="selectedSandboxOption.authUrl"
                  :show-toast-on-copy="true"
                  toast-message-on-copy="Copied Auth URL to clipboard."
                />
              </div>

              <!-- Token URL -->
              <div class="flex flex-row gap-2">
                <DsText size="xs" weight="light">
                  Token URL:
                </DsText>
                <DsTextWithCopyButton
                  size="xs"
                  :text="`${selectedSandboxOption.tokenUrl}`"
                  :text-to-copy="selectedSandboxOption.tokenUrl"
                  :show-toast-on-copy="true"
                  toast-message-on-copy="Copied Token URL to clipboard."
                />
              </div>
              <div class="pb-4" />
            </div>

            <!-- Logins -->
            <div v-if="selectedSandboxOption && selectedSandboxOption.logins && selectedSandboxOption.logins.length > 0">
              <div class="pb-4" />
              <DsText size="md" weight="light">
                Logins
              </DsText>

              <DsTable
                v-if="selectedSandboxOption.logins"
                :headers="tableHeaders"
                :items="selectedSandboxOption.logins"
                :id-selector="item => item.username"
              >
                <template #default="{item}">
                  <div class="capitalize">
                    {{ item.loginType }}
                  </div>

                  <DsCopyButton :text-to-copy="item.username" :show-toast-on-copy="true">
                    <code v-code-highlight :class="'bash'">
                      {{ item.username }}
                    </code>
                  </DsCopyButton>

                  <DsCopyButton :text-to-copy="item.password" :show-toast-on-copy="true">
                    <code v-code-highlight :class="'bash'">
                      {{ item.password }}
                    </code>
                  </DsCopyButton>
                </template>
              </DsTable>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="pb-8" />
  </DsContainer>
</template>
