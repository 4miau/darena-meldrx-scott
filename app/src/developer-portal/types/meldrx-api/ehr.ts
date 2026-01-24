import type { AddWorkspaceEhrAppCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/AddWorkspaceEhrAppCommand';
import type { WorkspaceEhrAppDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/WorkspaceEhrAppDto';
import type { PagedResult } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Models/PagedResult';
import type { CreateEhrLaunchContextCommand } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/EhrLaunchContextDto';
import type { EhrLaunchContextCreateResponse } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/EhrLaunchContextCreateResponse'
import type { ApiFilter } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter'
import type { AppDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/App'
import type { SourceAttributeGroup } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/SourceAttribute';
import type { RegisteredAppWithWorkspaceDto } from '../meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/RegisteredAppWithWorkspaceDto';

export interface EhrApi {
    list: (workspaceSlug: string, apiFilter:ApiFilter) => Promise<PagedResult<WorkspaceEhrAppDto>>;
    listAvailableApps: (workspaceSlug: string) => Promise<AppDto[]>;
    listOrgAccessibleApps: (workspaceSlug: string, apiFilter:ApiFilter) => Promise<PagedResult<RegisteredAppWithWorkspaceDto>>;
    listOrgOnlyApps: (workspaceSlug: string, apiFilter:ApiFilter) => Promise<PagedResult<RegisteredAppWithWorkspaceDto>>;
    listWorkspaceApps: (workspaceSlug: string, apiFilter:ApiFilter, activeOnly?:boolean) => Promise<PagedResult<RegisteredAppWithWorkspaceDto>>;
    getDsiSourceAttributes: (workspaceSlug: string, ehrAppId: string) => Promise<SourceAttributeGroup[]>;
    putDsiSourceAttributes: (workspaceSlug: string, ehrAppId: string, SourceAttributeGroup: SourceAttributeGroup[]) => Promise<AppDto[]>;
    listEhrLaunchApps: (workspaceSlug: string, apiFilter:ApiFilter) => Promise<PagedResult<WorkspaceEhrAppDto>>;
    create: (workspaceSlug: string, command: AddWorkspaceEhrAppCommand) => Promise<WorkspaceEhrAppDto>;
    delete: (workspaceSlug: string, ehrAppId: string) => Promise<void>;
    createContext:(workspaceSlug:string, command: CreateEhrLaunchContextCommand) => Promise<EhrLaunchContextCreateResponse>;
}
