export interface DirectoryListingDto{
    id?: string;
    organizationId?: string;
    displayName: string;
    address1: string;
    address2?: string;
    city: string;
    state: string;
    zip: string;
    active: boolean;
    displayAddress?: string;
    fhirServerUrl?: string;
    organization?: string;
}
