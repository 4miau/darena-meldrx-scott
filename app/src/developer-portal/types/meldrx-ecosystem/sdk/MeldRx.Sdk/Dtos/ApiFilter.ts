export interface ApiFilter {
    page?: number,
    filter?: string,
    orderBy?: string,
    descending?: boolean,
}

export interface MarketplaceApiFilter {
    page?: number,
    filter?: string,
    category?: string,
    price?: string,
    verified?: string,
    hosted?: string,
    ascending?: boolean,
}
