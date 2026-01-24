<script setup lang="ts">
import { defaultScopes } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Scopes';

useHead({ title: 'Component Gallery | MeldRx' });

const workspaceConfig = [
    { value: 'external', title: 'External' },
    { value: 'meldrx', title: 'MeldrRx' },
    { value: 'both', title: 'Both' }
];
const currentWorkspaceSelection = ref(workspaceConfig[0].value)

const dsSelectState = reactive({
    selected: '',
    placeholder: 'Select an option',
    searchPlaceholder: 'Search options',
    options: [
        { value: 'external', title: 'External' },
        { value: 'meldrx', title: 'MeldrRx' },
        { value: 'both', title: 'Both' }
    ],

    differentSearchValues: [
        { value: 'spanish', title: 'Spanish' },
        { value: 'english', title: 'English' },
        { value: 'russian', title: 'Russian' }
    ],

    code1: `<DsSelect
      :search-options="(q) => q ? searchOptions.filter(x => x.value.includes(q)) : options"
    />`,

    code2: `<DsSelect
      :search-options="async (query) => await searchApi(query).then(r => [{...}, {...}])"
    />`
})

const scopeSelector = reactive({
    value: '  openid profile'
})

</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        Selects (Dropdowns)
      </DsText>
      <div class="pb-5" />

      <DsText size="xl">
        DsSelect
      </DsText>

      <DsSelect v-model="currentWorkspaceSelection" :items="workspaceConfig" />
      <div class="pb-4" />
      <pre>
Selected Option: {{ currentWorkspaceSelection }}
All Options: {{ workspaceConfig }}
      </pre>
      <DsDivider />
    </div>

    <div>
      <DsText size="2xl" weight="light">
        Search Select
      </DsText>
      <div class="pb-5" />

      <DsText size="xl">
        DsSearchSelect
      </DsText>

      <DsSelect
        v-model="dsSelectState.selected"
        :items="workspaceConfig"
        :placeholder="dsSelectState.placeholder"
        searchable
        :search-placeholder="dsSelectState.searchPlaceholder"
      />
      <div class="flex items-center mt-1">
        <DsText class="mr-4" size="md">
          Placeholder:
        </DsText>
        <DsTextInput v-model="dsSelectState.placeholder" />
      </div>
      <div class="flex items-center mt-1">
        <DsText class="mr-4" size="md">
          Search Placeholder:
        </DsText>
        <DsTextInput v-model="dsSelectState.searchPlaceholder" />
      </div>
      <div class="pb-4" />
      <pre>
Selected Option: {{ dsSelectState.selected }}
All Options: {{ dsSelectState.options }}
      </pre>
      <DsDivider />
    </div>

    <div>
      <DsText size="2xl" weight="light">
        Search Select with different search set.
      </DsText>

      <div class="pb-5" />

      <DsText size="xl">
        DsSearchSelect
      </DsText>

      <DsSelect
        v-model="dsSelectState.selected"
        :items="dsSelectState.options"
        :placeholder="dsSelectState.placeholder"
        searchable
        :search-placeholder="dsSelectState.searchPlaceholder"
        />
      <div class="pb-4" />

      <DsText size="md" weight="light">
        With custom options during search.
      </DsText>
      <DsCodeblock :code="dsSelectState.code1" language="javascript" />
      <DsText size="md" weight="light">
        Can be an async function.
      </DsText>
      <DsCodeblock :code="dsSelectState.code2" language="javascript" />
      <div class="pb-5" />

      <pre>
Different options duirng search: {{ dsSelectState.differentSearchValues }}
      </pre>
      <DsDivider />
    </div>

    <div>
      <DsText size="2xl" weight="light">
        Scope Selector
      </DsText>
      <div class="pb-5" />

      <DsText size="xl">
        ScopeSelector
      </DsText>

      <ScopeSelector
        v-model="scopeSelector.value"
        :scopes="defaultScopes"
      />
      <div class="pb-4" />
      <pre>
Scopes: {{ scopeSelector.value }}
      </pre>
      <DsDivider />
    </div>
  </DsContainer>
</template>
