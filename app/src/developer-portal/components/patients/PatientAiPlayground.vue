<script setup lang="ts">
const systemMessage =
    'You are a healthcare AI assistant. You already have patient context. Since ' +
    'you are under a patient context, all questions are most likely regarding the ' +
    'patient. If a question is unclear, assume it is for the patient.';

const route = useRoute();
const { models, loading } = useAiModels();
</script>

<template>
    <DsLoadingSpinner :loading="loading" />
    <AiPlayground
        v-if="!loading"
        :system-message="systemMessage"
        :models="models"
        :disabled="models.length === 0"
        :patient-id="route.params.patientId?.toString()"
    />
</template>
