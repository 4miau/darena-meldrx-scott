import type { Guid } from '~/types/common/Guid';

export interface PersonDto {
    id: Guid;
    firstName: string;
    middleName: string;
    lastName: string;
    gender: any;
    birthDate: Date;
    addressLine1: string;
    addressLine2: string;
    city: string;
    state: string;
    zipCode: string;
    creatingFhirServer: boolean;
    identifiers: any[];
    contacts: any[];
}
