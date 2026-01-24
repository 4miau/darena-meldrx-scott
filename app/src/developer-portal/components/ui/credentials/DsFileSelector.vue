<!--
    Input for a private key.

    Usage:
        <DsFileSelector
            v-model="clientSecret"
        />
 -->

<script setup lang="ts">
import { Colors } from '~/types/ui/colors';


const props = defineProps<{
  modelValue?: FileList;
  required?: boolean;
  rules?: ValidationRule<string>[];
  label?: string;
  placeholderText?: string;
  multiple?: boolean;
  allowedExtensions?: string[];
  hideClearButton?: boolean;
}>()

const emit = defineEmits<{
  'update:modelValue': [value?: FileList];
}>()

const fileNames = computed(() => {
    if (props.modelValue) {
        return Array.from(props.modelValue).map(file => file.name).join('; ')
    }
    return ''
})

// When a file is uploaded...
function onFileUpload (event: Event) {
    dirty.value = true
    if (!event.target) {
        return
    }

    const inputElement = event.target as HTMLInputElement
    if (!inputElement) {
        return
    }

    const files = inputElement.files
    if (!files || files.length === 0) {
        return
    }

    // Check if extension is allowed...
    for (let i = 0; i < files.length; i++) {
        const tmpfileName = files[i].name
        const fileExtension = tmpfileName.split('.').pop() ?? ''
        if (props.allowedExtensions) {
            if (!fileExtension || !props.allowedExtensions.includes('.' + fileExtension)) {
                return
            }
        }
    }

    emit('update:modelValue', files)
}

// "Clear" button...
function onClearClick () {
    dirty.value = true
    emit('update:modelValue', undefined)
}

const {
    dirty,
    validationResult
} = useValidation(
    () => props.modelValue ? Array.from(props.modelValue!).map(f => f.name).join(',') as string : '',
    () => props.rules,
    () => props.required ?? false
)
</script>

<template>
  <div>
    <DsLabeledInput :label="label" :required="required">
      <div class="w-full">
        <label class="flex flex-col items-center justify-center w-full h-40 border-2 border-silver border-dashed rounded-lg cursor-pointer bg-bliss">
          <div class="flex flex-col items-center justify-center py-5 px-16">
            <DsIcon name="heroicons:cloud-arrow-up" size="xl" class="mb-4"/>
            <p class="mb-2 text-sm text-onyx">
              {{ placeholderText }}
            </p>
          </div>
          <input
              ref="fileInput"
              type="file"
              class="hidden"
              :multiple="multiple"
              :accept="allowedExtensions?.length ? allowedExtensions.join(',') :undefined"
              @change="onFileUpload"
          >
        </label>
      </div>

      <template v-if="modelValue != undefined">

        <DsText size="xs">
          {{ fileNames }}
        </DsText>

        <div class="py-2"/>
      </template>

      <template v-if="!hideClearButton">
          <div class="py-1"/>
          <DsButton @click="onClearClick">
            Clear
          </DsButton>
      </template>

    </DsLabeledInput>

    <div v-if="!validationResult.isValid" class="relative mb-6">
      <DsText size="xs" :color="Colors.fire" class="absolute top-0">
        {{ validationResult.error }}
      </DsText>
    </div>
  </div>
</template>
