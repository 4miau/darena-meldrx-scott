<script setup lang="ts">
import { Colors } from "~/types/ui/colors";

const props = defineProps<{
    email?: string;
    organizationName?: string;
    version?: string;
    environment?: string;
}>();

const emit = defineEmits<{
  close: []
}>()

function changeContext(){
    externalNavigation.changeContext()
    emit('close')
}

// TECH DEBT ALERT - This is a fix for version number to protect us against audits since our version on CHPL is v2.0.
// They audit for Major and Minor version randomly, so if we got audited, we'd be in trouble.
const formattedVersion = computed(() =>
    props.version?.replace(/v(\d+)\./, "v$1.0.") ?? ""
);

const currentYear = new Date().getFullYear();
</script>

<template>
  <div class="bg-indigo rounded p-4 flex-col justify-start items-start inline-flex">
    <!-- Email -->
    <DsText size="xs" weight="light" :color="Colors.white">
      Signed in as
    </DsText>
    <DsText size="md" weight="light" :color="Colors.white">
      {{ email }}
    </DsText>
    <div class="pb-5" />

    <!-- Organization -->
    <div v-if="organizationName" class="flex-col inline-flex">
      <DsText size="xs" weight="light" :color="Colors.white">
        Current organization
      </DsText>
      <DsText size="md" weight="light" :color="Colors.white">
        {{ organizationName }}
      </DsText>
      <div class="pb-5" />
    </div>

    <!-- Account Settings / Signout -->
    <button id="account-settings-menu-item" class="flex items-center hover:underline text-white pb-2" @click="changeContext">
      <DsIcon name="heroicons:arrows-right-left" size='sm' />
      <DsText size="md" weight="light" :color="Colors.white" class="pl-4">
        Change Context
      </DsText>
    </button>

    <button id="account-settings-menu-item" class="flex items-center hover:underline text-white pb-2" @click="externalNavigation.manageAccount()">
      <DsIcon name="heroicons:cog-8-tooth" size='sm' />
      <DsText size="md" weight="light" :color="Colors.white" class="pl-4">
        Account Settings
      </DsText>
    </button>

    <button id="sign-out-menu-item" class="flex items-center hover:underline text-white" @click="externalNavigation.signOut()">
      <DsIcon name="heroicons:arrow-right-on-rectangle" size='sm' />
      <DsText size="md" weight="light" :color="Colors.white" class="pl-4">
        Sign Out
      </DsText>
    </button>
    <DsDivider class="my-2" />

    <DsText size="xs" weight="light" :color="Colors.white">
      Darena Solutions © {{ currentYear }} | All rights reserved
    </DsText>
    <div class="pb-2" />

    <DsText size="xs" weight="light" :color="Colors.white">
      Version {{ formattedVersion }} {{ environment !== 'prd' ? '| ' + environment : '' }}
    </DsText>
  </div>
</template>
