export interface SynapsePackageDownloadDto {
    isResourceBeingDownloaded: boolean;
    downloadContextToken: string;
    downloadBackgroundJobId: string;
    downloadTotal: number | null;
    wasDownloaded: boolean;
}
