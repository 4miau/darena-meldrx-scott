import type {Card} from "~/types/cds-hooks/CDSCards";

export interface PopulationTriggerReportData {
 patientId: string;
 patientName: string;
 gender: string;
 doB: string;
 includedIndicators: string[];
 cards: Card[];
}
