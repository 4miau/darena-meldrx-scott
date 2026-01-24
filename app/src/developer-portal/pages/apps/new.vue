<script setup lang="ts">
import type { LinkedAppForm } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CreateDeveloperApp'
import { DynamicAuthMethods } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/DynamicAuthMethods'
import { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod'
import { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType'
import { SecretType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/SecretType'
import {Colors} from "~/types/ui/colors";
import type { INewApp } from '~/types/ui/apps/NewApp'
import type { CreateCdsHooksAppCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CreateCdsHooksAppCommand'

useHead({ title: 'Register App | MeldRx' });

const { $api } = useNuxtApp();
const fhirApiProviders = useFhirApiProviders()
const { permissions } = useAuth()


const isLoading = ref<boolean>(false);
const pageState = ref<'new-app' | 'app-created'>('new-app');
const showCDSModal = ref<boolean>(false);
const newAppClientId = ref<string>('');
const newAppClientSecret = ref<string | undefined>(undefined);

const newApp = ref<INewApp>({
    appName: '',
    ehrLaunchUrl: '',
    appPublisherUrl: '',
    userType: SoFAppUserType.Patient,
    authenticationClientType: SoFAppTokenAuthMethod.Public,
    scopes: '',
    redirectUrls: [],
    linkedApps: [],
});

const confirmation = useConfirmation();

function createApp() {
    if (isLoading.value) { return; }

    isLoading.value = true
    return $api.apps
        .create({
            clientName: newApp.value.appName,
            publisherUrl: newApp.value.appPublisherUrl,
            soFAppUserType: newApp.value.userType,
            tokenEndpointAuthMethod: newApp.value.authenticationClientType === SoFAppTokenAuthMethod.Public
                ? DynamicAuthMethods.None
                : DynamicAuthMethods.ClientSecretPost,
            ehrLaunchUrl: newApp.value.ehrLaunchUrl,
            secretType: SecretType.ClientSecret,
            jwksUri: '',
            scope: newApp.value.scopes,
            redirectUris: newApp.value.redirectUrls,
            postLogoutRedirectUris: [],
            linkedApps: newApp.value.linkedApps.map((la) => {
                const form: LinkedAppForm = {
                    fhirApiProviderMeldRxIdentifier: la.fhirApiProviderMeldRxIdentifier!,
                    clientName: la.clientName,
                    clientId: la.clientId,
                    soFAppTokenAuthMethod: la.soFAppTokenAuthMethod!,
                    secretType: la.secretType!,
                    clientSecret: la.clientSecret,
                    scopes: la.scopes,
                    jwksAlg: la.jwksAlg,
                    jwksKid: la.jwksKid,
                    isSharedCredentialType: la.isSharedCredentialType
                }
                return form
            })
        })
        .then((data) => {
            // Go to success page...
            pageState.value = 'app-created';
            newAppClientId.value = data.clientId;
            newAppClientSecret.value = data.clientSecret;
        })
        .catch((error) => {
            handleApiError(error, 'Unable to register app')
        })
        .finally(() => {
            isLoading.value = false;
        })
}

async function createCdsHookApp (value: CreateCdsHooksAppCommand) {
    if (isLoading.value) {
        return
    }

    isLoading.value = true
    try {
        const formData = new FormData()
        formData.append('Name', value.name!)
        if (value.cdsHookServiceUrl) {
            formData.append('CdsHookServiceUrl', value.cdsHookServiceUrl)
        }

        if (value.cdsHook) {
            formData.append('CdsHook.Title', value.cdsHook.title)
            formData.append('CdsHook.Hook', value.cdsHook.hook)
            formData.append('CdsHook.Description', value.cdsHook.description)
            formData.append('CdsHook.UsageRequirements', value.cdsHook.usageRequirements)
        }

        value.cards?.forEach((card, index) => {
            formData.append(`Cards[${index}].Condition`, card.condition)
            formData.append(`Cards[${index}].Detail`, card.detail)
            formData.append(`Cards[${index}].Indicator`, card.indicator)
            formData.append(`Cards[${index}].Summary`, card.summary)
        })

        if(value.elmFiles){
            for (const file of value.elmFiles) {
                formData.append('Elms', file)
            }
        }

        if(value.cqlEditorArtifact){
            formData.append('CqlEditorArtifact', JSON.stringify(value.cqlEditorArtifact))
        }

        const data = await $api.apps.createCdsHookApp(formData)

        showCDSModal.value = true
        newAppClientId.value = data.clientId
    } catch (error) {
        handleApiError(error, 'Unable to register app')
    } finally {
        isLoading.value = false
    }
}


// "Cancel"
async function onCancel() {
    // Check if user really wants to cancel it first...
    const { isCancelled } = await confirmation(
        'Are you sure you want to stop creating this app? All of your progress will be permanently lost. This action cannot be undone.',
        'Discard App'
    );
    if (isCancelled) { return; }

    goToApps();
}

async function goToApps(appId?:string) {
    const url = `/apps`+ (appId ? `/${appId}/publish` : '')
    navigateTo(url);
}
</script>

<template>
  <DsModal :model-value="showCDSModal" @close="goToApps()">
    <div class="flex flex-col items-center p-5 space-y-5">
      <DsText size="lg">
        CDS App Registered
      </DsText>

      <DsText size="sm" weight="light">
        You can configure app details and source attributes on the publish page.
      </DsText>

      <div class="space-x-3">
        <DsButton :color="Colors.secondary" @click="goToApps()">
          Return to Apps
        </DsButton>
        <DsButton :color="Colors.primary" @click="goToApps(newAppClientId)">
          Continue to Publish
        </DsButton>
      </div>
    </div>
  </DsModal>
  <DsContainer>
    <DsLoadingOverlay :loading="isLoading" />

    <!-- New App -->
    <div v-if="pageState === 'new-app'">
      <DsText size="2xl" weight="light">
        Register App
      </DsText>
      <div class="py-2" />

      <CreateNewApp
        v-model="newApp"
        :fhir-api-providers="fhirApiProviders"
        :organization-id="permissions.developerPermissionsDto?.organizationId ?? ''"
        @create-app="createApp"
        @create-cds-hook-app="createCdsHookApp"
        @cancel="onCancel"
      />
    </div>

    <!-- App Created -->
    <div v-if="pageState === 'app-created'">
      <AppSuccessfullyCreated
        :client-id="newAppClientId"
        :client-secret="newAppClientSecret"
        @back-to-apps="goToApps()"
      />
    </div>
  </DsContainer>
</template>
