
export default function (objSelector: () => any) {
    const router = useRouter()

    async function updateQuery(obj: any){
        const newQuery = {}
        forceAssignObject(newQuery, obj)
        await router.replace({query: newQuery})
    }

    if(Object.keys(router.currentRoute.value.query).length > 0){
        forceAssignObject(objSelector(), router.currentRoute.value.query)
    } else {
        updateQuery(objSelector())
    }

    watch(objSelector, updateQuery, { deep: true })
}

function forceAssignObject (to: any, from: any) {
    for (const key in from) {
        const item = from[key]
        const itemType = typeof item
        if (itemType === 'object' && itemType === typeof to[key]) {
            if(!(key in to)){
                to[key] = {}
            }

            forceAssignObject(to[key], item)
        } else if (['string', 'number', 'bigint', 'boolean', 'symbol', 'undefined'].indexOf(itemType) >= 0 && item) {
            // NOTE: no need to decode/encode values as the router does it automatically
            to[key] = item
        }
    }
}
