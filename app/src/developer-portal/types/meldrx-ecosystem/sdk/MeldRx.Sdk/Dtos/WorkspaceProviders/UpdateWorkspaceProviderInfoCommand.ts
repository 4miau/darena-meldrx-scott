export interface UpdateWorkspaceProviderInfoCommand {
    providerId: string;
    displayName: string;
    dateOfActivation: string;
    directoryIds: string[];
}
