<!--
    Form component to centralize validation of containing inputs

    Usage:
        <DsForm ref="someRef">
            <DsTextInput ... required :rules="..." />
            <DsTextInput ... required :rules="..." />

            other inputs...

        </DsForm>

    then:
    <script>
        // will bind to the DsForm automatically because of 'ref' attribute
        const formRef = ref<FormRef>()

        function triggerValidation() {
            // returns true if all required fields are valid (triggers evaluation of rules on inputs)
            const requiredFieldsAreValid = formRef.value!.validate()
        }
    </script>

    - formRef.value!.validate() will make input's dirty displaying errors.
 -->

<script setup lang="ts">
import { useDefineForm } from '~/composables/useForm';

const form = useDefineForm()

defineExpose(form)
</script>

<template>
  <slot />
</template>
