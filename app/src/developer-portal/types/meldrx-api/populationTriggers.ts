import type {populationTriggerReportDto} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PopulationTriggerReportDto";
import type {PopulationTriggerReportData} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PopulationTriggerReportData";
import type {
    CreatePopulationTrigger,
    PopulationTrigger,
    UpdatePopulationTrigger
} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PopulationTrigger";


export interface PopulationTriggersApi {
    createPopulationTrigger:(workspaceSlug:string, form:CreatePopulationTrigger ) => Promise<PopulationTrigger>,
    updatePopulationTrigger:(workspaceSlug:string, form:UpdatePopulationTrigger , populationTriggerId:string ) => Promise<PopulationTrigger>,
    deletePopulationTrigger:(workspaceSlug:string, populationTriggerId:string ) => Promise<boolean>,
    getPopulationTriggers:(workspaceSlug:string) => Promise<PopulationTrigger[]>,
    runPopulationTrigger:(workspaceSlug:string, populationTriggerId:string) => Promise<void>,
    getPopulationTriggerReports:(workspaceSlug:string, populationTriggerId:string) => Promise<populationTriggerReportDto[]>,
    getPopulationTriggerReportData:(workspaceSlug:string, reportId:string) => Promise<PopulationTriggerReportData[]>,
}
