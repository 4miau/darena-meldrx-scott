import type {Patient, Group, Bundle, FhirResource} from 'fhir/r4';
import type ResourceType from "~/types/fhir/ResourceType";
import type { VirtualWorkspaceOperationPayload } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Fhir/VirtualWorkspaceOperationPayload';
import type { VirtualWorkspaceCreatedResponse } from '~/types/meldrx-ecosystem/sdk/MeldRx.Sdk/Fhir/VirtualWorkspaceCreatedResponse';

export interface FhirApi {
    uploadBundle: (workspaceSlug: string, bundle: Bundle) => Promise<void>;

    search<T>(workspaceSlug: string, resourceType: ResourceType, query: {[key: string] : string}): Promise<Bundle<T>>;

    searchPatients: (workspaceSlug: string) => Promise<Bundle<Patient>>;
    searchPatientsByPage: (workspaceSlug: string, page:number, token:string) => Promise<Bundle<Patient>>;
    searchPatientsByName: (workspaceSlug: string, patientName: string) => Promise<Bundle<Patient>>;
    searchPatientsByIds: (workspaceSlug: string, patientIds: string[]) => Promise<Bundle<Patient>>;
    deletePatient: (workspaceSlug: string, patientId: string) => Promise<void>;
    createPatient: (workspaceSlug: string, patient: Patient) => Promise<Patient>;
    updatePatient: (workspaceSlug: string, patient: Patient) => Promise<Patient>;

    getGroups: (workspaceSlug: string) => Promise<Bundle<Group>>;
    getGroupById: (workspaceSlug: string, groupId: string) => Promise<Group>;
    createGroup: (workspaceSlug: string, group: Group) => Promise<void>;
    updateGroup: (workspaceSlug: string, groupId: string, group: Group) => Promise<void>;
    deleteGroup: (workspaceSlug: string, groupId: string) => Promise<void>;

    getEverythingByPatientId(workspaceSlug: string, patientId: string): Promise<Bundle<FhirResource>>;
    getResourceById<T>(workspaceSlug: string, resourceType: string, resourceId: string): Promise<T>;
    getResourcesByPatientId<T>(workspaceSlug: string, resourceType: string, patientId: string): Promise<Bundle<T>>;
    deleteDocument: (workspaceSlug: string, documentId: string) => Promise<void>;

    getLink<T>(link: string): Promise<Bundle<T>>;
    getBinaryDocument(workspaceSlug: string, documentId: string, contentType: string): Promise<Blob>;

    operations: FhirOperations;
}

export interface FhirOperations {
    virtualWorkspace: (workspaceSlug: string, payload : VirtualWorkspaceOperationPayload) => Promise<VirtualWorkspaceCreatedResponse>
}
