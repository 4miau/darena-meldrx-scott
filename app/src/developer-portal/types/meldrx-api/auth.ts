import type { ApplicationUserInfo } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApplicationUserInfo'

export interface AuthApi {
    me: () => Promise<ApplicationUserInfo>
}
