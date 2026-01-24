import type {DsiOption} from "../../Enums/DsiOption";
import type {SourceAttributeGroup} from "./SourceAttribute";
import type {SoFAppUserType} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType";
import type {ChaiModelCardGroup} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/ChaiModelCardForm";

export enum PublishedStatus {
    NotPublished,
    Published
}

export interface PublishedAppDto {
    publishedStatus: PublishedStatus;
    appId: string;
    appName: string;
    description: string;
    descriptionBrief: string;
    display: string;
    soFAppUserType: SoFAppUserType
    dsiType: DsiOption;
    ehrIntegration: string;
    publisherUrl: string;
    termsOfServiceUrl: string;
    privacyPolicyUrl: string;
    id: string;
    logoUrl: string;
    isPaid: boolean;
    intendedUsers: string[];
    sourceAttributeGroups?: SourceAttributeGroup[];
    chaiModelCardGroups?: ChaiModelCardGroup[];
}
