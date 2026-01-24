import { computed } from 'vue'
import type { ApplicationUserInfo } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApplicationUserInfo'
import type { WorkspaceDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDto'
import type { PersonDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PersonDto'

function createEmptyUser (): ApplicationUserInfo {
    return {
        claims: {},
        permissions: {
            isDeveloper: false,
            hasPeople: false,
            hasWorkspaces: false,
            developerPermissionsDto: null,
            workspaceUserPermissionDto: null,
            hasMipsReports: false
        }
    }
}

export interface PersonMetadata {
    person: PersonDto;
    patients: {patientId: string, workspace: WorkspaceDto}[];
}

export default function () {
    const { $api } = useNuxtApp()
    const userState = useState<{
        init: boolean;
        user: ApplicationUserInfo;
        people: PersonMetadata[];
        workspaces: WorkspaceDto[];
    }>(
        'userState',
        () => ({
            init: false,
            user: createEmptyUser(),
            people: [],
            workspaces: [],
        })
    )

    async function loadUserWorkspaces(){
        await $api.users.workspaces()
            .then(workspaces => userState.value.workspaces = workspaces)
    }
    async function loadUser () {
        try {
            userState.value.user = await $api.auth.me()

            const permissions = userState.value.user.permissions
            const loadData = []
            if (permissions.hasWorkspaces || permissions.isDeveloper) {
                loadData.push(
                    loadUserWorkspaces()
                )
            }

            if (permissions.hasPeople) {
                loadData.push(
                    $api.users.people()
                        .then(people => {
                            for (const person of people) {
                                const workspacePatient = {
                                    patientId: person.accessiblePatientId,
                                    workspace: person.workspaceDto,
                                }

                                const existingPerson = userState.value.people
                                    .find(x => x.person.id === person.personDto.id)

                                if (existingPerson) {
                                    existingPerson.patients.push(workspacePatient)
                                } else {
                                    userState.value.people.push(
                                        {
                                            person: person.personDto,
                                            patients: [workspacePatient]
                                        }
                                    )
                                }
                            }
                        })
                )
            }

            if (loadData.length > 0) {
                await Promise.all(loadData)
            }

        } catch (e: any) {
            if (e.response?.status === 401) {
                userState.value.user = createEmptyUser()
            }
        } finally {
            userState.value.init = true
        }
    }

    return {
        initialized: () => userState.value.init,
        user: computed(() => userState.value.user),
        permissions: computed(() => userState.value.user.permissions),
        workspaces: computed(() => userState.value.workspaces),
        people: computed(() => userState.value.people),
        loadUser,
        loadUserWorkspaces,
        authenticated: computed(() => !!userState.value.user.claims && Object.keys(userState.value.user.claims).length > 0),
        isAdmin: () => (userState.value.user.claims.role === 'SuperAdmin'),
        isLinked: () => 'linked-workspace-id' in userState.value.user.claims,
        isLaunch: () => 'launch' in userState.value.user.claims,
        name: computed(() => userState.value.user.claims.name ?? ''),
    }
}
