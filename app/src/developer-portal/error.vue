<script setup lang="ts">
import type { NuxtError } from '#app'
import { Colors } from '~/types/ui/colors'

const props = defineProps<{
  error: NuxtError
}>()

const ehrLaunchUrl = computed(() => {
    if(!props.error.data){
        return undefined;
    }

    if(typeof props.error.data !== 'object'){
        return undefined;
    }

    if('ehrLaunchUrl' in props.error.data){
        return props.error.data.ehrLaunchUrl
    }

    return undefined
})

</script>

<template>
  <NuxtLayout>
    <DsContainer>
      <template v-if="error?.statusCode === 404">
        <DsText size="2xl" weight="light">
          Nothing to see here...
        </DsText>
        <div class="pb-4"/>
        <DsText size="sm" weight="light">
          Sorry, the page you're looking for doesn't exist. Please check the URL and try again.
        </DsText>
        <div class="pb-2"/>
        <img src="~/assets/images/misc/default404.gif" alt="404 Error">
      </template>
      <template v-if="error?.statusCode === 440">
        <template v-if="ehrLaunchUrl">
          <div class="space-y-4">
            <div>
              <DsText size="2xl" weight="light">
                Session expired...
              </DsText>
            </div>
            <div>
              <DsText size="sm" weight="light">
                Your session has expired. Click below to sign in.
              </DsText>
            </div>
            <div>
              <DsButton id='sign-in-button' :color="Colors.secondary" @click="navigateTo(ehrLaunchUrl, {external: true})">
                Sign In
              </DsButton>
            </div>
          </div>
        </template>
        <template v-else>
          <div class="space-y-4">
            <div>
              <DsText size="2xl" weight="light">
                Session expired...
              </DsText>
            </div>
            <div>
              <DsText size="sm" weight="light">
                Your session has expired. Please close this window and launch the app again.
              </DsText>
            </div>
          </div>
        </template>
      </template>
    </DsContainer>
  </NuxtLayout>
</template>
