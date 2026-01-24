<script setup lang="ts">

const props = defineProps<{
  show: boolean;
  persistent?: boolean;
}>()

defineEmits<{
  close: []
}>()

const drawerClass = computed(() => [
    props.show ? 'translate-x-0' : 'translate-x-full',
])

</script>

<template>

  <div
      v-if="show"
      class="fixed inset-0 bg-space bg-opacity-50 z-40"
      @click="() => { if(!props.persistent){ $emit('close') } }"
  />

  <div
      class="drawer fixed top-0 right-0 z-50 min-h-screen max-h-screen overflow-auto w-1/3 bg-white flex flex-col transition-transform ease-in-out overflow-x-hidden"
      :class="drawerClass"
  >
    <div v-if="$slots.header"  class="pt-5 px-5">
      <slot name="header"/>
      <DsDivider/>
    </div>
    <div class="overflow-y-auto px-5 grow" :class="{'pt-5': !$slots.header, 'pb-5': !$slots.footer}">
      <slot/>
    </div>
    <div v-if="$slots.footer" class="pb-5 px-5">
      <DsDivider/>
      <slot name="footer"/>
    </div>
  </div>
</template>

<style scoped>
div.drawer {
  box-shadow: 5px 0 15px 0;
}
</style>
