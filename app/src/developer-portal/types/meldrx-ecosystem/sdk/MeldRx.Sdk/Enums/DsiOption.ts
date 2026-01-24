
export enum DsiOption {
    None = 'None',
    Predictive = 'Predictive',
    EvidenceBased = 'EvidenceBased',
}

export function DsiOptionMap(v:DsiOption) {
    if(v === DsiOption.None){
        return 'None'
    }
    if(v === DsiOption.Predictive){
        return 'Predictive'
    }
    if(v === DsiOption.EvidenceBased){
        return 'Evidence Based'
    }
}