<script setup lang="ts">
import type { IDsSelectItem } from '~/types/ui/DsSelect';
import type {
    ChatPayload,
    AiInferenceResponse,
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/AiInference';
import { Colors } from '~/types/ui/colors';
import type { AiModelDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/AiModelDto';
import { AiPromptReader } from '~/utils/AiPromptReader';
import { AiStreamResponseChunkType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/AiStreamResponseChunkType';

const { $api } = useNuxtApp();
const route = useRoute();
const workspaceSlug = ref<string>(route.params.workspaceSlug as string);

const props = defineProps<{
    disabled?: boolean;
    models?: AiModelDto[];
    systemMessage?: string;
    patientId?: string;
}>();

const modelItems = computed<IDsSelectItem<string>[]>(
    () => props.models?.map((x) => ({ value: x.name, title: x.name })) || []
);

const allowedFileTypes = [
    { fileExtension: ['.jpg', '.png', '.jpeg'], fileDescription: 'Images' },
];

const state = reactive<{
    loading: boolean;
    files?: FileList;
    messageForm: ChatPayload;
    response?: AiInferenceResponse;
    responseContent: string;
}>({
    loading: false,
    messageForm: {
        model: '',
        systemMessage:
            props.systemMessage || 'You are a healthcare AI assistant',
        chatMessage: '',
    },
    responseContent: '',
});

const formRef = ref<FormRef>();

async function onSubmit() {
    if (!formRef.value) return;

    const isFormValid = formRef.value.validate();
    if (!isFormValid) return;

    state.loading = true;

    try {
        const selectedModel = props.models?.find(
            (m) => m.name === state.messageForm.model
        );

        const payload: ChatPayload = {
            ...state.messageForm,
            patientId: props.patientId,
        };

        if (selectedModel?.isMultiModal && state.files?.[0]) {
            const file = state.files?.[0];

            if (file) {
                const { base64 } = await fileToRawBase64(file);
                payload.base64BinaryData = base64;
                payload.base64BinaryDataName = file.name;
            }
        }

        state.responseContent = '';
        state.response = undefined;

        const usage: AiInferenceResponse = {
            promptToken: 0,
            completionToken: 0,
            totalTokens: 0,
            toolCalls: [],
        };

        const response = await $api.ai.inference(workspaceSlug.value, payload);
        const reader = new AiPromptReader(response);

        for await (const chunk of reader.read()) {
            switch (chunk.type) {
                case AiStreamResponseChunkType.Text:
                    state.responseContent = state.responseContent + chunk.text;
                    break;
                case AiStreamResponseChunkType.Usage:
                    usage.promptToken += chunk.inputCount || 0;
                    usage.completionToken += chunk.outputCount || 0;
                    usage.totalTokens += chunk.totalCount || 0;

                    break;
                case AiStreamResponseChunkType.Tools:
                    usage.toolCalls.push(chunk.name);
                    break;
            }
        }

        state.response = usage;
    } catch (e) {
        handleApiError(e, 'Failed sending message');
    } finally {
        state.loading = false;
    }
}

function fileToRawBase64(
    file: File
): Promise<{ base64: string; type: string }> {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => {
            const result = reader.result as string;
            const matches = result.match(/^data:(.*);base64,(.*)$/);
            if (!matches) return reject(new Error('Invalid base64 format'));

            const [, type, base64] = matches;
            resolve({ base64, type });
        };
        reader.onerror = reject;
    });
}
</script>

<template>
    <div class="grid grid-cols-12 gap-14">
        <div class="col-span-4">
            <DsLabeledText
                label="AI Playground"
                text="Use this to test sending messages to  your configured models."
            />
        </div>

        <DsForm ref="formRef">
            <div v-if="props.models" class="col-span-8 space-y-5">
                <div class="space-y-2">
                    <DsSelect
                        v-model="state.messageForm.model"
                        label="Available Models"
                        :items="modelItems"
                        placeholder="Select the model to use"
                    />
                    <div>
                        <DsBadge
                            v-if="
                                props.models.find(
                                    (x) => x.name === state.messageForm.model
                                )?.isMultiModal
                            "
                            size="sm"
                            rounded
                            :text-color="Colors.white"
                            :color="Colors.secondary"
                        >
                            MultiModal
                        </DsBadge>
                    </div>
                </div>

                <DsTextArea
                    v-if="!props.systemMessage"
                    v-model="state.messageForm.systemMessage"
                    label="System Message"
                    required
                    :disabled="disabled"
                    :rules="[[(v) => !!v, 'System Message cant be empty']]"
                    placeholder="You are a healthcare AI assistant, ask for more information where it is needed to answer."
                />
                <DsTextArea
                    v-model="state.messageForm.chatMessage"
                    label="Message"
                    required
                    :disabled="disabled"
                    :rules="[[(v) => !!v, 'Message cant be empty']]"
                    placeholder="What is bilirubin and why is it important?"
                />

                <DsFileSelector
                    v-if="
                        props.models.find(
                            (x) => x.name === state.messageForm.model
                        )?.isMultiModal
                    "
                    v-model="state.files"
                    placeholder-text="Upload a file up to 20MB"
                    hide-clear-button
                    label="File"
                    :allowed-extensions="
                        allowedFileTypes.flatMap((x) => x.fileExtension)
                    "
                    :multiple="false"
                />

                <div class="flex space-x-3 items-center">
                    <DsButton
                        :disabled="
                            disabled ||
                            state.loading ||
                            !state.messageForm.model ||
                            !state.messageForm.systemMessage ||
                            !state.messageForm.chatMessage
                        "
                        @click="onSubmit"
                    >
                        Submit
                    </DsButton>
                    <DsLoadingSpinner :loading="state.loading" />
                </div>

                <DsTextArea
                    :model-value="state.responseContent"
                    disabled
                    label="Response"
                    :min-rows="3"
                />
                <div>
                    <div v-if="state.response?.promptToken">
                        <DsText size="sm">
                            Prompt Tokens:
                            {{ state.response.promptToken }}</DsText
                        >
                    </div>
                    <div v-if="state.response?.completionToken">
                        <DsText size="sm">
                            Completion Tokens:
                            {{ state.response.completionToken }}</DsText
                        >
                    </div>
                    <div v-if="state.response?.totalTokens">
                        <DsText size="sm">
                            Total Tokens:
                            {{ state.response.totalTokens }}</DsText
                        >
                    </div>
                    <div v-if="state.response?.toolCalls.length">
                        <DsText size="sm">
                            Tools Called:
                            {{ state.response.toolCalls.join(', ') }}</DsText
                        >
                    </div>
                </div>
            </div>
        </DsForm>
    </div>
</template>
