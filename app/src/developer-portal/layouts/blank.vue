<script setup lang="ts">
</script>

<template>
  <div>
    <AppHeader />
    <main class="pt-[74px] pb-[50px]">
      <slot />
    </main>
    <AppFooter :show-graphics="false" />
  </div>
</template>
