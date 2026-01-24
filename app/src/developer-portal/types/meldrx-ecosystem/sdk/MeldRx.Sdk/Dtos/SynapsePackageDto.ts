import type { SynapsePackageDownloadDto } from './SynapsePackageDownloadDto';

export interface SynapsePackageDto {
    id: string;
    createdAt: string;
    description: string;
    metadata: string;
    expiresAt: string | null;
    linkedPatientId: string;
    linkedWorkspaceId: string | null;
    wasImported: boolean;
    downloadInfo: SynapsePackageDownloadDto;
}
