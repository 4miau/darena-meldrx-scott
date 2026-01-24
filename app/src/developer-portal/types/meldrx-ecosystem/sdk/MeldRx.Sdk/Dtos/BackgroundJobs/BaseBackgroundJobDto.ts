import type { BackgroundJobStatus } from '../../Enums/BackgroundJobStatus';
import type { BackgroundJobType } from '../../Enums/BackgroundJobType';

export interface BaseBackgroundJobDto {
    id: string;
    createdOn: Date;
    modifiedOn: Date;
    completedOn: Date | null;
    type: BackgroundJobType;
    status: BackgroundJobStatus;
    fhirServerId: string | null;
    userId: string;
    clientId: string;
    message: string;
    errorMessages: string[];
    percentageComplete: number;
    title: string;
    description: string;
    externalJobId: string;
}
