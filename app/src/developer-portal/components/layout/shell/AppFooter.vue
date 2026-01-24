<script setup lang="ts">
import { Colors } from "~/types/ui/colors";

defineProps<{
  showGraphics: boolean;
}>();
const { footerColor } = useBranding();
const currentYear = new Date().getFullYear();

const headerClass = computed(() => {
    return footerColor.value
        ? `sticky bottom-0 h-12 flex items-center px-4`
        : 'sticky bottom-0 h-12 bg-dsprimary flex items-center px-4';
});

const styleClass = computed(() => {
    return footerColor.value
        ? { backgroundColor: footerColor.value }
        : {};
});

</script>

<template>
  <div class="fixed bottom-0 w-full z-30">
    <footer :class="headerClass" :style="styleClass" >
      <div class="flex items-center gap-4 text-white text-sm font-medium leading-none">
        <DsText size="md" :color="Colors.white">
          Darena Solutions | MeldRx © {{ currentYear }}
        </DsText>
        <span> | </span>
        <DsLink underline="hover" href="https://www.darenasolutions.com/terms-of-service">
          Terms of Service
        </DsLink>
        <span> | </span>
        <DsLink underline="hover" href="https://www.darenasolutions.com/privacy-policy">
          Privacy Policy
        </DsLink>
        <span> | </span>
        <DsLink underline="hover" href="https://www.darenasolutions.com/trademark-usage">
          Trademark Usage
        </DsLink>
        <DevOnly>
          <span> | </span>
          <DsLink underline="hover" href="/component-gallery">
            Component Gallery
          </DsLink>
        </DevOnly>
      </div>
    </footer>
  </div>
</template>
