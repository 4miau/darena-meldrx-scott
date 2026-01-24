<script setup lang="ts">
import {Colors} from '~/types/ui/colors';
import type {
    AddWorkspaceModel,
    WorkspaceModelDto,
    UpdateWorkspaceModel
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Ai/AiModelDto";

const formRef = ref<FormRef>();

const props = defineProps<{
  show: boolean,
  workspaceModel?: WorkspaceModelDto;
}>();

const emit = defineEmits<{
  'addWorkspaceModel': [model: AddWorkspaceModel];
  'updateWorkspaceModel': [model: UpdateWorkspaceModel];
  'close': [];
}>();

const state = reactive<{
  form: AddWorkspaceModel | UpdateWorkspaceModel;
  loading: boolean;
}>({
    loading: false,
    form: createDefaultWorkspaceModel(),
});


function createDefaultWorkspaceModel(): AddWorkspaceModel | UpdateWorkspaceModel{
  
    if(props.workspaceModel) {
        return {...props.workspaceModel}
    }
    
    return {
        modelUrl: "",
        modelName: "",
        apiKey:"",
        modelHost: "Azure",
        disableToolCalling: false,
        isMultiModal: false,
        publisherName: ""
    }
}

function onSubmit() {
    if (!formRef.value) return;
    const isValid = formRef.value.validate();
    if (!isValid) return;

    if (props.workspaceModel) {
        emit('updateWorkspaceModel', state.form as UpdateWorkspaceModel);
    } else {
        emit('addWorkspaceModel', state.form as AddWorkspaceModel);
    }
}


watch(() => props.show, (value) => {
    if(value){
        state.form = createDefaultWorkspaceModel()
    }
});
</script>

<template>
  <DsModal :model-value="props.show" @close="$emit('close')">
    <DsModalProgressCard :total-steps="1" :current-step="1">
    <div class="flex w-full py-8 bg-space justify-center items-center">
      <div class="flex w-full items-center justify-center">
        <div class="w-12 h-12 bg-silver rounded-full flex justify-center items-center">
          <MeldRxApp/>
        </div>
      </div>
      <div class="flex-1"/>
    </div>
    <div class="flex flex-col w-full py-5 px-10">
      
      <div class="w-full flex-col justify-center items-center gap-5 flex">
        <DsText size="2xl" weight="light">
          Azure Model Details
        </DsText>

        <DsText size="sm" weight="light">
          Configure the azure model to add to the workspace.
        </DsText>
      </div>
      <!-- Azure model Details -->
      <DsForm ref="formRef">
        <div  class="flex flex-col space-y-2">
          <!-- Name -->
          <DsTextInput
              v-model="state.form.modelName"
              :disabled="!!props.workspaceModel?.id"
              :required="true"
              :rules="[[(v) => !!v, 'Name cant be empty']]"
              type="text"
              label="Model Name"
          />
          
          <!-- Url -->
          <DsTextInput
              v-model="state.form.modelUrl"
              :required="true"
              :rules="ValidationRules.url"
              type="text"
              label="Model URL"
          />
          
          <!-- API Key -->
          <DsTextInput
              v-model="state.form.apiKey"
              :required="true"
              :rules="[[(v) => !!v, 'Name cant be empty']]"
              type="text"
              label="Model API Key"
          />

          <DsTextInput
              v-model="state.form.publisherName"
              :required="true"
              :rules="[[(v) => !!v, 'Name cant be empty']]"
              type="text"
              label="Model Publisher"
          />

          <DsText size="md" weight="light">
            Multi-Modal Model
            <DsToggle
                id="toggle-is-multimodal-button"
                v-model="state.form.isMultiModal"
            />
          </DsText>

          <DsText size="md" weight="light">
            Disable Tool Calling
            <DsToggle
                id="toggle-disable-tool-calling-button"
                v-model="state.form.disableToolCalling"
            />
          </DsText>
          
        </div>
      </DsForm>

      <DsDivider/>

      <!-- Buttons -->
      <div class="flex justify-center w-full gap-5">
        <DsButton variant="outline" :color="Colors.secondary" :text-color='Colors.secondary' @click="$emit('close')">
          Close
        </DsButton>
        <DsButton :color="Colors.secondary" @click="onSubmit">
          {{ props.workspaceModel ? 'Update Model' : 'Add Model' }}
        </DsButton>
      </div>
    </div>
    </DsModalProgressCard>
  </DsModal>
</template>
