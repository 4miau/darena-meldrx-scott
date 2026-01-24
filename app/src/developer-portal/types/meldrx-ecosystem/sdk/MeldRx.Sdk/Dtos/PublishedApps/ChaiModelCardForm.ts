export interface ChaiModelCardItem {
    id: number
    display: string;
    answer: string;
}

export interface ChaiModelCardGroup {
    id: number;
    display: string;
    chaiModelCardItems: ChaiModelCardItem[];
}