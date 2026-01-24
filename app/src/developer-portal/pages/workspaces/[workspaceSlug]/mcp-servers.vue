<script setup lang="ts">
import type McpServerForm from '~/components/Ai/McpServerForm.vue';
import {
    McpServerAuthType,
    McpServerTransportType,
    type McpServerDto,
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/McpServerDto';
import type { TestToolDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/TestToolDto';
import { Colors } from '~/types/ui/colors';

type McpServerFormInstance = InstanceType<typeof McpServerForm>;

definePageMeta({ layout: 'workspace', middleware: ['require-workspace'] });
useHead({ title: 'MCP Servers | MeldRx' });

let randomId = 0;

const { $api } = useNuxtApp();
const route = useRoute();
const { workspace, refreshWorkspace } = useWorkspace();
const workspaceSlug = ref<string>(route.params.workspaceSlug as string);

const loading = ref(false);
const loadingMcpServers = ref(true);
const loadingTest = ref(false);
const showTestSuccessModal = ref(false);

const mcpServers = ref<McpServerDto[]>([]);
const mcpServersToDelete: McpServerDto[] = [];
const testTools = ref<TestToolDto[]>([]);

const mcpServerFormRefs = ref<(McpServerFormInstance | null)[]>([]);

const allFormsValid = () => {
    return mcpServerFormRefs.value
        .filter((x) => x?.formRef)
        .every((x) => !!x?.formRef?.validate());
};

const onSave = async () => {
    if (!allFormsValid()) {
        return;
    }

    let hasErrors = false;
    loading.value = true;

    try {
        await $api.ai.toggleDefaultMcpTools(
            workspaceSlug.value,
            workspace.value?.enableDefaultMcpTools || false
        );

        await refreshWorkspace();
    } catch (error) {
        handleApiError(error);
        hasErrors = true;
    }

    for (let i = 0; i < mcpServers.value.length; i++) {
        try {
            const mcpServer = mcpServers.value[i];
            const updatedServer = mcpServer.id.startsWith('new-')
                ? await $api.ai.createMcpServer(workspaceSlug.value, mcpServer)
                : await $api.ai.updateMcpServer(workspaceSlug.value, mcpServer);

            mcpServers.value[i] = updatedServer;
        } catch (error) {
            handleApiError(error);
            hasErrors = true;
        }
    }

    for (let i = 0; i < mcpServersToDelete.length; i++) {
        try {
            await $api.ai.deleteMcpServer(
                workspaceSlug.value,
                mcpServersToDelete[i].id
            );

            mcpServersToDelete.splice(i--, 1);
        } catch (error) {
            handleApiError(error);
            hasErrors = true;
        }
    }

    // Show success notification
    if (!hasErrors) {
        successNotification('MCP servers successfully updated.');
    }

    loading.value = false;
};

const addMcpServer = () => {
    if (!allFormsValid()) {
        return;
    }

    mcpServers.value.push({
        id: `new-${randomId++}`,
        endpoint: '',
        transportType: McpServerTransportType.Sse,
        authType: McpServerAuthType.ApiKey,
    });
};

const removeMcpServer = (mcpServer: McpServerDto) => {
    mcpServers.value.splice(mcpServers.value.indexOf(mcpServer), 1);

    if (!mcpServer.id.startsWith('new-')) {
        mcpServersToDelete.push(mcpServer);
    }
};

const testMcpServer = async (mcpServer: McpServerDto) => {
    loadingTest.value = true;

    try {
        const response = await $api.ai.testMcpServer(
            workspaceSlug.value,
            mcpServer
        );

        testTools.value = response;
        showTestSuccessModal.value = true;
    } catch (error) {
        handleApiError(error);
    }

    loadingTest.value = false;
};

onMounted(async () => {
    try {
        const response = await $api.ai.listMcpServers(workspaceSlug.value);

        mcpServers.value = response;
        loadingMcpServers.value = false;
    } catch (e) {
        handleApiError(e, 'Failed to load MCP servers');
    }
});
</script>

<template>
    <DsContainer v-if="workspace">
        <div class="mb-5">
            <DsText size="2xl" weight="light" class="block">
                Configure MCP Servers
            </DsText>
        </div>

        <DsDivider />

        <div class="grid grid-cols-12 gap-14">
            <div class="col-span-4">
                <DsLabeledText
                    label="MeldRx Default MCP Server"
                    text="Toggle to enable or disable default MCP tools available within MeldRx"
                />
            </div>

            <div class="col-span-8 flex items-center">
                <DsToggle v-model="workspace.enableDefaultMcpTools" />
            </div>
        </div>

        <DsDivider />

        <div class="grid grid-cols-12 gap-14">
            <div class="col-span-4">
                <DsLabeledText
                    label="External MCP Servers"
                    text="Configure one or more external MCP servers"
                />
            </div>

            <div class="col-span-8 space-y-5">
                <template
                    v-for="(mcpServer, index) in mcpServers"
                    :key="mcpServer.id"
                >
                    <McpServerForm
                        :ref="x => mcpServerFormRefs[index] = x as McpServerFormInstance"
                        v-model="mcpServers[index]"
                        :disabled="loading"
                        :loading-test="loadingTest"
                        @remove="removeMcpServer"
                        @test="testMcpServer"
                    />
                    <DsDivider v-if="mcpServers.length > 1" />
                </template>
                <DsLoadingSpinner :loading="loadingMcpServers" />
                <DsButton
                    v-if="!loadingMcpServers"
                    :color="Colors.primary"
                    :disabled="loading"
                    variant="filled"
                    @click="addMcpServer"
                >
                    <DsIcon name="heroicons:plus" />
                    Add External MCP Server
                </DsButton>
            </div>
        </div>

        <DsDivider />

        <DsButton size="md" :disabled="loading" @click="onSave">
            Save
        </DsButton>
        <DsViewer
            v-if="showTestSuccessModal"
            @close="showTestSuccessModal = false"
        >
            <div class="min-w-[800px] max-w-[1200px] p-4">
                <div class="flex items-center">
                    <DsText size="lg">MCP Server Tools</DsText>
                    <DsIcon
                        class="ml-4 bg-dsprimary"
                        name="heroicons:check-circle-solid"
                        size="lg"
                    />
                </div>
                <DsDivider />
                <template v-for="(tool, index) in testTools" :key="tool.name">
                    <div>
                        <DsText size="sm">{{ tool.name }}</DsText>
                    </div>
                    <div v-if="tool.description">
                        <DsText size="xs" weight="light">{{
                            tool.description
                        }}</DsText>
                    </div>
                    <DsDivider v-if="index !== testTools.length - 1" />
                </template>
            </div>
        </DsViewer>
    </DsContainer>
</template>
