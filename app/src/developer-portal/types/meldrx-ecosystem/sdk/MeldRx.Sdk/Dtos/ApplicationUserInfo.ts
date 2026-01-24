import type { Claims } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Claims'
import type { UserPermissionsDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/UserPermissionDto'

export interface ApplicationUserInfo {
    claims: Claims;
    permissions: UserPermissionsDto;
}
