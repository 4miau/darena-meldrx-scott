<script setup lang="ts">
import type { AppSourceType } from '~/types/ui/apps/AppSourceType';

definePageMeta({
    layout: 'workspace',
    middleware: ['require-workspace'],
    allowLinked:true
})
useHead({ title: 'Extension Activation | MeldRx' })

const route = useRoute()
const confirmation = useConfirmation()
const currentTab: Ref<number> = ref(0);

const state = reactive<{
    appSourceType: AppSourceType;
    workspaceSlug: string;
    isLoading: boolean;

}>({
    isLoading: false,
    appSourceType: 'Published',
    workspaceSlug: route.params.workspaceSlug as string,
});


async function onCancelClicked() {
    const { isCancelled } = await confirmation(
        'Are you sure you want to cancel activating an Extension.',
        'Cancel Activating Extension'
    );
    if (isCancelled) { return; }
    navigateTo(`/workspaces/${route.params.workspaceSlug}/extensions`);
}
</script>

<template>
    <DsContainer>
        <DsLoadingOverlay :loading="state.isLoading" />

        <!-- Header Section -->
        <div>
            <DsText size="2xl" weight="light" class="block pb-5">
                Activate an Extension
            </DsText>

            <DsText size="sm" weight="light" class="block pb-5">
                Activate a published extension either from the marketplace
                or one made available by your organization.
            </DsText>
        </div>

        <div>
            <!-- Tab Headers -->
            <div class="justify-start items-start gap-1.5 inline-flex">
                <DsTabHeader title="Step 1" subtitle="Choose extention type" :enabled="currentTab >= 0" @click="() => {}" />
                <DsTabHeader title="Step 2" subtitle="Select an extension" :enabled="currentTab >= 1"   @click="() => {}  " />
            </div>

            <!-- Tab 1 Content (Step 1) -->
            <div v-if="currentTab === 0">
                <Step01ChooseExtensionType
                    v-model:app-source-type="state.appSourceType"
                    @next="currentTab = 1"
                    @cancel="onCancelClicked" />
            </div>

            <!-- Tab 2 Content (Step 2) -->
            <div v-if="currentTab === 1">
                <Step02ConfirmExtensionSettings
                    :workspace-slug="state.workspaceSlug"
                    :app-source-type="state.appSourceType"
                    @previous="() => currentTab = 0"
                    @cancel="onCancelClicked" />
            </div>
        </div>

    </DsContainer>
</template>
