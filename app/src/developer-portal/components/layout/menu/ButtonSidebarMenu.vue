<script setup lang="ts">
import {Colors} from '~/types/ui/colors'

const { menuButtonColor, menuMouseOverColor} = useBranding();
const props = defineProps<{
    label?: string;
    path?: string;
    target?: '_blank' | '_self' | '_parent' | '_top';
    fontSize?: string;
    active?: boolean;
 }>()

const headerClass = computed(() => {
    return {
        'pl-6': !props.active,
        'bg-indigo hover:bg-cerulean': !(menuButtonColor.value && menuMouseOverColor.value),
        'bg-cerulean border-l-4 border-jasmine pl-5': props.active,
    };
});
const styleClass = computed(() => {
    return menuButtonColor.value
        ? { backgroundColor: menuButtonColor.value, fontSize: props.fontSize }
        : { fontSize: props.fontSize };
});

</script>

<template>
  <div class="border-b border-solid border-secondary">
    <DsLink underline='never' :href='path ?? "#"' :target='target ?? "_self"'>
        <button
            :class="headerClass"
            class='flex items-center w-full ds-button py-1 ds-button--filled-primary'
            :style="styleClass"
            @mouseover="menuMouseOverColor ? ($event as any).currentTarget.style.backgroundColor = menuMouseOverColor : null"
            @mouseleave="menuButtonColor ? ($event as any).currentTarget.style.backgroundColor = menuButtonColor : null"
        >
        <DsText size='md' weight='light' :color='Colors.white'>{{ label }}</DsText>
    </button>
    </DsLink>
  </div>
</template>
