import type { DsiOption } from "../Enums/DsiOption";

export interface WorkspaceEhrAppDto {
    id: string;
    launchUrl: string;
    cdsHookServiceUrl:string;
    dsiType: DsiOption
    slug: string;
    appId: string;
    appName: string;
    description:string;
    createdAt: string | null;
    organizationName: string;
    publisherUrl?: string;
    termsOfServiceUrl?: string;
    privacyPolicyUrl?: string;
    hasChaiModelCard: boolean;
}
