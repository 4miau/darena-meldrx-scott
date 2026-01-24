<script setup lang="ts">
import { Colors } from '~/types/ui/colors';
useHead({ title: 'Component Gallery | MeldRx' });


const sampleToastProps : Toast = {
    title: 'Whatever',
    description: 'Just enter restricted Air Space'
}

const myToast = notification
</script>

<template>
  <DsButton
    :color="Colors.primary"
    variant="filled"
    @click="notification({
      title: 'Welcome',
      description: 'Thanks for entering restricted Air Space.',
      displayTime: 3000,
      variant: 'success',
    })"
  >
    Welcome Toast
  </DsButton>
  <br><br>
  <DsButton
    :color="Colors.primary"
    variant="filled"
    @click="notification({
      title: 'WTF',
      description: 'Entering restricted Air Space ??',
      displayTime: 3000,
      variant: 'error',
    })"
  >
    WTF Toast
  </DsButton>
  <br><br>
  <DsButton
    :color="Colors.primary"
    variant="filled"
    @click="notification({
      title: 'content content content content content content content content content content content ',
      description: 'Entering restricted Air Space ?? Entering restricted Air Space ?? Entering restricted Air Space ?? Entering restricted Air Space ?? Entering restricted Air Space ?? Entering restricted Air Space ?? Entering restricted Air Space ?? Entering restricted Air Space ?? Entering restricted Air Space ?? Entering restricted Air Space ?? ',
      displayTime: 3000,
      variant: 'error'
    });"
  >
    Alot of text!!
  </DsButton>
  <br><br>
  <DsButton
    :color="Colors.primary"
    variant="filled"
    @click="notification(sampleToastProps);"
  >
    Whatever Toast
  </DsButton>
  <br><br>
  <DsButton
    :color="Colors.primary"
    variant="filled"
    @click="myToast({
      title: 'Whatever',
      description: 'I wont go away',
      displayTime: 0
    })"
  >
    Never Expire
  </DsButton>
  <br>
</template>
