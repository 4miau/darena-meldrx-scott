<script setup lang="ts">
useHead({ title: 'Component Gallery | MeldRx' });
const currentTab = ref(0);
</script>

<template>
  <DsContainer>
    <div>
      <DsText size="2xl" weight="light">
        DsTabHeader
      </DsText>
      <div class="pb-5" />

      <DsText size="xl" class="block">
        Active Only
      </DsText><br>
      <div class="justify-start items-start gap-1.5 inline-flex">
        <DsTabHeader title="Tab 1 (active only)" subtitle="Tab 1 subtitle" :enabled="currentTab == 0" @click="currentTab = 0" />
        <DsTabHeader title="Tab 2 (active only)" subtitle="Tab 2 subtitle" :enabled="currentTab == 1" @click="currentTab = 1" />
      </div><br><br>

      <div v-if="currentTab === 0">
        <div>Tab 1 Content</div>
      </div>
      <div v-if="currentTab === 1">
        <div>Tab 2 Content</div>
      </div>
      <DsDivider />

      <DsText size="xl" class="block">
        With Progression
      </DsText><br>
      <div class="justify-start items-start gap-1.5 inline-flex">
        <DsTabHeader title="Tab 1 (with progression)" subtitle="Tab 1 subtitle" :enabled="currentTab >= 0" @click="currentTab = 0" />
        <DsTabHeader title="Tab 2 (with progression)" subtitle="Tab 2 subtitle" :enabled="currentTab >= 1" @click="currentTab = 1" />
      </div><br><br>

      <div v-if="currentTab === 0">
        <div>Tab 1 Content</div>
      </div>
      <div v-if="currentTab === 1">
        <div>Tab 2 Content</div>
      </div>
      <DsDivider />
    </div>
  </DsContainer>
</template>
