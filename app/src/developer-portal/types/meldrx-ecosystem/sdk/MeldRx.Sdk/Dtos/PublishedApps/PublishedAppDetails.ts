import type {DsiOption} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/DsiOption";
import type {SourceAttributeGroup} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/SourceAttribute";
import type {ChaiModelCardGroup} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/ChaiModelCardForm";
import type {PublishedStatus} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/PublishedAppDto";

export interface PublishedAppDetails {
    publishedStatus: PublishedStatus;
    appId: string;
    appName: string;
    description: string;
    ehrIntegration: string;
    dsiType: DsiOption;
    logoUrl?: string;
    publisherUrl: string;
    termsOfServiceUrl: string;
    privacyPolicyUrl: string;
    organizationName: string;
    sourceAttributeGroups?: SourceAttributeGroup[];
    chaiModelCardGroups?: ChaiModelCardGroup[];
    isPaid: boolean;
    meldRxVerified: boolean;
    meldRxHosted: boolean;
    intendedUsers: string[];
    lastUpdated?: string
}

export interface PublishedAppDetailsForMarketplace {
    appId: string;
    appName: string;
    descriptionBrief: string;
    ehrIntegration: string;
    dsiType: DsiOption;
    logoUrl?: string;
    publisherUrl: string;
    organizationName: string;
    isPaid?: boolean;
    meldRxVerified: boolean;
    meldRxHosted: boolean
}
