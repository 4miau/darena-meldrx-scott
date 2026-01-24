<script setup lang="ts">
import { Colors } from '~/types/ui/colors';

definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    refreshWorkspace: true,
    allowLinked: true,
});
useHead({ title: 'AI Playground | MeldRx' });

const route = useRoute();
const workspaceSlug = ref<string>(route.params.workspaceSlug as string);
const { workspace } = useWorkspace();

const { models } = useAiModels();
</script>

<template>
    <DsContainer v-if="workspace">
        <div class="mb-5">
            <DsText size="2xl" weight="light" class="block">
                AI Playground
            </DsText>
            <div class="mt-5">
                <DsText size="md" weight="light">
                    Test configured AI models with this workspace.
                </DsText>
            </div>
        </div>
        <DsBanner v-if="models.length === 0" icon='heroicons:information-circle'>
          <DsLink underline="always" :href="`/workspaces/${workspaceSlug}/models`">
            <DsText size="sm" weight="light" :color="Colors.fire" class="underline text-center">
                You need to configure a model to use the playground.
            </DsText>
          </DsLink>
        </DsBanner>
        <DsDivider />

        <AiPlayground :models="models" :disabled="models.length === 0" />
    </DsContainer>
</template>
