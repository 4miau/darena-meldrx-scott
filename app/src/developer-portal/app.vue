<template>
  <NuxtLayout>
    <NuxtPage />
    <DsNotification />
    <DsGlobalConfirmation />
  </NuxtLayout>
</template>

<style>

/* break margin collapsing: https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_box_model/Mastering_margin_collapsing */
body > div:first-child {
  border-bottom: 1px solid transparent;
  margin-bottom: -1px;
}
</style>
