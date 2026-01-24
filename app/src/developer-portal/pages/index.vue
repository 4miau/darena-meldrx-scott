<script setup lang="ts">
import { Colors } from "~/types/ui/colors";

definePageMeta({
    layout: 'blank',
    middleware: [
        function(){

            const { currentRoute } = useRouter()
            const { user, isAdmin, workspaces, people } = useAuth()
            if(currentRoute.value.query.select === 'true'){
                return
            }

            //superadmin gets redirected to admin
            if (isAdmin()) {
                return navigateTo('/administrator/workspaces')
            }

            // Developer users get redirected to workspaces if they have no people...
            if (user.value.permissions.isDeveloper && !user.value.permissions.hasPeople) {
                return navigateTo('/workspaces')
            }

            // If user has only one workspace and no people, navigate to that workspace...
            if (user.value.permissions.hasWorkspaces && !user.value.permissions.hasPeople && workspaces.value.length === 1) {
                if (workspaces.value[0].linkedFhirApiDto !== null) {
                    return navigateTo(`/workspaces/${workspaces.value[0].fhirDatabaseDisplayName}/patients`)
                } else {
                    return navigateTo(`/workspaces/${workspaces.value[0].fhirDatabaseDisplayName}`)
                }
            }

            // If user has only one person and no workspaces, navigate to that person...
            if (user.value.permissions.hasPeople && !user.value.permissions.hasWorkspaces && people.value.length === 1 && !user.value.permissions.isDeveloper) {
                return navigateTo(`/person/${people.value[0].person.id}`)
            }
        }
    ]
})
useHead({ title: 'Home | MeldRx' })

const { user, workspaces, people } = useAuth()
const show = computed(() => ({
    username: user.value.claims.name?.split(' ').at(0) ?? user.value.claims.email,
    workspaceSelector: workspaces.value.length > 0,
    peopleSelector: people.value.length > 0,
}))

</script>

<template>
  <div class="absolute top-0 left-0 h-screen w-screen pt-[100px] bg-bliss">
    <div class="text-center mb-5">
      <DsText size="2xl" weight="light" :color="Colors.onyx" class="block">
        Welcome, <DsText size="2xl">{{ show.username }}</DsText>!
      </DsText>
    </div>

    <div class="grid grid-cols-12 gap-4">

      <!-- Workspaces -->
      <div
          v-if="show.workspaceSelector"
          :class="(show.workspaceSelector && show.peopleSelector) ? 'col-start-1 col-span-6 xl:col-start-2 xl:col-span-5' : 'col-start-4 col-span-6 2xl:col-start-5 2xl:col-span-4'"
      >
        <div class="bg-bliss">
          <div class="bg-onyx text-center p-2">
            <DsText size="xl" weight="light" :color="Colors.white">
              My Workspaces
            </DsText>
          </div>

          <div class="grid grid-cols-1 border border-silver">
            <DsLink
                v-for="workspace in workspaces"
                :key="workspace.id"
                class="bg-white px-2 pt-2 last:pb-2"
                underline="always"
                :href="`/workspaces/${workspace.fhirDatabaseDisplayName}`"
            >
              <div class="p-2 border border-silver hover:bg-seafoam">
                <DsAvatar :alt="workspace.name[0]" size="sm" class="border border-dsprimary bg-bliss text-silver"/>
                <DsText size="md">
                  {{ workspace.name }}
                </DsText>
              </div>
            </DsLink>
          </div>
        </div>
      </div>

      <!-- People -->
      <div
          v-if="show.peopleSelector"
          :class="(show.workspaceSelector && show.peopleSelector)
            ? 'col-start-7 col-span-6 xl:col-start-7 xl:col-span-5'
            : 'col-start-4 col-span-6 2xl:col-start-5 2xl:col-span-4'"
      >
        <div class="bg-bliss">
          <div class="bg-onyx text-center p-2">
            <DsText size="xl" weight="light" :color="Colors.white">
              My People
            </DsText>
          </div>

          <div class="grid grid-cols-1 border border-silver">
            <DsLink
                v-for="person in people"
                :key="person.person.id"
                class="bg-white px-2 pt-2 last:pb-2"
                underline="always"
                :href="`/person/${person.person.id}`"
            >
              <div class="p-2 border border-silver hover:bg-seafoam">
                <DsAvatar :alt="person.person.firstName[0]" size="sm" class="border border-dsprimary bg-bliss text-silver"/>
                <DsText size="md">
                  {{ person.person.firstName + ' ' + person.person.lastName }}
                </DsText>
              </div>
            </DsLink>
          </div>
        </div>
      </div>
    </div>
    <div v-if="user.permissions.isDeveloper" class="flex p-5 justify-center">
      <DsButton :color="Colors.primary" variant="filled" size="md" @click="navigateTo('/workspaces')">
        Go to MeldRx
      </DsButton>
    </div>
  </div>
</template>
