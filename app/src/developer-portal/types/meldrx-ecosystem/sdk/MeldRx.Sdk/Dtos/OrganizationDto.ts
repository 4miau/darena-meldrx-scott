import type { PartnerType } from '../Enums/PartnerType';
import type { OrganizationType } from './../Enums/OrganizationType';
import type { WorkspaceDto } from './WorkspaceDto';

export interface CreateOrganizationWithAdminDto {
    type: OrganizationType;
    name: string;
    tin: string;
    primaryContactFirstName: string;
    primaryContactLastName: string;
    primaryContactEmail: string | null;
}

export interface CreateOrganizationWithAdminResultDto {
    id: string;
    type: OrganizationType;
    name: string;
    userId: string;
}

export interface OrganizationDto {
    id: string;
    type: OrganizationType;
    name: string;
    tin: string;
    address: string;
    address2: string;
    city: string;
    state: string;
    zipCode: string;
    phone: string;
    fax: string;
    partnerType: PartnerType | null;
    primaryContactFirstName: string;
    primaryContactLastName: string;
    email: string;
    aadTenantId: string;
    aadAutoActiveUserGroups: string[];
    fhirServers: WorkspaceDto[];
    ehrOrganizationId: string | null;
    ehrOrganizationName: string;
    hasMipsReports: boolean;
}
