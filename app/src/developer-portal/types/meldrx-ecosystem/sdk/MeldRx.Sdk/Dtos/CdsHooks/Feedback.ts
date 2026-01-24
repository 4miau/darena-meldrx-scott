import type {Card, CardOverrideReasonWithComment} from "~/types/cds-hooks/CDSCards";


export interface Feedback {
    card: string;
    outcome: 'accepted' | 'overridden';
    overrideReason?: CardOverrideReasonWithComment;
    outcomeTimestamp: string;
    acceptedSuggestions?: [{'id':string}];
}
export interface FeedbackPayload{
    feedback: Feedback[];
}
export interface CdsFeedbackDto {
    id: string;
    workspaceSlug: string;
    appId: string;
    patientId: string;
    userId: string;
    card: Card;
    feedback: Feedback;
}
