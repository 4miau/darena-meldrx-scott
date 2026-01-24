export type PagedResultInfo = {
    total: number,
    totalPages: number,
    hasNextPage: boolean,
    currentPage: number,
    pageSize: number,
}

export type PagedResult<T> = PagedResultInfo & {
    resources: T[]
}
