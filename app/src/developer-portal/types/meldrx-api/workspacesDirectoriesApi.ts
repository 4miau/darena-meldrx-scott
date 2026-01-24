import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'
import type { DirectoryListingDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DirectoryListingDto'
import type { ApiFilter } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter'
import type { DirectorySelectorView } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceDirectory/DirectorySelectorView'

export interface WorkspacesDirectoriesApi {
    list:(workspaceSlug: string, apiFilter: ApiFilter) => Promise<PagedResult<DirectoryListingDto>>;
    listAll:(workspaceSlug: string) => Promise<DirectorySelectorView[]>;
    create:(workspaceSlug: string, directorySettings: DirectoryListingDto) => Promise<DirectoryListingDto>;
    update:(workspaceSlug: string, directorySettings: DirectoryListingDto) => Promise<DirectoryListingDto>;
    updateStatus:(workspaceSlug: string, directoryListingId: string, isActive: boolean) => Promise<void>;
}
