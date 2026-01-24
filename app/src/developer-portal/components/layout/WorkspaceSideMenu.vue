<script setup lang='ts'>
import ButtonSidebarMenu from './menu/ButtonSidebarMenu.vue'
import ButtonGroupSidebarMenu from './menu/ButtonGroupSidebarMenu.vue'
import { type NavMenuItems, resolveActiveRoute } from '~/types/menu/sidebarMenu'
import type { WorkspaceDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto';
import {Colors} from "~/types/ui/colors";

const router = useRouter();
const { workspace } = useWorkspace();
const { permissions, isAdmin, workspaces } = useAuth();

const optionsWorkspaces = computed<{ value: string, title: string }[]>(
    () => workspaces.value.map(x => ({ value: x.id, title: x.name }))
);

const menuTree = computed<NavMenuItems[]>(() => {
    if (!workspace.value) {
        return []
    }

    const admin = isAdmin();
    const result : NavMenuItems[] = [
        {
            group: 'return-to-workspaces',
            hide: !permissions.value.isDeveloper,
            menuItems: [
                { label: 'Return to Home', subMenu: [], path: '/workspaces', matchExact: true }
            ]
        },
        {
            group: 'return-to-workspaces',
            hide: !admin,
            menuItems: [
                { label: 'Return to Admin', subMenu: [], path: '/administrator/organizations', matchExact: true }
            ]
        },
        {
            group: 'workspace-dsi-linked',
            hide: !workspace.value?.linkedFhirApiDto || admin,
            menuItems: [
                { label: 'Extensions', subMenu: [
                    { label: 'Active Extensions', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/extensions` },
                    { label: 'Workspace Extensions', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/workspace-extensions` },
                ], path: '/extension' },
                { label: 'DSI Feedback', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/feedback` }
            ]
        },
        {
            group: 'workspace-data',
            hide: !!workspace.value?.linkedFhirApiDto || admin,
            menuItems: [
                { label: 'Patients', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/patients` },
                { label: 'Groups', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/groups` },
                { label: 'Import Data', subMenu: [
                    { label: 'Import Data', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/importData` },
                    { label: 'Sample Data', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/sampledata`  },
                ], path: `/importData` },
            ]
        },
        {
            group: 'workspace-dsi',
            hide: !!workspace.value?.linkedFhirApiDto || admin,
            menuItems: [
                { label: 'Extensions', subMenu: [
                    { label: 'Active Extensions', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/extensions` },
                    { label: 'Workspace Extensions', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/workspace-extensions` },
                    {
                        label: 'Population Triggers',
                        subMenu: [],
                        path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/population-trigger`
                    }
                ], path: '/extension' },
                { label: 'DSI Feedback', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/feedback` }
            ]
        },
        {
            group: 'patient-connect',
            hide: !!workspace.value?.linkedFhirApiDto || admin,
            menuItems: [
                {
                    label: 'Patient Connect',
                    subMenu: [
                        { label: 'General', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/patient-connect` },
                        { label: 'Connected Organizations', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/connected-organizations` },
                        { label: 'Received Data', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/packages` }
                    ],
                    path: '/patient-connect'
                }
            ]
        },
        {
            group: 'ai',
            menuItems: [
                {
                    label: "AI",
                    subMenu: [
                        { label: 'Models', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/models` },
                        { label: 'MCP Servers', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/mcp-servers` },
                        { label: 'Playground', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/playground` },
                    ],
                    path: '/ai'
                }
            ]
        },
        !workspace.value?.linkedFhirApiDto
            ? {
                group: 'standalone',
                menuItems: [
                    {
                        label: 'Settings',
                        subMenu: [
                            { label: 'General', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}`, matchExact: true },
                            { label: 'System Apps', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/apps`, hide: !permissions.value.isDeveloper },
                            { label: 'Providers', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/providers` },
                            { label: 'Directory', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/directory` },
                            { label: 'Users', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/users` },
                        ],
                        path: '/settings'
                    }
                ]
            }
            : {
                group: 'linked',
                menuItems: [
                    {
                        label: 'Settings',
                        subMenu: [
                            { label: 'General', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}`, matchExact: true, },
                            { label: 'System Apps', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/apps`, hide: !permissions.value.isDeveloper },
                            { label: 'Data Rules', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/rules` },
                            { label: 'Users', subMenu: [], path: `/workspaces/${workspace.value.fhirDatabaseDisplayName}/users` },
                        ],
                        path: '/settings'
                    }
                ]
            },
        {
            group: 'support',
            menuItems: [
                { label: 'App Marketplace', subMenu: [], path: '/marketplace' },
                { label: 'Directory', subMenu: [], path: '/directory', target: '_blank' },
                { label: 'Docs', subMenu: [], path: 'https://docs.meldrx.com', target: '_blank' }
            ]
        }
    ]

    for (const navMenuItem of result) {
        resolveActiveRoute(router.currentRoute.value, navMenuItem.menuItems)
    }

    return result
})

function onWorkspaceChange(workspaceId: string) {
    const destinationWorkspace = workspaces.value.find(x => x.id === workspaceId) as WorkspaceDto | undefined;
    if (!destinationWorkspace) {
        return;
    }
    if (!destinationWorkspace.linkedFhirApiDto === !workspace.value?.linkedFhirApiDto) {
        navigateTo({ name: router.currentRoute.value.name!, params: { workspaceSlug: destinationWorkspace.fhirDatabaseDisplayName } })
    } else if (destinationWorkspace.linkedFhirApiDto) {
        navigateTo(`/workspaces/${destinationWorkspace.fhirDatabaseDisplayName}`)
    } else {
        navigateTo(`/workspaces/${destinationWorkspace.fhirDatabaseDisplayName}/patients`)
    }
}
</script>


<template>
  <!-- Top must be equal to the navbar height -->
  <div class="pt-[74px] w-[225px] pb-[48px] bg-secondary fixed left-0 z-20 h-screen">
    <div v-if="workspace && !isAdmin()" class="bg-secondary relative text-white z-10 flex flex-col">
      <div class="mt-3 text-white pl-[10px]">
        Workspace
      </div>
      <div>
        <DsSelect
            searchable
            :model-value="workspace.id"
            :items="optionsWorkspaces"
            search-placeholder="Search Workspaces"
            class="px-[10px] z-20 placeholder-silver"
            @update:model-value="onWorkspaceChange"
        />
      </div>
    </div>
    <div v-else-if="workspace && isAdmin()">
        <div class="mt-3 text-white pl-[10px]">
            Workspace
        </div>
        <div class="flex flex-col mt-[10px] pl-[10px] text-xs">
            <DsText size="sm" :color="Colors.white">
                {{workspace.name}}
            </DsText>
        </div>
        <div class="mt-3 text-white pl-[10px]">
            Organization
        </div>
        <div class="flex flex-col mt-[10px] pl-[10px] text-xs">
            <DsText size="xs" class="cursor-pointer hover-underline" :underline="true" :color="Colors.white" @click="navigateTo(`/administrator/organizations?filter=${workspace.organizationId}`)">
                {{workspace.organizationName}}
            </DsText>
        </div>
        <div class="mt-3 text-white pl-[10px]">
            Reseller Organization
        </div>
        <div class="flex flex-col mt-[10px] pl-[10px] text-xs">
            <DsText v-if="workspace.resellerName" size="xs" class="cursor-pointer hover-underline" :underline="true" :color="Colors.white" @click="navigateTo(`/administrator/organizations?filter=${workspace.resellerId}`)">
                {{workspace.resellerName}}
            </DsText>
        </div>
    </div>
      <div class="overflow-y-auto pb-4 z-10">
        <div v-for="group in menuTree.filter(x => !x.hide)" :key="group.group" class="my-3">
          <div v-for="item in group.menuItems.filter(x => !x.hide)" :key="item.label">
            <div v-if="item.subMenu.length == 0">
              <ButtonSidebarMenu
                :label="item.label"
                :path="item.path"
                :target="item.target"
                :font-size="item.fontSize"
                :active="item.active"
              />
            </div>
            <div v-else>
              <ButtonGroupSidebarMenu :menu-tree-item="item"/>
            </div>
          </div>
        </div>
      </div>
  </div>
</template>
