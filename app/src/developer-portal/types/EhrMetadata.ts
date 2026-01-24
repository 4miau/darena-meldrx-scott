import type { EHRs } from './ehrs';
import { SoFAppUserType } from './meldrx-ecosystem/sdk/MeldRx.Sdk/Dtos/DynamicRegistration/SoFAppUserType';

type EhrLoginType = 'provider' | 'patient';

/** Represents a login to an EHR */
export interface IEhrLogin {
    loginType: EhrLoginType;
    username: string;
    password: string;
}

/** Represents a Sandbox server for an EHR */
export interface ISandboxServer {
    name: string;
    userTypes: SoFAppUserType[];
    fhirUrl: string;
    authUrl: string;
    tokenUrl: string;
    logins: IEhrLogin[];
}

/** Represents additional metadata for a given EHR */
export interface IEhrMetadata {
    ehr: EHRs;
    name: string;
    associatedChplIds: string[];
    primaryChplId?: string;
    sandboxServers: ISandboxServer[];
}

/**
 * Metadata associated with certain EHRs.
 * NOTE: These need to be updated periodically. For example, every quarter when Epic releases new versions.
 */

const EHR_METADATA: IEhrMetadata[] = [
    {
        ehr: 'Epic',
        name: 'Epic',
        associatedChplIds: ['11340', '10869', '11262', '11303', '10941', '11120', '10942', '11119', '11304', '11341', '10872', '11263'],
        primaryChplId: '11340', // EpicCare Ambulatory Aug. 2023
        sandboxServers: [
            {
                name: 'Epic Sandbox',
                userTypes: [SoFAppUserType.Patient, SoFAppUserType.Provider, SoFAppUserType.System],
                fhirUrl: 'https://fhir.epic.com/interconnect-fhir-oauth/api/FHIR/R4',
                authUrl: 'https://fhir.epic.com/interconnect-fhir-oauth/oauth2/authorize',
                tokenUrl: 'https://fhir.epic.com/interconnect-fhir-oauth/oauth2/token',
                logins: [
                    { loginType: 'patient', username: 'fhirjason', password: 'epicepic1' },
                    { loginType: 'patient', username: 'fhircamila', password: 'epicepic1' },
                    { loginType: 'patient', username: 'fhirderrick', password: 'epicepic1' },
                    { loginType: 'provider', username: 'FHIR', password: 'EpicFhir1!' }
                ]
            }
        ]
    },

    {
        ehr: 'Cerner',
        name: 'Cerner',
        associatedChplIds: ['10604', '11015'],
        primaryChplId: '11015', // Cerner Millenium
        sandboxServers: [
            {
                name: 'Cerner Patient Sandbox',
                userTypes: [SoFAppUserType.Patient, SoFAppUserType.System],
                fhirUrl: 'https://fhir-myrecord.cerner.com/r4/ec2458f2-1e24-41c8-b71b-0e701af7583d',
                authUrl: 'https://authorization.cerner.com/tenants/ec2458f2-1e24-41c8-b71b-0e701af7583d/protocols/oauth2/profiles/smart-v1/personas/patient/authorize',
                tokenUrl: 'https://authorization.cerner.com/tenants/ec2458f2-1e24-41c8-b71b-0e701af7583d/protocols/oauth2/profiles/smart-v1/token',
                logins: [
                    { loginType: 'patient', username: 'nancysmart', password: 'Cerner01' }
                ]
            },
            {
                name: 'Cerner Provider Sandbox',
                userTypes: [SoFAppUserType.Provider, SoFAppUserType.System],
                fhirUrl: 'https://fhir-ehr-code.cerner.com/r4/ec2458f2-1e24-41c8-b71b-0e701af7583d',
                authUrl: 'https://authorization.cerner.com/tenants/ec2458f2-1e24-41c8-b71b-0e701af7583d/protocols/oauth2/profiles/smart-v1/personas/provider/authorize',
                tokenUrl: 'https://authorization.cerner.com/tenants/ec2458f2-1e24-41c8-b71b-0e701af7583d/protocols/oauth2/profiles/smart-v1/token',
                logins: []
            }
        ]
    },

    {
        ehr: 'AthenaHealth',
        name: 'AthenaHealth',
        associatedChplIds: ['10616', '10917', '10950', '10951', '11259', '11260', '11270', '11271'],
        primaryChplId: '11271',
        sandboxServers: [
            {
                name: 'AthenaHealth Sandbox',
                userTypes: [SoFAppUserType.Patient, SoFAppUserType.Provider, SoFAppUserType.System],
                fhirUrl: 'https://api.preview.platform.athenahealth.com/fhir/r4',
                authUrl: 'https://api.preview.platform.athenahealth.com/oauth2/v1/authorize',
                tokenUrl: 'https://api.preview.platform.athenahealth.com/oauth2/v1/token',
                logins: [
                    { loginType: 'patient', username: 'patientapitest', password: 'Password1!' }
                ]
            }
        ]
    },

    {
        ehr: 'NextGen',
        name: 'NextGen',
        associatedChplIds: ['9372', '10861', '11302', '10850'],
        primaryChplId: '10850',
        sandboxServers: [
            {
                name: 'NextGen Sandbox',
                userTypes: [SoFAppUserType.Patient, SoFAppUserType.Provider, SoFAppUserType.System],
                fhirUrl: 'https://fhir.nextgen.com/nge/prod/fhir-api-r4/fhir/r4',
                authUrl: 'https://fhir.nextgen.com/nge/prod/patient-oauth/authorize',
                tokenUrl: 'https://nativeapi.nextgen.com/nge/prod/nge-oauth/token',
                logins: []
            }
        ]
    },

    {
        ehr: 'Veradigm',
        name: 'Veradigm',
        associatedChplIds: ['11289', '11009'],
        primaryChplId: '11289',
        sandboxServers: [
            {
                name: 'Veradigm EHR Provider',
                userTypes: [SoFAppUserType.Provider, SoFAppUserType.System],
                fhirUrl: 'https://fhir.fhirpoint.open.allscripts.com/fhirroute/fhir/PEHRsandDEV/',
                authUrl: 'https://fhir.fhirpoint.open.allscripts.com/fhirroute/authorizationV2/PEHRsandDEV/connect/authorize',
                tokenUrl: 'https://fhir.fhirpoint.open.allscripts.com/fhirroute/authorizationV2/PEHRsandDEV/connect/token',
                logins: []
            },
            {
                name: 'Veradigm EHR Patient',
                userTypes: [SoFAppUserType.Patient, SoFAppUserType.System],
                fhirUrl: 'https://fhir.fhirpoint.open.allscripts.com/fhirroute/open/PEHRsandDEV/',
                authUrl: 'https://open.allscripts.com/fhirroute/patientauthv2/ea08d4a6-6eab-4cd2-a956-d28f9ef9809e/connect/authorize',
                tokenUrl: 'https://open.allscripts.com/fhirroute/patientauthv2/ea08d4a6-6eab-4cd2-a956-d28f9ef9809e/connect/token',
                logins: []
            },
            {
                name: 'TouchWorks EHR Provider',
                userTypes: [SoFAppUserType.Provider, SoFAppUserType.System],
                fhirUrl: 'https://tw181unityfhir.intranet.open.allscripts.com/R4/fhir-veradigmtwr4/',
                authUrl: 'https://tw181unityfhir.open.allscripts.com/authorizationV2-veradigmtwr4/connect/authorize',
                tokenUrl: 'https://tw181unityfhir.open.allscripts.com/authorizationV2-veradigmtwr4/connect/token',
                logins: []
            },
            {
                name: 'TouchWorks EHR Patient',
                userTypes: [SoFAppUserType.Patient, SoFAppUserType.System],
                fhirUrl: 'https://tw181unityfhir.open.allscripts.com/R4/open-veradigmtwr4/',
                authUrl: 'https://open.allscripts.com/fhirroute/patientauthv2/986f907f-b56c-4ff2-a12d-6c30d20180b2/connect/authorize',
                tokenUrl: 'https://open.allscripts.com/fhirroute/patientauthv2/986f907f-b56c-4ff2-a12d-6c30d20180b2/connect/token',
                logins: []
            },
            {
                name: 'Sunrise Provider',
                userTypes: [SoFAppUserType.Provider, SoFAppUserType.System],
                fhirUrl: 'https://scmfhirconnect.open.allscripts.com:4438/R4/fhir-VeradigmSCMR4/',
                authUrl: 'https://scmfhirconnect.open.allscripts.com:4438/authorizationV2-VeradigmSCMR4/connect/authorize',
                tokenUrl: 'https://scmfhirconnect.open.allscripts.com:4438/authorizationV2-VeradigmSCMR4/connect/token',
                logins: []
            },
            {
                name: 'Sunrise Patient',
                userTypes: [SoFAppUserType.Patient, SoFAppUserType.System],
                fhirUrl: 'https://scmfhirconnect.open.allscripts.com:4438/R4/open-VeradigmSCMR4',
                authUrl: 'https://open.allscripts.com/fhirroute/patientauthv2/738be4a8-d6b7-400d-a773-44ef294c25a3/connect/authorize',
                tokenUrl: 'https://open.allscripts.com/fhirroute/patientauthv2/738be4a8-d6b7-400d-a773-44ef294c25a3/connect/token',
                logins: []
            }
        ]
    }
];

export default EHR_METADATA;
