<script setup lang="ts">
const showDrawer = ref<boolean>(false)

</script>

<template>
  <DsContainer>
    <h1>Sample Drawer</h1>
    <DsButton @click="showDrawer = !showDrawer">
      Toggle Drawer
    </DsButton>
    <DsDrawer
        :show="showDrawer"
        @close="showDrawer = false"
    >
      <template #header>
        Header
      </template>

      <div>
        Body
      </div>

      <template #footer>
        Footer
      </template>
    </DsDrawer>
  </DsContainer>
</template>
