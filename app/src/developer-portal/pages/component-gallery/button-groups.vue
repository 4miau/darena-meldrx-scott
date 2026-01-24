<script setup lang="ts">
import { ref } from 'vue';
import { Colors } from '~/types/ui/colors';
import type { WorkspaceType } from '~/types/meldrx-api/workspaces';
import type { IDsSingleSelectButtonListItem } from '~/types/ui/DsSingleSelectButtonList';
import { SoFAppTokenAuthMethod } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppTokenAuthMethod';
import { SoFAppUserType } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';
useHead({ title: 'Component Gallery | MeldRx' });

const userTypeOptions: IDsSingleSelectButtonListItem<SoFAppUserType>[] = [
    { value: SoFAppUserType.Patient, title: 'Patient' },
    { value: SoFAppUserType.Provider, title: 'Provider' },
    { value: SoFAppUserType.System, title: 'System' }
];

const currentButton = ref('two');
const currentButtonVertical = ref(false);
const currentButtonMultiSelect = ref(false);

const currentGenericOptionUserType = ref<SoFAppUserType>(SoFAppUserType.Patient);
const currentAuthType = ref<SoFAppTokenAuthMethod>(SoFAppTokenAuthMethod.Public);
const currentWorkspaceType = ref<WorkspaceType>('linked');
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        Button Groups
      </DsText>
      <div class="pb-5" />

      <DsText size="xl">
        Generic Options
      </DsText>
      <DsSingleSelectButtonList v-model="currentGenericOptionUserType" :options="userTypeOptions" />
      <pre>{{ currentGenericOptionUserType }}</pre>
      <DsDivider />

      <DsText size="xl" class="block">
        DsButtonGroup
      </DsText>
      <DsButton @click="currentButtonVertical = !currentButtonVertical">
        {{ currentButtonVertical ? 'Change to Horizontal' : 'Change to Vertical' }}
      </DsButton>
      <DsButton @click="currentButtonMultiSelect = !currentButtonMultiSelect">
        {{ currentButtonMultiSelect ? 'Single Select' : 'Multi Select' }}
      </DsButton>
      <DsButtonGroup
        v-model="currentButton"
        :vertical="currentButtonVertical"
        :multiple="currentButtonMultiSelect"
        :active-color="Colors.primary"
        required
        :rules="[
          [(v) => (v?.length ?? 0) > 0, 'at least one selection required']
        ]"
      >
        <DsButton value="one" variant="filled">
          One
        </DsButton>
        <DsButton value="two" variant="filled">
          Two
        </DsButton>
        <DsButton value="three" variant="filled">
          Three
        </DsButton>
      </DsButtonGroup>
      <pre>{{ currentButton }}</pre>
      <DsDivider />

      <DsText size="xl">
        WorkspaceTypeSelect
      </DsText>
      <WorkspaceTypeSelect v-model="currentWorkspaceType" />
      <pre>{{ currentWorkspaceType }}</pre>
      <DsDivider />

      <DsText size="xl">
        AuthenticationClientTypeSelect
      </DsText>
      <AuthenticationClientTypeSelect v-model="currentAuthType" />
      <pre>{{ currentAuthType }}</pre>
      <DsDivider />

    </div>
  </DsContainer>
</template>
