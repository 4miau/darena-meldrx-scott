export interface CreateWorkspaceProviderCommand {
    npi: string;
    providerName: string;
    workspaceSlug: string;
    dateOfActivation: string;
    directoryIds: string[];
}
