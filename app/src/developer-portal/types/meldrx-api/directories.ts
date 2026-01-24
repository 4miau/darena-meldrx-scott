import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'
import type { DirectoryTableView } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Directories/DirectoryTableView'
import type { Guid } from '~/types/common/Guid'
import type { DirectoryListingDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DirectoryListingDto'
import type { DirectoryProviderTableView } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Directories/DirectoryProviderTableView'
import type { ApiFilter } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter'

export interface DirectoriesApi {
    list: (apiFilter: ApiFilter) => Promise<PagedResult<DirectoryTableView>>;
    get: (id: Guid) => Promise<DirectoryListingDto>;
    listProviders: (directoryId: Guid, apiFilter: ApiFilter) => Promise<PagedResult<DirectoryProviderTableView>>;
}
