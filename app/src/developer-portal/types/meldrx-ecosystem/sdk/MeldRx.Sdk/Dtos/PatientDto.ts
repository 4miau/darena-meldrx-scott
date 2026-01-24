import type { Address } from 'fhir/r4';
import type { Gender } from '../Enums/Gender';
import { PatientLinkStatus } from '../Enums/PatientLinkStatus';

export interface PatientDto {
    identifier: string | null;
    dobWithAge: string;
    id: string | null;
    firstName: string;
    middleName: string | null;
    lastName: string;
    dateOfBirth: string;
    gender?: Gender;
    phoneNumber: string | null;
    emailAddresses: string | null;
    address: Address | null;
    enableInvite: PatientLinkStatus;
    imageSource: string;
    age: string;
}

export function getDefaultPatientDto(): PatientDto {
    return {
        identifier: null,
        dobWithAge: '',
        id: null,
        firstName: '',
        middleName: null,
        lastName: '',
        dateOfBirth: '',
        phoneNumber: null,
        emailAddresses: null,
        address: null,
        enableInvite: PatientLinkStatus.Unlinked,
        imageSource: '',
        age: ''
    };
}
