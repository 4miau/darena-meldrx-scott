<script setup lang="ts">
import { ref } from 'vue';
import { Colors } from '~/types/ui/colors';
useHead({ title: 'Component Gallery | MeldRx' });

const dsTextInputValue = ref<string>('Test Value');
const dsClientSecretInputValue = ref<string>('Secret Value');
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        Text Inputs
      </DsText>
      <div class="pb-5" />

      <DsText size="xl">
        DsTextInput
      </DsText>
      <DsTextInput v-model="dsTextInputValue" label="Text Input Label" placeholder="DsTextInput" /><br>
      <DsTextInput v-model="dsTextInputValue" label="Text Input Label (required)" placeholder="DsTextInput" :required="true" /><br>
      <DsTextInput v-model="dsTextInputValue" label="Text Input Label (required)" placeholder="DsTextInput" :required="true" :disabled="true" /><br>
      <pre>{{ dsTextInputValue }}</pre>
      <DsDivider />

      <DsText size="xl">
        DsClientSecretInput
      </DsText>
      <DsClientSecretInput v-model="dsClientSecretInputValue" :required="true" />
      <pre>{{ dsClientSecretInputValue }}</pre>
      <DsDivider />

      <DsText size="xl">
        DsTextInput with #right Content
      </DsText>
      <DsTextInput
        v-model="dsTextInputValue"
        label="Text Input Label for testing"
        placeholder="DsTextInput"
        required
        :rules="[[(v) => !!v, 'not empty']]"
      >
        <template #right>
          <DsButton :color="Colors.primary" :text-color='Colors.primary' variant="outline" @click="() => {}">
            <DsIcon name="heroicons:check-circle" />
            Click Me
          </DsButton>
        </template>
      </DsTextInput>
      <DsDivider />

      <DsText size="xl">
        DsTextInputWithCopyButton
      </DsText>
      <DsTextInputWithCopyButton v-model="dsTextInputValue" placeholder="DsTextInputWithCopyButton" /><br>
      <DsTextInputWithCopyButton v-model="dsTextInputValue" label="Text Input Label" placeholder="DsTextInputWithCopyButton" /><br>
      <DsDivider />
    </div>
  </DsContainer>
</template>
