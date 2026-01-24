export interface CreateWorkspaceProvidersFromNpisCommand {
    npis: string[];
    workspaceSlug: string;
    dateOfActivation: string;
    directoryIds: string[];
}
