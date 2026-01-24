<script setup lang='ts'>
import ButtonSidebarMenu from './menu/ButtonSidebarMenu.vue'
import ButtonGroupSidebarMenu from './menu/ButtonGroupSidebarMenu.vue'
import { type NavMenuItems, resolveActiveRoute } from '~/types/menu/sidebarMenu'
import { MeldRxSubscription } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/MeldRxSubscription';

const router = useRouter()
const { permissions } = useAuth();
const { subscription } = useSubscription();
const { menuBackgroundColor } = useBranding();
const baseUrl = window.location.origin;
const menuTree = computed<NavMenuItems[]>(() => {
    const result : NavMenuItems[] = permissions.value.isDeveloper ? [
        {
            group: 'common',
            menuItems: [
                { label: 'Workspaces', subMenu: [], path: '/workspaces' },
                { label: 'Apps', subMenu: [], path: '/apps' },
                {
                    label: 'Tools',
                    path: '/tools',
                    subMenu: [
                        { label: 'CCDA to FHIR', path: '/ccda', subMenu: [], target: '_blank' },
                        { label: 'FHIR API Providers', path: '/tools/fhir-providers', subMenu: [] }
                    ]
                },
                {
                    label: 'Settings',
                    subMenu: [
                        { label: 'Team Members', subMenu: [], path: '/settings/team-members' },
                        { label: 'Billing & Usage', subMenu: [], path: '/settings/billing-and-usage' },
                        { label: 'Branding',
                            subMenu: [],
                            path: '/settings/branding',
                            hide: subscription.value?.subscriptionType != MeldRxSubscription.Enterprise
                        }
                    ],
                    path: '/settings'
                },
            ]
        },
        {
            group: 'mips',
            menuItems: [
                { label: 'MyMipsScore',
                    hide: !permissions.value.hasMipsReports,
                    subMenu: [],
                    path: `${baseUrl}/mymipsscore`,
                }
            ]
        },
        {
            group: 'support',
            menuItems: [
                { label: 'App Marketplace', subMenu: [], path: '/marketplace' },
                { label: 'Docs', subMenu: [], path: 'https://docs.meldrx.com', target: '_blank' },
                { label: 'Directory', subMenu: [], path: '/directory', target: '_blank' },
            ]
        }
    ] : [
        {
            group: 'common',
            menuItems: [
                {
                    label: 'Tools',
                    path: '/tools',
                    subMenu: [
                        { label: 'CCDA to FHIR', path: '/ccda', subMenu: [], target: '_blank' },
                        { label: 'FHIR API Providers', path: '/tools/fhir-providers', subMenu: [] }
                    ]
                },
            ]
        },
        {
            group: 'support',
            menuItems: [
                { label: 'App Marketplace', subMenu: [], path: '/marketplace' },
                { label: 'Docs', subMenu: [], path: 'https://docs.meldrx.com', target: '_blank' },
                { label: 'Directory', subMenu: [], path: '/directory', target: '_blank' },
            ]
        }
    ];

    for (const navMenuItem of result) {
        resolveActiveRoute(router.currentRoute.value, navMenuItem.menuItems)
    }

    return result;
})

const headerClass1 = computed(() => {
    return menuBackgroundColor.value
        ? `pt-[74px] w-[225px] pb-[50px] fixed z-20 left-0 h-screen overflow-y-auto`
        : 'pt-[74px] w-[225px] pb-[50px] bg-secondary fixed z-20 left-0 h-screen overflow-y-auto';
});

const styleClass1 = computed(() => {
    return menuBackgroundColor.value
        ? { backgroundColor: menuBackgroundColor.value }
        : {};
});
const headerClass2 = computed(() => {
    return menuBackgroundColor.value
        ? `mt-6 relative text-white z-10`
        : 'mt-6 bg-secondary relative text-white z-10';
});

const styleClass2 = computed(() => {
    return menuBackgroundColor.value
        ? { backgroundColor: menuBackgroundColor.value }
        : {};
});

</script>

<template>
  <!-- Top must be equal to the navbar height -->
  <div :class="headerClass1" :style="styleClass1">
    <div :class="headerClass2" :style="styleClass2">
      <div v-for="group in menuTree.filter(group => group.menuItems.some(item => !item.hide))" :key="group.group" class="mb-3">
        <div v-for="item in group.menuItems.filter(item => !item.hide)" :key="item.label">
          <div v-if="item.subMenu.length == 0">
            <ButtonSidebarMenu :label="item.label" :path="item.path" :target="item.target" :active="item.active" />
          </div>
          <div v-else>
            <ButtonGroupSidebarMenu :menu-tree-item="item" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
