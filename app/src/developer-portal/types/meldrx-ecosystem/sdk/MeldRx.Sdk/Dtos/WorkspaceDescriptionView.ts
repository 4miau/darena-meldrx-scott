import type { Guid } from '~/types/common/Guid'

export type WorkspaceDescriptionView = {
    id: Guid;
    name: string;
    deleted: boolean;
}
