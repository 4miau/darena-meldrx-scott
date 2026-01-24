<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
useHead({ title: 'Component Gallery | MeldRx' });
</script>

<template>
  <DsContainer>
    <div>
      <DsButton :color='Colors.primary'>
        Hello!
      </DsButton>
      <DsButton :color='Colors.primary'>
        <DsIcon name='heroicons:x-mark'/>
        Hello!
      </DsButton>
      <DsButton :color='Colors.primary'>
        Hello!
        <DsIcon name='heroicons:x-mark'/>
      </DsButton>
      <DsButton :color='Colors.primary'>
        <DsIcon name='heroicons:x-mark'/>
      </DsButton>
    </div>
  </DsContainer>
</template>
