import type { AppRole } from '../Enums/AppRole';
export interface CreateAppPermissionCommand {
    clientId: string;
    workspaceSlug: string;
    appRole: AppRole;
}
