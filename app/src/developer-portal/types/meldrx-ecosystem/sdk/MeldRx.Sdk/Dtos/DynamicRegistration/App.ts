import type {SoFAppUserType} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType";

export interface AppDto {
    appId: string;
    appName: string;
    appType: SoFAppUserType;
    orgName: string;
    orgId: string;
}