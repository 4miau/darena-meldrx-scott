<!--
    Displays a Linked App (Logo + App Name + Circle Selector)
-->

<script setup lang="ts">

const props = defineProps<{
    logosrc?: string,
    title: string,
    selected?: string
}>()

defineEmits(['click'])

const cssBackground = computed(() => { return props.selected === props.title ? 'bg-pine' : 'bg-transparent' })

</script>

<template>
  <div class="flex justify-between cursor-pointer" @click="$emit('click',title)">
    <div class="flex-[0_0_auto] mr-auto">
      <img :src="logosrc" alt="EHR Logo" style="width:30px">
    </div>
    <div class="flex-1 text-left ml-4">
      <DsText size="sm">
        {{ title }}
      </DsText>
    </div>
    <div class="flex-[0_0_auto] w-[15px] h-[15px] transition-[background-color] duration-[0.3s] ease-[ease] border ml-auto rounded-[50%] border-[black]" :class="cssBackground" />
  </div>
</template>
