export enum MeldRxSubscription {
    DeveloperSubscription = 'developer',
    BasicSubscription = 'basic',
    BasicSubscriptionAnnual = 'basic-annual',
    AdvancedSubscription = 'advanced',
    AdvancedSubscriptionAnnual = 'advanced-annual',
    Enterprise = 'enterprise'
}
export const MeldRxSubscriptionConfig = [
    { value:MeldRxSubscription.DeveloperSubscription,title: 'developer'},
    { value:MeldRxSubscription.BasicSubscription,title: 'basic'},
    { value:MeldRxSubscription.BasicSubscriptionAnnual,title: 'basic-annual'},
    { value:MeldRxSubscription.AdvancedSubscription,title: 'advanced'},
    { value:MeldRxSubscription.AdvancedSubscriptionAnnual,title: 'advanced-annual'},
    { value:MeldRxSubscription.Enterprise,title: 'enterprise'}
]