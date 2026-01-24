import type {CdsEventCommand} from "~/types/cds-hooks/CDSCards";
import type {FeedbackPayload} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/CdsHooks/Feedback";
import type { MeldRxCdsEventResponse } from '~/types/cds-hooks/MeldRxCdsEventResponse';

export interface CdsServicesApi {
    getCards: (workspaceId: string, command: CdsEventCommand) => Promise<MeldRxCdsEventResponse>;
    postFeedback:(workspaceId: string, serviceId:string, command: FeedbackPayload) => Promise<void>;
}
