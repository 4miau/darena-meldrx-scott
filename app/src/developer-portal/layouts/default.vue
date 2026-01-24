<script setup lang="ts">
// Left-Margin must be equal to the sidebar width.
// If you change the width of the sidebar, please update the left-margin here.
// If sidebar is not shown, do not add any left-margin.
const contentLeftMarginClass = computed(() => { return isShowingSidebar.value ? 'ml-[225px]' : ''; });

const isShowingSidebar = ref<boolean>(true);
const toggleSidebar = () => { isShowingSidebar.value = !isShowingSidebar.value; };
</script>

<template>
  <AppHeader hamburger-visible @hamburger-menu-click="toggleSidebar" />
  <AppSideMenu v-if="isShowingSidebar" />
  <main class="pt-[74px] pb-[50px]" :class="contentLeftMarginClass">
    <slot />
  </main>
  <AppFooter :show-graphics="isShowingSidebar" />
</template>
