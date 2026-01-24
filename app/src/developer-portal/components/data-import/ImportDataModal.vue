<script setup lang="ts">
import type { UploadDocumentTypes } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/UploadDocumentTypes'
import { Colors } from '~/types/ui/colors'

interface IProps {
  show: boolean;
  showDescriptionDate?: boolean;
  title: string;
  description: string;
  allowedImportTypes: UploadDocumentTypes[],
}

// Set default properties...
const props = withDefaults(defineProps<IProps>(), {
    show: false,
    showDescriptionDate: false,
    title: 'Import Data',
    description: 'Import data'
})

const emit = defineEmits<{
  'importData': [value: { fileContents: string, fileExtension: string }];
  'importDataWithDescriptionDate': [value: { fileContents: ArrayBuffer, fileDate: string, fileDescription: string, fileName: string }];
  'close': [];
  'cancel': [];
}>()

const formRef = ref<FormRef>()
const state = reactive<{
  files?: FileList,
  date: string
  description: string
}>({
    date: DateTime.toISODateString(DateTime.nowDate()),
    description: '',
})

watch(() => props.show, (newVal) => {
    if (newVal) {
        state.files = undefined;
        state.date = DateTime.toISODateString(DateTime.nowDate());
        state.description = '';
    }
});

const placeholderText = computed(() => {
    const fileTypes = props.allowedImportTypes
        .flatMap(importType => importType.fileExtension);

    if (fileTypes.length === 1) {
        return `Upload a ${fileTypes[0]} file up to 20MB`;
    } else if (fileTypes.length === 2) {
        return `Upload a ${fileTypes[0]} or ${fileTypes[1]} file up to 20MB`;
    } else if (fileTypes.length > 2) {
        return `Upload a ${fileTypes.slice(0, -1).join(' ')} or ${fileTypes[fileTypes.length - 1]} file up to 20MB`;
    }
    return 'Upload a file up to 20MB'
})

async function onImportDataClick () {
    const isValid = formRef?.value?.validate() ?? false
    if (!isValid) {
        return
    }
    const file = state.files?.item(0)
    if (!file) {
        return
    }

    // technical dept: props.showDescriptionDate means we may upload a non-text file type.
    if (props.showDescriptionDate) {
        emit('importDataWithDescriptionDate', {
            fileContents: await file.arrayBuffer(),
            fileDate: state.date,
            fileDescription: state.description,
            fileName: file.name
        })
    } else {
        emit('importData', {
            fileContents: await file.text(),
            fileExtension: file.name.split('.').pop() ?? ''
        })
    }
}
</script>

<template>
  <DsModal :model-value="show" @close="$emit('close')">
    <DsModalProgressCard :total-steps="1" :current-step="1">
      <DsForm ref="formRef">
        <!-- Icon -->
        <div class="flex w-full py-8 bg-space justify-center items-center">
          <div class="flex w-full items-center justify-center">
            <div class="w-14 h-14 rounded-full flex justify-center items-center pt-2" style="background-color:#E79288">
              <MeldRxClipboard class="mb-2"/>
            </div>
          </div>
          <div class="flex-1"/>
        </div>

        <div class="flex w-full p-4">
          <div class="flex flex-col w-full">
            <!-- Heading -->
            <div class="w-full flex-col justify-center items-center gap-5 flex">
              <DsText size="2xl" weight="light">
                {{ title }}
              </DsText>

              <DsText size="sm" weight="light" class="text-center">
                {{ description }}
                <ul>
                  <li v-for="item in allowedImportTypes" :key="item.fileExtension.toString">{{ item.fileDescription }}</li>
                </ul>
              </DsText>
            </div>

            <div class="grid grid-cols-1 gap-4">
              <DsFileSelector
                  v-model="state.files"
                  label="File"
                  :placeholder-text="placeholderText"
                  :required="true"
                  :allowed-extensions="allowedImportTypes.flatMap(x => x.fileExtension)"
                  :multiple="false"
                  :hide-file-contents-preview="true"
                  :rules="[[(v?: string) => !!v, 'File is required']]"
              />
              <template v-if="showDescriptionDate">
                <!-- Date of Document -->
                <DsLabeledInput label="Date of Document" :required="true">
                  <div class="justify-start items-start inline-flex w-full">
                    <DsDatePicker
                        v-model="state.date"
                        :past-only="true"
                        :required="true"
                        :rules="[
                      [v => !!v, 'Date of Document']
                      ,[v => !isNaN(Date.parse(v as string)), 'Date is not valid']
                      ,[v => { const today = new Date(); const inputDate = new Date(v as string); return inputDate < today;}, 'Date of document must be in the past']
                    ]"
                    />
                  </div>
                </DsLabeledInput>
                <!-- File Description -->
                <DsTextInput
                    v-model="state.description"
                    :required="true"
                    :rules="[ValidationRules.describeNotEmpty('File description is required')]"
                    type="string"
                    label="Description"
                />
              </template>
            </div>

            <!-- Buttons -->
            <DsContainer>
              <div class="flex justify-center w-full gap-5">
                <DsButton :color="Colors.secondary" :text-color='Colors.secondary' variant="outline" @click="$emit('cancel')">
                  Cancel
                </DsButton>
                <DsButton :color="Colors.secondary" @click="() => onImportDataClick()">
                  Upload
                </DsButton>
              </div>
            </DsContainer>
          </div>
        </div>
      </DsForm>
    </DsModalProgressCard>
  </DsModal>
</template>
