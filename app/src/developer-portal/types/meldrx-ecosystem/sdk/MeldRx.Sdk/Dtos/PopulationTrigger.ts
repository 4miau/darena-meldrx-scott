export interface PopulationTrigger {
 id: string,
 populationType: PopulationType
 groupId: string,
 includedIndicators: string[],
 cdsServiceId: string,
}

export interface CreatePopulationTrigger {
 populationType: PopulationType
 groupId: string,
 includedIndicators: string[],
 cdsServiceId: string,
}

export interface UpdatePopulationTrigger {
 id: string,
 populationType: PopulationType
 groupId: string,
 includedIndicators: string[],
 cdsServiceId: string,
}

export enum PopulationType {
 Workspace = "Workspace",
 Group = "Group",
}
