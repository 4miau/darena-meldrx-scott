import type {DsiOption} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/DsiOption";
import type {CdsAppType} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/CdsAppType";
import type {SourceAttributeGroup} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/SourceAttribute";
import type { CdsHookForm } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CdsHookForm';
import type { CardForm } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/CardForm';
import type {ChaiModelCardGroup} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PublishedApps/ChaiModelCardForm";

export interface WorkspaceExtensionForm {
 clientId: string,
 clientName: string,
 cdsAppType:CdsAppType,
 dsiType:DsiOption,
 cdsHookServiceUrl?: string,
 cdsHook?: CdsHookForm;
 cards?: CardForm[];
 cqlEditorArtifact?: any;
 sourceAttributeGroups?: SourceAttributeGroup[],
 chaiModelCardGroups?: ChaiModelCardGroup[];
}
