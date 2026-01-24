<script setup lang="ts">
import { useRoute } from 'vue-router';
import type DynamicRegistrationDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/DynamicRegistrationDto';
import type LinkedAppDto from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/LinkedAppDto';
import type { INewLinkedApp, LinkedApp } from '~/types/ui/apps/NewLinkedApp';
import { Colors } from '~/types/ui/colors';
import { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod';
import { SecretType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/SecretType';
import { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';

useHead({ title: 'Manage App | MeldRx' });

const route = useRoute()
const { $api } = useNuxtApp()
const confirmation = useConfirmation();
const fhirApiProviders = useFhirApiProviders();

const { permissions } = useAuth();

const isLoadingApp = ref<boolean>(false);
const isLoadingLinkedApp = ref<boolean>(false);

const state = ref<{
  app?: DynamicRegistrationDto,
  linkedApps: LinkedAppDto[],
  newSecret?: string
}>({
    linkedApps: []
});

const showSecretModel = computed(() => !!state.value.newSecret)

// Load the app...
async function loadApp() {
    isLoadingApp.value = true;

    try {
        const originalApp = await $api.apps.get(route.params.appId as string);

        // If linked apps are loaded, set the user type for all linked apps...
        if (state.value.linkedApps.length > 0) {
            state.value.linkedApps.forEach((linkedApp) => {
                linkedApp.soFAppUserType = originalApp.soFAppUserType;
            });
        }

        if (originalApp) {
            state.value.app = originalApp;
        }
    } catch (error) {
        handleApiError(error, 'Unable to load app')
    }

    if (!state.value.app) {
        throw showError({ statusCode: 404 })
    }

    isLoadingApp.value = false;
}

// Load linked apps associated with this app...
async function loadLinkedApps() {
    isLoadingLinkedApp.value = true;

    try {
        const linkedApps = await $api.linkedApps.list(route.params.appId as string);

        // If app was loaded, set the user type for all linked apps...
        if (state.value.app) {
            linkedApps.forEach((linkedApp) => {
                linkedApp.soFAppUserType = state.value.app?.soFAppUserType;
            });
        }

        state.value.linkedApps = linkedApps;
    } catch (error) {
        handleApiError(error, 'Unable to load linked apps');
    }

    isLoadingLinkedApp.value = false;
}

loadApp();
loadLinkedApps();

async function onPublishApp(clientId: string) {
    navigateTo(`/apps/${clientId}/publish`);
}

async function onDeleteApp() {
    // Check if user really wants to delete it first...
    const { isCancelled } = await confirmation(
        'Are you sure you want to delete this app? All connections will be disabled and related workspaces will be orphaned. This action cannot be undone.',
        'Delete App'
    );
    if (isCancelled) {
        return;
    }

    isLoadingApp.value = true;

    try {
        await $api.apps.delete(route.params.appId as string);
    } catch (error) {
        handleApiError(error, 'Unable to delete app');
        isLoadingApp.value = false;
        return;
    }

    isLoadingApp.value = false;
    navigateTo('/apps');
}

// Occurs when "Save" button is pressed...
async function updateApp(app: DynamicRegistrationDto) {
    if (!app) {
        return;
    }

    isLoadingApp.value = true;

    try {
        await $api.apps.update({
            clientId: app.client_id,
            clientName: app.client_name,
            publisherUrl: app.client_uri ?? '',
            scope: app.scope.split(' '),
            redirectUrls: app.redirect_uris,
            jwksUri: app.jwks_uri,
            ehrLaunchUrl: app.ehrLaunchUrl ?? '',
            cdsHookServiceUrl: app.cdsHookServiceUrl ?? ''
        });
    } catch (error) {
        handleApiError(error, 'Unable to save app');
        isLoadingApp.value = false;
        return;
    }

    isLoadingApp.value = false;
    navigateTo('/apps');
}

// Occurs when a new linked app is added. Saves it immeidately...
async function onAddLinkedApp(linkedApp: INewLinkedApp | undefined) {
    if (!linkedApp) {
        return;
    }
    if (!state.value.app) {
        return;
    }

    isLoadingApp.value = true;

    try {
        await $api.linkedApps.create({
            meldRxClientId: state.value.app.client_id,
            soFAppTokenAuthMethod: linkedApp.soFAppTokenAuthMethod ?? SoFAppTokenAuthMethod.Public,
            fhirApiProviderMeldRxIdentifier: linkedApp.fhirApiProviderMeldRxIdentifier!,
            clientName: linkedApp.clientName ?? '',
            clientId: linkedApp.clientId ?? '',
            clientSecret: linkedApp.clientSecret ?? '',
            scopes: linkedApp.scopes ?? '',
            jwksAlg: linkedApp.jwksAlg ?? '',
            jwksKid: linkedApp.jwksKid ?? '',
            secretType: linkedApp.secretType ?? SecretType.ClientSecret,
            isSharedCredentialType: linkedApp.isSharedCredentialType ?? false
        });

        // Reload linked apps for this app...
        await loadLinkedApps();
    } catch (error) {
        handleApiError(error, 'Unable to add linked app');
    }

    isLoadingApp.value = false;
}

// Occurs when a linked app is edited. Saves it immeidately...
async function onEditLinkedApp (linkedApp: LinkedApp) {
    if (!state.value.app) {
        return;
    }

    isLoadingApp.value = true;

    try {
    // Update linked app
        const newLinkedApp = LinkedAppUtils.forLinkedApp(state.value.app.client_id, linkedApp);
        await $api.linkedApps.update(newLinkedApp);

        // Reload linked apps for this app...
        await loadLinkedApps();
    } catch (error) {
        handleApiError(error, 'Unable to edit linked app');
    }

    isLoadingApp.value = false;
}

// Occurs when a linked app is deleted. Saves it immeidately...
async function onDeleteLinkedApp (linkedAppId: string) {
    // Check if user really wants to delete it first...
    const { isCancelled } = await confirmation(
        'Are you sure you want to delete this linked app? This action cannot be undone.',
        'Delete Linked App'
    );
    if (isCancelled) {
        return;
    }

    isLoadingApp.value = true;

    try {
        await $api.linkedApps.delete(linkedAppId);
    } catch (error) {
        handleApiError(error, 'Unable to delete linked app');
    }

    // Reload linked apps for this app...
    await loadLinkedApps();
    isLoadingApp.value = false;
}

async function createSecret () {
    isLoadingApp.value = true;

    try {
        state.value.newSecret = await $api.apps.createSecret(route.params.appId as string);
    } catch (error) {
        handleApiError(error, 'Unable to delete linked app');
    }

    await loadApp();
}

async function deleteSecret (secretId: number) {
    const { isCancelled } = await confirmation(
        'Are you sure you want to delete this secret? This action cannot be undone.',
        'Delete Secret'
    );
    if (isCancelled) {
        return;
    }
    isLoadingApp.value = true;

    try {
        await $api.apps.deleteSecret(route.params.appId as string, secretId);
    } catch (error) {
        handleApiError(error, 'Unable to delete secret');
    }

    await loadApp();
}

</script>

<template>
  <DsContainer>
    <DsLoadingOverlay :loading="isLoadingApp || isLoadingLinkedApp" />

    <DsModal :model-value="showSecretModel" @close="state.newSecret = ''">
      <DsContainer>
        <div class="grid gap-4 p-5">
          <DsText size="xl">
            New Secret
          </DsText>
          <DsText size="xs" weight="light">
            Please save and store your secret securely, you won't be able to see it againt after closing this window.
          </DsText>
          <DsTextInputWithCopyButton label="New Client Secret" disabled :model-value="state.newSecret!" class="w-full" />
          <DsButton :color="Colors.primary" @click="state.newSecret = ''">
            Close
          </DsButton>
        </div>
      </DsContainer>
    </DsModal>

    <DsText size="2xl" weight="light">
      Manage App
    </DsText>
    <div class="pb-5" />

    <DsText size="sm" weight="light">
      View and modify details about this application
    </DsText>
    <div class="pb-5" />

    <!--App Actions-->
    <div class="grid grid-cols-12 gap-8">
      <!-- App Details -->
      <div class="col-span-4">
        <DsLabeledText
          label="App Actions"
          text="Administrative actions for this app"
        />
      </div>

      <!-- Actions -->
      <div class="col-span-8">
        <DsText size="sm">
          Actions
        </DsText>
        <div class="pb-2" />
        <div class="flex">
        <DsButton
            v-if="!(state.app?.soFAppUserType === 'System')"
            :id="`publish-${state.app?.client_id.replaceAll(' ', '-').toLowerCase()}-button`"
            :color="Colors.secondary"
            :text-color='Colors.secondary'
            variant="outline"
            style="margin-right: 10px;"
            @click="onPublishApp(state.app?.client_id ?? '')"
        >
          <DsIcon name='heroicons:cloud-arrow-up' size='sm' />
          Publish
        </DsButton>
        <DsButton :color="Colors.fire" :text-color='Colors.fire' variant="outline" @click="onDeleteApp">
          <DsIcon name='heroicons:x-mark' size='sm' />
          Delete App
        </DsButton>
        </div>
        <div class="py-1" />
      </div>
    </div>
    <DsDivider />

    <template v-if="state.app">

      <UpdateCdsHookApp
        v-if="state.app.soFAppUserType === SoFAppUserType.CdsHooks"
        :app="state.app"
        @cancel="navigateTo('/apps')"
      />

      <AppManage
        v-else
        :app="state.app"
        :linked-apps="state.linkedApps"
        :fhir-api-providers="fhirApiProviders"
        :organization-id="permissions.developerPermissionsDto?.organizationId ?? ''"
        @add-linked-app="onAddLinkedApp"
        @delete-linked-app="onDeleteLinkedApp"
        @edit-linked-app="onEditLinkedApp"
        @create-secret="createSecret"
        @delete-secret="deleteSecret"
        @save="updateApp"
        @cancel="navigateTo('/apps')"
      />

    </template>
  </DsContainer>
</template>
