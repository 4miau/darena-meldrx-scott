import type { BaseBackgroundJobDto } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/BackgroundJobs/BaseBackgroundJobDto';
import type { BackgroundJobStatus } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/BackgroundJobStatus';
import type { BackgroundJobType } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/BackgroundJobType';
import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'

export interface DataImportApi {
    importCcda: (workspaceSlug:string, fileContents:string)=>Promise<void>;
    listJobs:(
        workspaceSlug: string,
        jobType: BackgroundJobType,
        jobStatus: BackgroundJobStatus,
        startDate: string,
        endDate: string,
        page?: number | null | undefined,
        size?: number | null | undefined
     ) =>Promise<PagedResult<BaseBackgroundJobDto>>;
}
