import type { AiModelDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/AiModelDto';

export function useAiModels() {
    const { $api } = useNuxtApp();
    const route = useRoute();
    const workspaceSlug = ref<string>(route.params.workspaceSlug as string);

    const models = ref<AiModelDto[]>([]);
    const loading = ref(false);

    onMounted(async () => {
        loading.value = true;

        try {
            models.value = (await $api.ai.getModels(
                workspaceSlug.value
            )) as AiModelDto[];
        } catch (e) {
            handleApiError(e, "Can't load models");
        }

        loading.value = false;
    });

    return { models, loading };
}
