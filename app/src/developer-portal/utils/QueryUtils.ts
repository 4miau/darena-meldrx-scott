import type {ApiFilter, MarketplaceApiFilter} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/ApiFilter";

export default class QueryUtils {
    public static toQueryParams(filter: ApiFilter, queryStart: boolean = false) {
        const resultObj : any = {}

        if (filter.page) {
            resultObj.page = filter.page
        }
        if (filter.filter) {
            resultObj.filter = filter.filter
        }
        if (filter.orderBy) {
            resultObj.orderBy = filter.orderBy
        }
        if (filter.descending) {
            resultObj.descending = filter.descending
        }

        if (Object.keys(resultObj).length === 0) {
            return ''
        }

        return QueryUtils.objectToQuery(resultObj, queryStart)
    }

    public static marketplaceApiFilterToQueryParams(filter: MarketplaceApiFilter, queryStart: boolean = false) {
        const resultObj : any = {}

        if (filter.page) {
            resultObj.page = filter.page
        }
        if (filter.filter) {
            resultObj.filter = filter.filter
        }
        if (filter.category) {
            resultObj.category = filter.category
        }
        if (filter.price) {
            resultObj.price = filter.price
        }
        if (filter.verified) {
            resultObj.verified = filter.verified
        }
        if (filter.hosted) {
            resultObj.hosted = filter.hosted
        }
        if (filter.ascending) {
            resultObj.ascending = filter.ascending
        }

        if (Object.keys(resultObj).length === 0) {
            return ''
        }

        return QueryUtils.objectToQuery(resultObj, queryStart)
    }
    
    public static objectToQuery (obj: any, queryStart: boolean = false){
        const params = []
        for (const key in obj) {
            const val = obj[key]
            if (val !== undefined && val != null && val !== "") {
                params.push(`${key}=${encodeURIComponent(val)}`)
            }
        }

        const query = params.join('&');
        return queryStart ? '?' + query : query
    }
}