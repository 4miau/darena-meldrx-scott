<script setup lang="ts">
import { Colors } from "~/types/ui/colors";
const config = useRuntimeConfig();
const { subscription } = useSubscription();

defineProps<{
  hamburgerVisible?: boolean
}>();

const userMenuVisible = ref(false);
const {
    user,
    authenticated,
    name
} = useAuth();

const { brandingConfiguration } = useBranding();
const userMenu = ref<HTMLElement>();
const version = config.public.containerTag ?? 'v0.0.1';
const environment = config.public.containerEnv ?? 'DEV';

function toggleUserMenuVisible() {
    userMenuVisible.value = !userMenuVisible.value;
};

useClickOutsideElement(userMenu, () => { userMenuVisible.value = false });
defineEmits<{'hamburgerMenuClick': [];}>();

const headerClass = computed(() => {
    return brandingConfiguration?.value?.BackgroundColorHex
        ? `px-4 py-2.5 fixed left-0 right-0 top-0 z-40`
        : 'bg-gradient-to-r from-indigo to-secondary px-4 py-2.5 fixed left-0 right-0 top-0 z-40';
});

const styleClass = computed(() => {
    return brandingConfiguration?.value?.BackgroundColorHex
        ? { backgroundColor: brandingConfiguration.value.BackgroundColorHex }
        : {};
});

</script>

<template>
  <nav :style="styleClass" :class="headerClass">
    <div class="flex flex-wrap justify-between items-center">
      <div class="flex justify-start items-center">
        <!-- Hamburger Menu -->
        <button v-if="hamburgerVisible" class="hover:bg-cerulean rounded w-[40px] h-[40px] flex items-center justify-center" @click="$emit('hamburgerMenuClick')">
          <DsIcon size="lg" name="heroicons:bars-3" :color='Colors.white' />
        </button>

        <!-- MeldRx Logo -->
        <DsLink href="/" underline="never" class="flex pl-2 items-center justify-between mr-4 text-white">
          <div>
            <img v-if="brandingConfiguration?.LogoImageUri" :src="`${config.public.storageUrl}/${brandingConfiguration.LogoImageUri}`" class="h-14 w-auto">
            <div v-else class="pb-[5px]">
              <DarenaSolutionsLogo />
            </div>
          </div>
            <DsText v-if="!brandingConfiguration?.Title" size="2xl" weight="light" :color="Colors.white" class="text-4xl pl-2">
              <span class="font-normal">Darena Solutions</span> | MeldRx
            </DsText>
            <DsText v-else size="2xl" weight="light" :color="Colors.white" class="text-4xl pl-2">{{ brandingConfiguration.Title }}</DsText>
        </DsLink>
      </div>
      <div v-if="authenticated" ref="userMenu" class="flex items-center lg:order-2">
        <button id="action-menu-button" class="hover:bg-cerulean rounded flex items-center justify-center px-2" @click="toggleUserMenuVisible">
          <DsAvatar size="sm" :alt="name.toUpperCase()" class="border border-2 border-dsprimary bg-bliss text-dsgray" />
          <span class="text-white text-xl mx-2 cursor-pointer h-full flex items-center">
            <DsIcon name="heroicons:chevron-down" />
          </span>
        </button>
        <UserMenu
          v-if="userMenuVisible"
          :email="user?.claims?.email"
          :organization-name="subscription?.organizationName"
          class="user-profile fixed right-5 top-[60px] z-[999]"
          :version="version"
          :environment="environment"
          @close="userMenuVisible = false"
        />
      </div>
      <div v-else>
        <DsButton id="login-button" @click="externalNavigation.signIn()">
          Log In
        </DsButton>
        <div class="inline pl-2" />
        <DsButton id="signup-button" variant="subtle" :text-color='Colors.gray' @click="externalNavigation.signUp()">
          Sign Up
        </DsButton>
      </div>
    </div>
  </nav>
</template>
