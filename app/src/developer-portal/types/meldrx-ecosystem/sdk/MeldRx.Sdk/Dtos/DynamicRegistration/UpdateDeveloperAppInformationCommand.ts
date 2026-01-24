export interface UpdateDeveloperAppInformationCommand {
    clientId: string;
    clientName: string;
    publisherUrl: string;
    scope: string[];
    redirectUrls: string[];
    jwksUri: string | null;
    ehrLaunchUrl: string;
    cdsHookServiceUrl:string;
}
