<!--
    Hyperlink
    Usage:
        <DsLink href="https://google.com" size="lg">Hello</DsText>
-->

<script setup lang="ts">
type Underline = 'always' | 'hover' | 'never';

const props = defineProps<{
    href: string;
    underline?: Underline;
    newTab?: boolean;
}>();

let underlineClass = 'underline';
if (props.underline === 'hover') { underlineClass = 'hover:underline'; }
else if (props.underline === 'never') { underlineClass = ''; }

</script>

<template>
  <NuxtLink :href="href" :class="underlineClass" :target="newTab ? '_blank' : '_self'">
    <slot />
  </NuxtLink>
</template>
