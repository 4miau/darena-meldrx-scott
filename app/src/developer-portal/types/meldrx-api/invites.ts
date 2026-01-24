import type {
    CreateInviteDto,
    InviteCreateResponseDto,
    InviteDto,
    SendInviteDto
} from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Invites';
import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult'

export interface InviteApi {
    create: (workspaceSlug: string, model: CreateInviteDto) => Promise<InviteCreateResponseDto>;
    send: (workspaceSlug: string, id: string, sendInviteDto: SendInviteDto) => Promise<void>;
    delete: (workspaceSlug: string, id: string) => Promise<void>;
    findSent: (workspaceSlug: string) => Promise<PagedResult<InviteDto>>;
    search: (workspaceSlug:string, patientIds: string[]) => Promise<InviteDto[]>
}
