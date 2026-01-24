import type { DownloadSynapsePackageModel } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DownloadSynapsePackageModel';
import type { SynapsePackageDto } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/SynapsePackageDto';
import type { CreateSynapsePackageDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CreateSynapsePackageDto'

export interface SynapseApi {
    getPackages: (workspaceSlug: string) => Promise<SynapsePackageDto[]>;
    create: (pkg: CreateSynapsePackageDto) => Promise<SynapsePackageDto>;
    deletePackage: (workspaceSlug: string, packageId: string) => Promise<void>;
    importPackage: (workspaceSlug: string, downloadSynapsePackageModel: DownloadSynapsePackageModel) => Promise<void>;
}
