import type { Patient, HumanName, ContactPoint } from 'fhir/r4';
import dayjs from 'dayjs';
import type { PatientDto } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/PatientDto';
import { Gender } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/Gender';
import type {InviteDto} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/Invites";
import {InviteStatus} from "~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Enums/InviteStatus";

export default class PatientUtils {
    // Convert a PatientDto to a FHIR Patient...
    public static patientDtoToPatient(patientDto: PatientDto, initialResource?: Patient): Patient {
        const patient: Patient = initialResource || { resourceType: 'Patient' };

        patient.active = true;

        patient.name = patient.name || [];
        patient.name[0] = PatientUtils.getHumanName(patientDto.firstName, patientDto.lastName);

        if (patientDto.dateOfBirth) { patient.birthDate = patientDto.dateOfBirth; }
        if (patientDto.gender !== null) { patient.gender = PatientUtils.genderToFHIRPatientGender(patientDto.gender); }
        if (patientDto.emailAddresses) {
            const newEmail = PatientUtils.getEmailAddressContactPoint(patientDto.emailAddresses);
            if (patient.telecom) {
                const emailIndex = patient.telecom.findIndex(contact => contact.system === 'email');
                if (emailIndex !== -1) {
                    patient.telecom[emailIndex] = newEmail;
                } else {
                    patient.telecom.push(newEmail);
                }
            } else {
                patient.telecom = [newEmail];
            }
        }
        if (patientDto.id) { patient.id = patientDto.id; }

        return patient;
    }

    // Convert a FHIR Patient to a PatientDto...
    public static patientToPatientDto(patient: Patient): PatientDto {
        const firstName = patient.name?.[0]?.given?.[0] || '';
        const lastName = patient.name?.[0]?.family || '';
        const emailAddresses = patient.telecom?.find(telecom => telecom.system === 'email')?.value || '';

        return {
            identifier: null,
            dobWithAge: '',
            id: patient.id || null,
            firstName,
            middleName: null,
            lastName,
            dateOfBirth: patient.birthDate ? patient.birthDate : '',
            gender: PatientUtils.fhirPatientGenderToGender(patient.gender),
            phoneNumber: null,
            emailAddresses,
            address: null,
            enableInvite: 0,
            imageSource: '',
            age: ''
        };
    }

    // Convert a first/last name into a FHIR HumanName...
    private static getHumanName(firstName: string, lastName: string): HumanName {
        return {
            use: 'usual',
            family: lastName,
            given: [firstName]
        };
    }

    // Convert an email address into a FHIR ContactPoint...
    private static getEmailAddressContactPoint(emailAddresses: string): ContactPoint {
        return {
            system: 'email',
            value: emailAddresses
        };
    }

    // Convert a PatientDto Gender to a FHIR Patient Gender...
    private static genderToFHIRPatientGender(gender?: Gender): Patient['gender'] {
        switch (gender) {
            case Gender.Male: return 'male';
            case Gender.Female: return 'female';
            case Gender.Other: return 'other';
            case Gender.Unknown: return 'unknown';
            default: return 'unknown';
        }
    }

    // Convert a FHIR Patient Gender to a PatientDto Gender...
    private static fhirPatientGenderToGender(fhirGender: Patient['gender']): Gender {
        switch (fhirGender) {
            case 'male': return Gender.Male;
            case 'female': return Gender.Female;
            case 'other': return Gender.Other;
            case 'unknown': return Gender.Unknown;
            default: return Gender.Unknown;
        }
    }

    // Parses a date in the format YYYY-MM-DD...
    public static parseDateYYYYMMDD(sDate: string): Date {
        const parts = sDate.split('-');
        if (parts.length !== 3) { throw new Error('Invalid date format'); }
        return new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]));
    }

    // Format a Patient name like: "{firstName} {lastName}"
    public static formatName(patient: Patient): string {
        return `${patient.name?.[0].given?.[0]} ${patient.name?.[0].family}`;
    }

    public static formatSex(patient: Patient): string {
        return patient.gender ?? 'Unknown';
    }

    public static formatDateOfBirth(patient: Patient): string {
        if (!patient.birthDate) { return 'Unknown'; }

        // Parse date so that we can create it in the local timezone...
        const parts = patient.birthDate.split('-');
        if (parts.length !== 3) { return 'Unknown'; }
        const localDate = new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]));
        return localDate.toLocaleDateString();
    }

    public static formatAge(patient: Patient): string {
        if (!patient.birthDate) {
            return 'Unknown';
        }

        const birthDate = dayjs(patient.birthDate);
        const currentDate = dayjs();

        const ageInYears = currentDate.diff(birthDate, 'year');
        const ageInMonths = currentDate.diff(birthDate, 'month') % 12;
        const ageInDays = currentDate.diff(birthDate, 'day') % 30;

        if (ageInYears === 0) {
            if (ageInMonths === 0) {
                return ageInDays === 1 ? '1 day old' : `${ageInDays} days old`;
            }
            return ageInMonths === 1 ? '1 month old' : `${ageInMonths} months old`;
        }

        return ageInYears === 1 ? '1 year old' : `${ageInYears} years old`;
    }

    public static determinePatientInviteStatus(invite:InviteDto ){
        if (invite && invite.acceptedOn == null && !invite.isSynapseRole) {
            return InviteStatus[0]
        } else if (invite && invite.acceptedOn != null) {
            return InviteStatus[1]
        } else if (invite && invite.isSynapseRole) {
            return InviteStatus[2]
        }
        return ''
    }
}
