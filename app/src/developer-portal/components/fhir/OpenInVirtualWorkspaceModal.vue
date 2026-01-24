<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
import { VirtualWorkspaceSnapshot } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Fhir/VirtualWorkspaceOperationPayload';

const {$api} = useNuxtApp()
const { loadUserWorkspaces } = useAuth();

const props = defineProps<{
  modelValue: boolean;
  loading: boolean;
  workspaceSlug: string;
  patientId: string;
}>();

const emit = defineEmits<{
  'update:modelValue': [boolean],
  'update:loading': [boolean],
}>()

type Form = {
    name: string
}
function defaultForm() : Form {
    return {
        name: ''
    }
}

const formRef = ref<FormRef>();
const form = ref<Form>(defaultForm());

watch(
    () => props.modelValue,
    (v) => {
        if(v){
            form.value = defaultForm()
        }
    }
)

async function createVirtualWorkspace() {
    if(!formRef.value?.validate()){
        return;
    }

    emit('update:loading', true)

    try {
        const response = await $api.fhir.operations.virtualWorkspace(
            props.workspaceSlug,
            {
                snapshot: VirtualWorkspaceSnapshot.PatientPrefetch,
                name: form.value.name,
                patientId: props.patientId,
                hook: 'patient-view',
            }
        )

        navigateTo(`/workspaces/${response.workspaceId}/patients`)
        emit('update:modelValue', false)
        loadUserWorkspaces()
    } catch(e){
        handleApiError(e, 'failed to create a virtual workspace')
    }

    emit('update:loading', false)
}

</script>

<template>
  <DsModal :model-value="modelValue" @close="$emit('update:modelValue', false)">
    <DsModalProgressCard :total-steps="1" :current-step="1">
      <DsForm ref="formRef">
        <!-- Icon -->
        <div class="flex w-full py-8 bg-space justify-center items-center">
          <div class="flex w-full items-center justify-center">
            <div class="w-14 h-14 rounded-full flex justify-center items-center pt-2" style="background-color:#E79288">
              <MeldRxClipboard class="mb-2" />
            </div>
          </div>
          <div class="flex-1" />
        </div>

        <div class="flex w-full p-4">
          <div class="flex flex-col w-full">
            <!-- Heading -->
            <div class="w-full flex-col justify-center items-center gap-5 flex">
              <DsText size="2xl" weight="light">
                Open in Virtual Workspace
              </DsText>

              <DsText size="sm" weight="light" class="text-center">
                Create an isolated workspace to test system integration with selected patient data.
              </DsText>
            </div>

            <div class="grid grid-cols-1 gap-4">
              <DsTextInput
                v-model="form.name"
                label="Workspace Name"
                placeholder="My Virtual Workspace"
              />
            </div>

            <!-- Buttons -->
            <DsContainer>
              <div class="flex justify-center w-full gap-5">
                <DsButton :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" @click="$emit('update:modelValue', false)">
                  Cancel
                </DsButton>
                <DsButton :color="Colors.secondary" @click="createVirtualWorkspace">
                  Create
                </DsButton>
              </div>
            </DsContainer>
          </div>
        </div>
      </DsForm>
    </DsModalProgressCard>
  </DsModal>
</template>
