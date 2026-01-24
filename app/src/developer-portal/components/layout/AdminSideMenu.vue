<script setup lang='ts'>
import ButtonSidebarMenu from './menu/ButtonSidebarMenu.vue'
import ButtonGroupSidebarMenu from './menu/ButtonGroupSidebarMenu.vue'
import { type NavMenuItems, resolveActiveRoute } from '~/types/menu/sidebarMenu'

const router = useRouter();

function getBaseUrl() {
    const { protocol, host } = window.location;
    return `${protocol}//${host}`;
}

const menuTree = computed<NavMenuItems[]>(() => {
    const result: NavMenuItems[] = [
        {
            group: 'entities',
            menuItems: [
                { label: 'Organizations', subMenu: [], path: '/administrator/organizations' },
                { label: 'Workspaces', subMenu: [], path: '/administrator/workspaces' },
                { label: 'Apps', subMenu: [], path: '/administrator/apps' },
                { label: 'Subscriptions', subMenu: [], path: '/administrator/subscriptions' }
            ]
        },
        {
            group: 'links',
            menuItems: [
                { label: 'Identity Admin', subMenu: [], path: `${getBaseUrl()}/Admin`, target: '_blank' },
                { label: 'MyMipsScore', subMenu: [], path: `${getBaseUrl()}/mymipsscore`, target: '_blank' },
                { label: 'App Marketplace', subMenu: [], path: '/marketplace' },
                { label: 'Directory', subMenu: [], path: '/directory', target: '_blank' }
            ]
        }
    ]

    for (const navMenuItem of result) {
        resolveActiveRoute(router.currentRoute.value, navMenuItem.menuItems)
    }

    return result
})
</script>

<template>
  <!-- Top must be equal to the navbar height -->
  <div class="pt-[74px] w-[225px] pb-[50px] bg-secondary fixed left-0 h-screen overflow-y-auto">
    <div v-for="group in menuTree" :key="group.group" class="my-3">
      <div v-for="item in group.menuItems" :key="item.label">
        <div v-if="item.subMenu.length == 0">
          <ButtonSidebarMenu
            :label="item.label" :path="item.path" :target="item.target" :font-size="item.fontSize"
            :active="item.active" />
        </div>
        <div v-else>
          <ButtonGroupSidebarMenu :menu-tree-item="item" />
        </div>
      </div>
    </div>
  </div>
</template>
