import type { DsiOption } from "../Enums/DsiOption";
import type { PublishedStatus } from "./PublishedApps/PublishedAppDto";

export interface RegisteredAppWithWorkspaceDto {
    appId: string;
    appName: string;
    description: string;
    display: string;
    ehrIntegration: string;
    dsiType: DsiOption;
    publishedStatus: PublishedStatus;
    publisherUrl: string | null;
    termsOfServiceUrl: string | null;
    privacyPolicyUrl: string | null;
    id: string | null;
    organizationName: string | null;
    launchUrl: string | null;
    cdsHookServiceUrl: string | null;
    slug: string | null;
    createdAt: string;
    fhirServerId: string | null;
    hasChaiModelCard: boolean;
}