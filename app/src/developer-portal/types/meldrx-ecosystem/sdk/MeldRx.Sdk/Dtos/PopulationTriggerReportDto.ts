import type {DateTime} from "#imports";

export interface populationTriggerReportDto {
 id: string;
 populationTriggerId: string;
 reportTime: DateTime;
 blobId: string;
}
