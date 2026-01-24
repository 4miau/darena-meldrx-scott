import type {Coding, FhirResource} from "fhir/r4";

export type CdsServiceResponse = {
    cards: Card[];
}

export type Card = {
    summary: string;
    indicator: string;
    detail?: string;
    source: CardSource;
    links?: CardLink[];
    suggestions?: CardSuggestion[];
    uuid: string;
    serviceUrl?: string;
    overrideReasons?: CardOverrideReason[];
    extension?: { [key: string]: string; };
}

export type CardSuggestion = {
    label: string,
    uuid?: string,
    isRecommended?: boolean,
    actions?: CardAction[]
}

export type CardAction = {
    type: string,
    description: string,
    resource?: FhirResource,
    resourceId?: string
}

export type CardSource = {
    label: string,
    url?: string,
    icon?: string,
    topic?: Coding
}

export type CardLink = {
    label: string,
    url: string,
    type: string,
    appContext?: string,
    autolaunchable?: boolean,
}

export type CardOverrideReasonWithComment = {
    reason: CardOverrideReason;
    userComment?: string;
}

export type CardOverrideReason = {
    display?: string;
    code: string;
    system: string;
}

export type CdsEventCommand = {
    eventType: string;
    patientId: string;
}

export enum CdsIndicator {
    Critical = 'Critical',
    Warning = 'Warning',
    Info = 'Info',
    All = 'All'
}
