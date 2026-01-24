<script setup lang="ts">
import type { MenuItem } from '~/types/menu/sidebarMenu'
import { Colors } from "~/types/ui/colors";

const { menuButtonColor, menuMouseOverColor} = useBranding();

const headerClassMainMenu = computed(() => {
    return { 'bg-indigo hover:bg-cerulean': !(menuButtonColor.value && menuMouseOverColor.value) }
});
const styleClassMainMenu = computed(() => {
    return menuButtonColor.value ? { backgroundColor: menuButtonColor.value } : { };
});

const getHeaderClassSubMenu = (isActive: boolean) => {
    return {
        'bg-indigo hover:bg-cerulean': !(menuButtonColor.value && menuMouseOverColor.value),
        'bg-cerulean border-l-4 border-jasmine pl-9': isActive
    };
};

const styleClassSubMenu = computed(() => {
    return menuButtonColor.value ? { backgroundColor: menuButtonColor.value} : { };
});

const props = defineProps<{
  menuTreeItem: MenuItem;
}>()

const displaySubMenu = ref(!!props.menuTreeItem.active)
const dropMenuIcon = computed(() => displaySubMenu.value ? 'heroicons:chevron-up' : 'heroicons:chevron-down')

function toggleSubMenu () {
    displaySubMenu.value = !displaySubMenu.value
}

watch(
    () => props.menuTreeItem.active,
    (v) => {
        if (v) {
            displaySubMenu.value = true
        }
    }
)
</script>

<template>
    <div class='border-b border-solid border-secondary'>
        <button
            :class='headerClassMainMenu'
            class='flex justify-between w-full ds-button py-1 ds-button--filled-primary rounded-none pl-6'
            :style='styleClassMainMenu'
            @click="toggleSubMenu"
            @mouseover="menuMouseOverColor ? ($event as any).currentTarget.style.backgroundColor = menuMouseOverColor : null"
            @mouseleave="menuButtonColor ? ($event as any).currentTarget.style.backgroundColor = menuButtonColor : null"
            >
            <DsText size='md' weight='light' :color='Colors.white'>{{ menuTreeItem.label }}</DsText>
            <DsIcon :name="dropMenuIcon" size='xs' :color='Colors.white' class='mr-2 mt-0.5' />
        </button>
    </div>
    <div v-if='displaySubMenu'>
        <div v-for="item in props.menuTreeItem.subMenu.filter(x => !x.hide)" :key="item.label" class="border-b border-solid border-lapis-lazuli">
            <DsLink underline='never' :href='item.path' :target='item.target ?? "_self"'>
                <button
                    :class='getHeaderClassSubMenu(!!item.active)'
                    class='flex w-full ds-button py-1.5 ds-button--filled-primary rounded-none pl-10'
                    :style='styleClassSubMenu'
                    @mouseover="menuMouseOverColor ? ($event as any).currentTarget.style.backgroundColor = menuMouseOverColor : null"
                    @mouseleave="menuButtonColor ? ($event as any).currentTarget.style.backgroundColor = menuButtonColor : null"
                >
                <DsText size='xs' weight='light' :color='Colors.white'>{{ item.label }}</DsText>
                </button>
            </DsLink>
        </div>
    </div>
</template>