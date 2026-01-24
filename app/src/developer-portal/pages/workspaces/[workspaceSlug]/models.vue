<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import type {
    AddWorkspaceModel,
    UpdateWorkspaceModel,
    WorkspaceModelDto,
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/AiModelDto';

definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    refreshWorkspace: true,
    allowLinked: true,
});
useHead({ title: 'Configure AI Models | MeldRx' });

const { $api } = useNuxtApp();
const route = useRoute();
const workspaceSlug = ref<string>(route.params.workspaceSlug as string);
const { workspace, refreshWorkspace } = useWorkspace();
const azureModelHeaders = ['Model Name', 'Model Url', 'Actions'];
const azureModelActions = [
    { value: 'update', title: 'Update Model' },
    { value: 'delete', title: 'Delete Model' },
];

const state = reactive<{
    loading: boolean;
    loadingToken: boolean;
    token?: string;
    isGithubModelsActive: boolean;
    models?: WorkspaceModelDto[];
    selectedModel?: WorkspaceModelDto;
    showWorkspaceModelModal: boolean;
}>({
    loading: false,
    isGithubModelsActive: workspace.value?.isGithubModelsActive ?? false,
    loadingToken: false,
    token: undefined,
    showWorkspaceModelModal: false,
});

const formRef = ref<FormRef>();

async function onSaveWorkspace() {
    state.loading = true;

    try {
        await $api.ai.updateGithubSettings(workspaceSlug.value, {
            isActive: state.isGithubModelsActive,
            githubToken: state.isGithubModelsActive ? state.token : undefined,
        });

        if (!state.isGithubModelsActive) {
            state.token = undefined;
        }

        await refreshWorkspace();

        // Show success notification
        successNotification('Models successfully updated.')
    } catch (error) {
        handleApiError(error, 'Unable to save workspace');
    } finally {
        state.loading = false;
    }
}

async function getToken() {
    state.loadingToken = true;

    try {
        const response = await $api.ai.getGitHubToken(workspaceSlug.value);
        state.token = response.token;
    } catch (e) {
        handleApiError(e);
    } finally {
        state.loadingToken = false;
    }
}

async function loadModels() {
    if (workspace.value?.isGithubModelsActive) {
        return;
    }
    state.loading = true;
    try {
        state.models = (await $api.ai.getWorkspaceModels(
            workspaceSlug.value
        )) as WorkspaceModelDto[];
    } catch (e) {
        state.models = [];
        handleApiError(e, "Can't load models");
    } finally {
        state.loading = false;
    }
}

function selectWorkspaceModel(modelId?: string) {
    if (!modelId) {
        state.selectedModel = undefined;
        state.showWorkspaceModelModal = true;
        return;
    }

    state.selectedModel = state.models?.find((x) => x.id === modelId);
    state.showWorkspaceModelModal = true;
}

async function addWorkspaceModel(model: AddWorkspaceModel) {
    state.loading = true;

    try {
        await $api.ai.addWorkspaceModel(workspaceSlug.value, model);
        state.showWorkspaceModelModal = false;
    } catch (error) {
        handleApiError(error);
    } finally {
        state.loading = false;
    }
    await loadModels();
}

async function updateWorkspaceModel(model: UpdateWorkspaceModel) {
    state.loading = true;

    try {
        await $api.ai.updateWorkspaceModel(workspaceSlug.value, model);
        state.showWorkspaceModelModal = false;
    } catch (error) {
        handleApiError(error);
    } finally {
        state.loading = false;
    }
    await loadModels();
}

async function deleteWorkspaceModel(modelId: string) {
    state.loading = true;

    try {
        await $api.ai.deleteWorkspaceModel(workspaceSlug.value, modelId);
        state.showWorkspaceModelModal = false;
    } catch (error) {
        handleApiError(error);
    } finally {
        state.loading = false;
    }
    await loadModels();
}

const onActionSelected = (action: string, azureModel: WorkspaceModelDto) => {
    if (action === 'update') {
        selectWorkspaceModel(azureModel.id);
    } else if (action === 'delete') {
        deleteWorkspaceModel(azureModel.id);
    }
};

onMounted(() => {
    getToken();
});
loadModels();
</script>

<template>
    <DsContainer v-if="workspace">
        <div class="mb-5">
            <DsText size="2xl" weight="light" class="block">
                Configure AI Models
            </DsText>
        </div>

        <DsDivider />

        <div class="grid grid-cols-12 gap-14">
            <div class="col-span-4">
                <DsLabeledText
                    label="Azure Models"
                    text="Add Azure models from the list available in the Azure Foundry."
                />
            </div>

            <div class="col-span-8 space-y-2">
                <div class="flex">
                    <DsButton
                        :color="Colors.primary"
                        variant="filled"
                        :disabled="workspace.isGithubModelsActive"
                        @click="selectWorkspaceModel()"
                    >
                        <DsIcon name="i-heroicons-plus" />
                        Add Azure Model
                    </DsButton>

                    <DsBanner v-if='workspace.isGithubModelsActive' icon='heroicons:information-circle' :color='Colors.fire' class='pl-5'>
                      To configure Azure models you have to disable the Github models.
                    </DsBanner>
                </div>

                <DsTable
                    v-if="
                        state.models &&
                        state.models.length > 0 &&
                        !workspace.isGithubModelsActive
                    "
                    :headers="azureModelHeaders"
                    :items="state.models"
                    :id-selector="(item) => item.id"
                >
                    <template #default="{ item }">
                        <div class="flex flex-col">
                            <DsText size="md" weight="light">
                                {{ item.modelName }}
                            </DsText>
                            <DsText size="sm" weight="light">
                                {{ item.modelUrl }}
                            </DsText>
                            <div class="space-x-2 flex flex-row">
                                <DsDropdown
                                    :id="
                                        item.modelName
                                            .replaceAll(' ', '-')
                                            .toLowerCase() + '-actions'
                                    "
                                    :options="azureModelActions"
                                    label="Actions"
                                    @select="
                                        (v) => {
                                            onActionSelected(v, item);
                                        }
                                    "
                                />
                            </div>
                        </div>
                    </template>
                </DsTable>
            </div>
        </div>

        <WorkspaceModelModal
            :show="state.showWorkspaceModelModal"
            :workspace-model="state.selectedModel"
            @add-workspace-model="(v:AddWorkspaceModel) => addWorkspaceModel(v)"
            @update-workspace-model="(v:UpdateWorkspaceModel) => updateWorkspaceModel(v)"
            @close="state.showWorkspaceModelModal = false"
        />

        <DsDivider />

        <DsForm ref="formRef">
            <div class="grid grid-cols-12 gap-14">
                <div class="col-span-4">
                    <DsLabeledText
                        label="Github Models"
                        text="Create a GitHub personal access token. The token needs to have models:read permissions.
              You can configure Github models for development and testing. Do not use with real data."
                    />
                </div>

                <div class="col-span-8 space-y-5">
                    <DsLabeledInput label="Github Models">
                        <DsButtonGroup v-model="state.isGithubModelsActive">
                            <DsButton :value="true">
                              Enabled
                            </DsButton>
                            <DsButton :value="false">
                              Disabled
                            </DsButton>
                        </DsButtonGroup>
                    </DsLabeledInput>
                    <DsTextInput v-model="state.token" label="Github Token" />
                </div>
            </div>
        </DsForm>

        <DsDivider />

        <DsButton
            size="md"
            :disabled="state.loading || state.loadingToken"
            @click="onSaveWorkspace"
        >
            Save
        </DsButton>
    </DsContainer>
</template>
