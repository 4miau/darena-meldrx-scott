import type { Guid } from '~/types/common/Guid'

export interface DirectoryTableView {
    id: Guid;
    practiceName: string;
    ehrVendor: string;
    providerCount: number;
    workspaceSlug: string;
    workspaceUrl: string;
}
