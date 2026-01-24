export interface SourceAttributeItem {
    id: number;
    description: string;
    display: string;
    answer: string;
}

export interface SourceAttributeGroup {
    id: number;
    description: string;
    sourceAttributeItems: SourceAttributeItem[];
}