<script setup lang="ts">
import {
    McpServerAuthType,
    McpServerTransportType,
    type McpServerDto,
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/McpServerDto';
import { Colors } from '~/types/ui/colors';
import type { IDsSelectItem } from '~/types/ui/DsSelect';

const model = defineModel<McpServerDto>({ required: true });
const props = defineProps<{ disabled: boolean; loadingTest: boolean }>();
const emit = defineEmits<{
    (e: 'remove' | 'test', mcpServer: McpServerDto): void;
}>();

const formRef = ref<FormRef>();

defineExpose({ formRef });

const transportTypes: IDsSelectItem<McpServerTransportType>[] = [
    { title: 'SSE', value: McpServerTransportType.Sse },
    { title: 'Streamable HTTP', value: McpServerTransportType.StreamableHttp },
];

const authTypes: IDsSelectItem<McpServerAuthType>[] = [
    { title: 'Open', value: McpServerAuthType.Open },
    { title: 'API Key', value: McpServerAuthType.ApiKey },
    { title: 'Basic Authentication', value: McpServerAuthType.BasicAuth },
];

const authTypeUpdated = (authType: McpServerAuthType) => {
    switch (authType) {
        case McpServerAuthType.Open:
            model.value.apiKeyHeaderName = undefined;
            model.value.apiKeyHeaderValue = undefined;
            model.value.basicAuthUsername = undefined;
            model.value.basicAuthPassword = undefined;
            break;
        case McpServerAuthType.ApiKey:
            model.value.basicAuthUsername = undefined;
            model.value.basicAuthPassword = undefined;
            break;
        case McpServerAuthType.BasicAuth:
            model.value.apiKeyHeaderName = undefined;
            model.value.apiKeyHeaderValue = undefined;
            break;
    }
};

const testClicked = (mcpServer: McpServerDto) => {
    if (!formRef.value?.validate()) {
        return;
    }

    emit('test', mcpServer);
};
</script>

<template>
    <DsForm ref="formRef">
        <div class="flex-col space-y-4">
            <DsTextInput
                v-model="model.endpoint"
                :disabled="props.disabled"
                required
                label="Endpoint"
                :rules="[[(x) => !!x, 'Endpoint is required']]"
            />
            <DsSelect
                v-model="model.transportType"
                label="Transport Type"
                :items="transportTypes"
                :disabled="props.disabled"
            />
            <DsSelect
                v-model="model.authType"
                label="Authentication Type"
                :items="authTypes"
                :disabled="props.disabled"
                @update:model-value="authTypeUpdated"
            />
            <div
                v-if="model.authType === McpServerAuthType.ApiKey"
                class="space-y-4"
            >
                <DsTextInput
                    v-model="model.apiKeyHeaderName"
                    label="API Key Header Name"
                    required
                    :disabled="props.disabled"
                    :rules="[[(x) => !!x, 'API key header name is required']]"
                />
                <DsTextInput
                    v-model="model.apiKeyHeaderValue"
                    label="API Key Header Value"
                    required
                    :disabled="props.disabled"
                    :rules="[[(x) => !!x, 'API key header value is required']]"
                />
            </div>
            <div
                v-if="model.authType === McpServerAuthType.BasicAuth"
                class="space-y-4"
            >
                <DsTextInput
                    v-model="model.basicAuthUsername"
                    label="Username"
                    required
                    :disabled="props.disabled"
                    :rules="[[(x) => !!x, 'Username is required']]"
                />
                <DsTextInput
                    v-model="model.basicAuthPassword"
                    label="Password"
                    required
                    :disabled="props.disabled"
                    :rules="[[(x) => !!x, 'Password is required']]"
                />
            </div>
            <div class="flex justify-end space-x-2">
                <DsButton
                    :color="Colors.secondary"
                    :text-color="Colors.secondary"
                    :disabled="props.disabled || props.loadingTest"
                    variant="outline"
                    @click="testClicked(model)"
                >
                    <DsIcon
                        name="i-heroicons-arrow-right-circle-solid"
                        size="md"
                    />
                    Test
                </DsButton>
                <DsButton
                    :color="Colors.fire"
                    :text-color="Colors.fire"
                    :disabled="props.disabled || props.loadingTest"
                    variant="outline"
                    @click="emit('remove', model)"
                >
                    <DsIcon name="i-heroicons-x-mark" />
                    Remove
                </DsButton>
            </div>
        </div>
    </DsForm>
</template>
